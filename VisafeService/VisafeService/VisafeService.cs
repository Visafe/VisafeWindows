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

    //public partial class VisafeService : ServiceBase
    public partial class VisafeService : PreshutdownEnabledService
    {
        private Thread _thread = null;
        private bool _threadRunning;
        private EventLog _eventLog;
        private DNSServer _dnsServer;
        private ServiceController _sc;

        public VisafeService()
        {
            _eventLog = new EventLog("Application");
            _eventLog.Source = "VisafeService";
            _dnsServer = new DNSServer();

            InitializeComponent();

            _sc = new ServiceController(Constants.SERVICE_NAME);
        }

        protected override void OnStart(string[] args)
        {
            _thread = new Thread(new ThreadStart(Run));
            _thread.Start();
            _threadRunning = true;
        }

        protected override void OnStop()
        {
            _threadRunning = false;
            _dnsServer.Exit();
        }

        protected override void OnShutdown()
        {
            _threadRunning = false;
            _dnsServer.Exit();
        }

        private void Run()
        {
            while (_threadRunning == true)
            {
                PipeSecurity ps = new PipeSecurity();
                ps.AddAccessRule(new PipeAccessRule("Users", PipeAccessRights.ReadWrite, AccessControlType.Allow));
                ps.AddAccessRule(new PipeAccessRule(System.Security.Principal.WindowsIdentity.GetCurrent().Name, PipeAccessRights.FullControl, AccessControlType.Allow));
                ps.AddAccessRule(new PipeAccessRule("SYSTEM", PipeAccessRights.FullControl, AccessControlType.Allow));

                var pipeServer = new NamedPipeServerStream(Constants.VISAFE_SERVICE_PIPE, 
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
                        //the first signal that the service receives when the application starts to check if it has started
                        if (signalData["signal"] == "check_start")
                        {
                            if (_sc.Status == ServiceControllerStatus.Running)
                            {
                                ss.WriteString(Constants.STARTED_NOTI_STRING);
                            }
                        }
                        else if (signalData["signal"] == "start")
                        {
                            _dnsServer.Start();
                            ss.WriteString("received"); //recevie signal from client and return it back to confirm
                        }
                        else if (signalData["signal"] == "stop")
                        {
                            _dnsServer.Stop();
                            ss.WriteString("received"); //recevie signal from client and return it back to confirm
                        }
                        else if (signalData["signal"] == "exit")
                        {
                            _dnsServer.Exit();
                            ss.WriteString("received"); //recevie signal from client and return it back to confirm
                        }
                        else if (signalData["signal"] == "update")
                        {
                            ss.WriteString("received");
                        }
                        else if (signalData["signal"] == "get_id")
                        {
                            string user_id = Helper.GetID();
                            ss.WriteString(user_id);
                        } 
                        //else if (signalData["signal"] == "open_form")
                        //{
                        //    ss.WriteString("recevied");
                        //    string signalDataString = "signal << open_form";
                        //    Helper.SendSignal(signalDataString);
                        //}
                    } 
                    catch (ThreadAbortException ex)
                    {
                        //do nothing
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
