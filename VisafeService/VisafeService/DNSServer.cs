using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Management;
using System.Net.NetworkInformation;
using System.IO;
using System.Diagnostics;

namespace VisafeService
{
    class DNSServer
    {
        public bool isOn = false; //default state is off

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

        private bool PingCheckHealth(string domain)
        {
            int threshold = 5;
            Ping myPing = new Ping();
            PingReply reply;

            for (int i = 0; i < threshold; i++)
            {
                try
                {
                    reply = myPing.Send(domain, 600);
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

            flushDNS();
        }

        public void Start()
        {
            string user_id = Helper.GetID();
            var dnsCryptProxyExecutablePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "dnsproxy.exe");

            //get the best host for DOH server from routing API
            string dohHost = Helper.GetDohHost();

            //source code dnsproxy: https://github.com/AdguardTeam/dnsproxy
            string arguments = " -u https://" + dohHost + "/dns-query/" + user_id + Constants.DNSPROXY_ARGS;

            _dnsProxyProcess.StartInfo.UseShellExecute = false;
            _dnsProxyProcess.StartInfo.FileName = dnsCryptProxyExecutablePath;
            _dnsProxyProcess.StartInfo.Arguments = arguments.ToLower();
            _dnsProxyProcess.StartInfo.CreateNoWindow = true;
            _dnsProxyProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            _dnsProxyProcess.Start();

            SetDNS(Constants.LOCAL_DNS_SERVER);

            //set state to on
            isOn = true;
        }

        public void Stop()
        {
            UnsetDNS();

            isOn = false;
        }

        public void Exit()
        {
            UnsetDNS();

            try
            {
                _dnsProxyProcess.Kill();
            }
            catch (Exception e)
            {
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
