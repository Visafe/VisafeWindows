using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Management;
using System.Net.NetworkInformation;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

namespace VisafeService
{
    class DNSServer
    {
        private bool isOn = false; //default state is off
        const string DNS_BASE_URL = "dns.visafe.vn";

        private Process _dnsProxyProcess;
        private NetworkInterface _currInterface;
        private IPAddressCollection _currDnsServers;

        public DNSServer() {
            _dnsProxyProcess = new Process();
            _currInterface = GetActiveEthernetOrWifiNetworkInterface();
            _currDnsServers = _currInterface.GetIPProperties().DnsAddresses;
            Helper.SaveUserDnsServers(_currDnsServers);
        }

        static void flushDNS()
        {
            string flushDnsCmd = @"/C ipconfig /flushdns";
            Helper.ExecuteCmdCommand(flushDnsCmd);
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

        public void SetDNS(string DnsString)
        {
            flushDNS();

            string[] Dns = { DnsString };
            var CurrentInterface = GetActiveEthernetOrWifiNetworkInterface();
            this._currInterface = CurrentInterface;
            _currDnsServers = CurrentInterface.GetIPProperties().DnsAddresses;
            Helper.SaveUserDnsServers(_currDnsServers);

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

        public void UnsetDNS()
        {
            //var CurrentInterface = GetActiveEthernetOrWifiNetworkInterface();
            //if (CurrentInterface == null) return;

            var CurrentInterface = _currInterface;

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
                            try
                            {
                                objdns["DNSServerSearchOrder"] = null;

                                var userDnsServers = Helper.GetUserDnsServers();
                                if (userDnsServers.Count > 0)
                                {
                                    List<string> tmpDnsServers = new List<string>();
                                    foreach (IPAddress ipAdd in userDnsServers)
                                    {
                                        string tempIpStr = ipAdd.ToString();

                                        if ((Helper.IsPrivateIp(tempIpStr) == false) && (IPAddress.IsLoopback(ipAdd) == false)) {
                                            tmpDnsServers.Add(tempIpStr);
                                        }
                                    }

                                    if (tmpDnsServers.Count > 0)
                                    {
                                        string[] dnsServers = tmpDnsServers.ToArray();

                                        objdns["DNSServerSearchOrder"] = dnsServers;
                                    }
                                }

                                objMO.InvokeMethod("SetDNSServerSearchOrder", objdns, null);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Failed to unset DNS", e);
                            }
                            
                        }

                        return;
                    }
                }
            }
        }

        public void Start()
        {
            string user_id = Helper.GetID();
            var dnsCryptProxyExecutablePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "dnsproxy.exe");

            if (PingCheckHealth() == true)
            {
                //thangnn2 - set DNS to loopback so that everything will be directed to this service
                SetDNS("127.0.0.2");
            }

            //commandline to direct to our dns service : dnsproxy.exe -u  https://dns.visafe.vn/dns-query/ + userid -b 1.1.1.1:53"
            //source code dnsproxy: https://github.com/AdguardTeam/dnsproxy
            string arguments = " -u https://" + DNS_BASE_URL + "/dns-query/" + user_id + " -l 127.0.0.2 -b 1.1.1.1:53";

            _dnsProxyProcess.StartInfo.UseShellExecute = false;
            _dnsProxyProcess.StartInfo.FileName = dnsCryptProxyExecutablePath;
            _dnsProxyProcess.StartInfo.Arguments = arguments.ToLower();
            _dnsProxyProcess.StartInfo.CreateNoWindow = true;
            _dnsProxyProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            _dnsProxyProcess.Start();

            //int pid = this._dnsProxyProcess.Id;

            //set state to on
            isOn = true;
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

            if (isOn == true)
            {
                _dnsProxyProcess.Kill();
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
