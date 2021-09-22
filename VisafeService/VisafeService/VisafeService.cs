using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Reflection;

namespace VisafeService
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };

    public partial class VisafeService : ServiceBase
    {
        private Thread _thread = null;
        private EventLog _eventLog;
        private DNSServer _dnsServer;
        //private Thread _threadCheckVisafe = null;
        private Updater _updater;

        public VisafeService()
        {
            _eventLog = new EventLog("Application");
            _eventLog.Source = "VisafeService";
            _dnsServer = new DNSServer();
            _updater = new Updater(Constants.VERSION_INFO_URL);

            //var dnsProxyProcesses = Process.GetProcessesByName("dnsproxy");
            //foreach (Process pr in dnsProxyProcesses)
            //{
            //    pr.Kill();
            //}

            //_updater = new Updater(Constants.VERSION_INFO_URL);

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _thread = new Thread(new ThreadStart(Run));
            _thread.Start();

            //_threadCheckVisafe = new Thread(new ThreadStart(CheckVisafeRunning));
            //_threadCheckVisafe.Start();
        }

        protected override void OnStop()
        {
            _dnsServer.Stop();
            _thread.Abort();            
        }

        private void CheckVisafeRunning()
        {
            while (true)
            {
                //if the process Visafe is killed or stopped, restart it
                Process[] proc = Process.GetProcessesByName("Visafe");
                if (proc.Length == 0)
                {
                    string currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string visafeExeFile = Path.Combine(currentFolder, "Visafe.exe");

                    if (File.Exists(visafeExeFile))
                    {
                        Helper.RunExecutable(visafeExeFile);
                    }
                }

                Thread.Sleep(5000);
            }
        }
        private void Run()
        {
            while (true)
            {
                PipeSecurity ps = new PipeSecurity();
                ps.AddAccessRule(new PipeAccessRule("Users", PipeAccessRights.ReadWrite, AccessControlType.Allow));
                ps.AddAccessRule(new PipeAccessRule(System.Security.Principal.WindowsIdentity.GetCurrent().Name, PipeAccessRights.FullControl, AccessControlType.Allow));
                ps.AddAccessRule(new PipeAccessRule("SYSTEM", PipeAccessRights.FullControl, AccessControlType.Allow));

                var pipeServer = new NamedPipeServerStream("VisafeServicePipe", 
                    PipeDirection.InOut, 
                    1,
                    PipeTransmissionMode.Message,
                    PipeOptions.Asynchronous,
                    511, 511, ps);

                try
                {
                    pipeServer.WaitForConnection();

                    StreamString ss = new StreamString(pipeServer);

                    string receivedString = ss.ReadString();
                    
                    _eventLog.WriteEntry("Receive signal from client: " + receivedString, EventLogEntryType.Information);

                    Dictionary<string, string> signalData = ParseSignalString(receivedString);

                    try
                    {
                        if (signalData["signal"] == "start")
                        {
                            //_dnsServer.Start(signalData["user_id"]);
                            _dnsServer.Start();
                            ss.WriteString("received"); //recevie signal from client and return it back to confirm
                        }
                        else if (signalData["signal"] == "stop")
                        {
                            _dnsServer.Stop();
                            ss.WriteString("received"); //recevie signal from client and return it back to confirm
                        }
                        else if (signalData["signal"] == "check_for_update")
                        {
                            bool newVersion = _updater.CheckForUpdate();

                            if (newVersion == true)
                            {
                                ss.WriteString("new");
                            }
                            else
                            {
                                ss.WriteString("no_new");
                            }
                        }
                        else if (signalData["signal"] == "update")
                        {
                            _dnsServer.Stop();
                            ss.WriteString("received");

                            _updater.Upgrade();
                        }
                        else if (signalData["signal"] == "get_id")
                        {
                            string user_id = Helper.GetID();
                            ss.WriteString(user_id);
                        }
                    }
                    catch (Exception e)
                    {
                        _eventLog.WriteEntry(e.Message, EventLogEntryType.Error);
                    }
                }
                catch (IOException e)
                {
                    _eventLog.WriteEntry(e.Message, EventLogEntryType.Error);
                }
                pipeServer.Close();
            }
        }

        private Dictionary<string, string> ParseSignalString(string signalDataString)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            string[] dataArr = signalDataString.Split(';');

            foreach (string field in dataArr)
            {
                if (field != "")
                {
                    field.Trim();   //remove spaces at the beginning and ending of the string

                    string[] delimeters = { "<<" };
                    string[] fieldArr = field.Split(delimeters, StringSplitOptions.RemoveEmptyEntries);

                    result[fieldArr[0].Trim()] = fieldArr[1].Trim();
                }
            }

            return result;
        }
    }
}
