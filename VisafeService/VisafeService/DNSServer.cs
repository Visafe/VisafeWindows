using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Management;
using System.Net.NetworkInformation;
using System.IO;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using System.Diagnostics;

namespace VisafeService
{
    class DNSServer
    {
        private bool isOn = false; //default state is off
        const string DNS_BASE_URL = "dns.visafe.vn";

        private Process _dnsProxyProcess = new Process();

        static void flushDNS()
        {
            string flushDnsCmd = @"/C ipconfig /all";
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo("cmd.exe", flushDnsCmd)

                };
                process.Start();

                process.WaitForExit();
                Console.WriteLine(String.Format("Successfully Flushed DNS:'{0}'", flushDnsCmd), EventLogEntryType.Information);

            }
            catch (Exception exp)
            {
                Console.WriteLine(String.Format("Failed to Flush DNS:'{0}' . Error:{1}", flushDnsCmd, exp.Message), EventLogEntryType.Error);
            }
        }

        private bool PingCheckHealth()
        {
            int threshold = 5;
            Ping myPing = new Ping();
            PingReply reply;

            for (int i = 0; i < threshold; i++)
            {
                try
                {
                    reply = myPing.Send(DNS_BASE_URL, 600);
                    if (reply != null)
                    {
                        return true;
                    }
                }
                catch
                {
                    continue;
                }
            }

            return false;
        }

        public static NetworkInterface GetActiveEthernetOrWifiNetworkInterface()
        {
            //coditions:
            //not loopback interface
            //not tunnel interface
            //status is up
            //network interface is wireless interface or ethernet
            //gateway address is not 0.0.0.0 (undefined)
            var Nic = NetworkInterface
                .GetAllNetworkInterfaces()
                .FirstOrDefault(
                    a => a.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    a.NetworkInterfaceType != NetworkInterfaceType.Tunnel &&
                    a.OperationalStatus == OperationalStatus.Up &&
                    (a.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || a.NetworkInterfaceType == NetworkInterfaceType.Ethernet) &&
                    a.GetIPProperties().GatewayAddresses.Any(g => g.Address.ToString() != "0.0.0.0")
                );

            var temp = NetworkInterface
                .GetAllNetworkInterfaces()
                .FirstOrDefault();

            return Nic;
        }

        public static void SetDNS(string DnsString)
        {
            flushDNS();

            string[] Dns = { DnsString };
            var CurrentInterface = GetActiveEthernetOrWifiNetworkInterface();
            if (CurrentInterface == null) return;

            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            foreach (ManagementObject objMO in objMOC)
            {
                if ((bool)objMO["IPEnabled"])
                {
                    if (objMO["Caption"].ToString().Contains(CurrentInterface.Description))
                    {
                        ManagementBaseObject objdns = objMO.GetMethodParameters("SetDNSServerSearchOrder");
                        if (objdns != null)
                        {
                            objdns["DNSServerSearchOrder"] = Dns;
                            objMO.InvokeMethod("SetDNSServerSearchOrder", objdns, null);
                        }
                    }
                }
            }
        }

        public static void UnsetDNS()
        {
            var CurrentInterface = GetActiveEthernetOrWifiNetworkInterface();
            if (CurrentInterface == null) return;

            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            foreach (ManagementObject objMO in objMOC)
            {
                if ((bool)objMO["IPEnabled"])
                {
                    if (objMO["Caption"].ToString().Contains(CurrentInterface.Description))
                    {
                        ManagementBaseObject objdns = objMO.GetMethodParameters("SetDNSServerSearchOrder");
                        if (objdns != null)
                        {
                            objdns["DNSServerSearchOrder"] = null;
                            objMO.InvokeMethod("SetDNSServerSearchOrder", objdns, null);
                        }
                    }
                }
            }
        }

        public void Start(string user_id)
        {
            var dnsCryptProxyExecutablePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "dnsproxy.exe");

            if (PingCheckHealth() == true)
            {
                //thangnn2 - set DNS to loopback so that everything will be directed to this service
                SetDNS("127.0.0.2");
            }

            //commandline to direct to our dns service : dnsproxy.exe -u  https://dns.visafe.vn/dns-query/ + userid -b 1.1.1.1:53"
            //source code dnsproxy: https://github.com/AdguardTeam/dnsproxy
            //string arguments = " -u https://dns.google/dns-query" + " -b 1.1.1.1:53";
            string arguments = " -u https://" + DNS_BASE_URL + "/dns-query/" + user_id + " -l 127.0.0.2 -b 1.1.1.1:53";
            //this._dnsProxyProcess = new Process();
            this._dnsProxyProcess.StartInfo.UseShellExecute = false;
            this._dnsProxyProcess.StartInfo.FileName = dnsCryptProxyExecutablePath;
            this._dnsProxyProcess.StartInfo.Arguments = arguments.ToLower();
            this._dnsProxyProcess.StartInfo.CreateNoWindow = true;
            this._dnsProxyProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            this._dnsProxyProcess.Start();

            int pid = this._dnsProxyProcess.Id;

            //set state to on
            isOn = true;

        }

        public void Stop()
        {
            UnsetDNS();
            //set state to on

            if (isOn == true)
            {
                this._dnsProxyProcess.Kill();
            }

            isOn = false;
        }

        private Dictionary<string, string> ParseDataString(string dataString)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            string[] dataArr = dataString.Split(';');

            foreach (string field in dataArr)
            {
                field.Trim();   //remove spaces at the beginning and ending of the string

                string[] delimeters = { "<<" };
                string[] fieldArr = field.Split(delimeters, StringSplitOptions.RemoveEmptyEntries);

                result[fieldArr[0].Trim()] = fieldArr[1].Trim();
            }

            return result;
        }
    }

}
