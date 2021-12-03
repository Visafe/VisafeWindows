using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace VisafeService
{
    class DeviceIdResponse
    {
        public string deviceId { get; set; }
    }

    class CheckDeviceResponse
    {
        public int status_code { get; set; }
        public string groupId { get; set; }
        public string groupName { get; set; }
        public string groupOwner { get; set; }
        public int numberDevice { get; set; }
    }

    class RoutingResponse
    {
        public string hostname { get; set; }
        public string ip { get; set; }
    }

    public static class Helper
    {
        public static bool ExecuteCmdCommand(string cmdArgs)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo("cmd.exe", cmdArgs)

                };
                process.Start();

                process.WaitForExit();
                return true;
            }
            catch (Exception exp)
            {
                Console.WriteLine(String.Format("Failed to execute command:'{0}' . Error:{1}", cmdArgs, exp.Message), EventLogEntryType.Error);
                return false;
            }
        }

        public static bool RunExecutable(string exeLocation)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo(exeLocation)

                };
                process.Start();

                return true;
            }
            catch (Exception exp)
            {
                Console.WriteLine(String.Format("Failed to start process:'{0}' . Error:{1}", exeLocation, exp.Message), EventLogEntryType.Error);
                return false;
            }
        }

        //if ID is avalable, it will not generate anymore. if not, it will generate new userID and save in file config 
        public static string GetID()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string visafeFolder = Path.Combine(appDataFolder, "Visafe");

            if (Directory.Exists(visafeFolder) == false)
            {
                Directory.CreateDirectory(visafeFolder);
            }

            string userIdConfig = Path.Combine(visafeFolder, Constants.USERID_CONFIG_FILE);

            string giatri;

            if (File.Exists(userIdConfig))
            {
                using (StreamReader sr = new StreamReader(userIdConfig))
                {
                    giatri = sr.ReadLine();
                    sr.Close();
                }

                if (giatri?.Length == null || giatri == "")
                {
                    giatri = GetDeviceId();
                    using (StreamWriter sw = new StreamWriter(userIdConfig))
                    {
                        sw.WriteLine(giatri);
                        sw.Close();
                    }
                    return giatri;
                }
                else
                {
                    return giatri;
                }
            }
            else
            {
                File.Create(userIdConfig).Dispose();
                giatri = GetDeviceId();
                using (StreamWriter sw = new StreamWriter(userIdConfig))
                {
                    sw.WriteLine(giatri);
                    sw.Close();
                }
            }

            return giatri;
        }

        private static string GetDeviceId()
        {
            string deviceId = random_id();

            try
            {
                var client = new RestClient(Constants.GET_DEVICE_ID_API);
                client.Timeout = -1;

                var request = new RestRequest(Method.GET);
                request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };

                IRestResponse response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string rawResponse = response.Content;
                    DeviceIdResponse responseContent = JsonConvert.DeserializeObject<DeviceIdResponse>(rawResponse);
                    deviceId = responseContent.deviceId;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return deviceId;
            }

            return deviceId;
        }

        //generate random string for id user
        public static string random_id()
        {
            int size = 12;
            char[] chars =
                "abcdefghijklmnopqrstuvwxyz1234567890".ToCharArray();
            byte[] data = new byte[size];

            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            try
            {
                crypto.GetBytes(data);
            }
            catch (Exception)
            {
                return null;
            }

            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public static bool IsPrivateIp(string ipAddress)
        {
            int[] ipParts = ipAddress.Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(s => int.Parse(s)).ToArray();
            // in private ip range
            if (ipParts[0] == 10 ||
                (ipParts[0] == 192 && ipParts[1] == 168) ||
                (ipParts[0] == 172 && (ipParts[1] >= 16 && ipParts[1] <= 31)))
            {
                return true;
            }

            // IP Address is probably public.
            // This doesn't catch some VPN ranges like OpenVPN and Hamachi.
            return false;
        }
        
        public static void SaveUserDnsServers(IPAddressCollection dnsServers)
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string visafeFolder = Path.Combine(appDataFolder, "Visafe");

            if (Directory.Exists(visafeFolder) == false)
            {
                Directory.CreateDirectory(visafeFolder);
            }

            string userDnsFile = Path.Combine(visafeFolder, Constants.USER_DNS_FILE);

            if (!File.Exists(userDnsFile))
            {
                File.Create(userDnsFile).Dispose();
                
                using (StreamWriter sw = new StreamWriter(userDnsFile))
                {
                    foreach (IPAddress dnsServerAdd in dnsServers)
                    {
                        string dnsServerString = dnsServerAdd.ToString().Trim();
                        sw.WriteLine(dnsServerString);
                    }
                    
                    sw.Close();
                }
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(userDnsFile))
                {
                    foreach (IPAddress dnsServerAdd in dnsServers)
                    {
                        string dnsServerString = dnsServerAdd.ToString().Trim();
                        sw.WriteLine(dnsServerString);
                    }

                    sw.Close();
                }
            }
        }

        public static List<IPAddress> GetUserDnsServers()
        {
            var result = new List<IPAddress>();

            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string visafeFolder = Path.Combine(appDataFolder, "Visafe");
            string userDnsFile = Path.Combine(visafeFolder, Constants.USER_DNS_FILE);

            if (File.Exists(userDnsFile))
            {
                using (StreamReader sr = new StreamReader(userDnsFile))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        try
                        {
                            IPAddress tempIpAdd = IPAddress.Parse(s.Trim());
                            result.Add(tempIpAdd);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Cannot read and parse IP address " + s, e);
                        }
                    }

                    sr.Close();
                }
            }

            return result;
        }
        
        public static string GetDohHost()
        {
            string dohHost = Constants.DEFAULT_DOH_HOST;

            //try
            //{
            //    var client = new RestClient(Constants.ROUTING_API);
            //    client.Timeout = 10000;

            //    var request = new RestRequest(Method.GET);
            //    request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };

            //    IRestResponse response = client.Execute(request);

            //    if (response.StatusCode == HttpStatusCode.OK)
            //    {
            //        string rawResponse = response.Content;
            //        RoutingResponse responseContent = JsonConvert.DeserializeObject<RoutingResponse>(rawResponse);
            //        dohHost = responseContent.hostname;
            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //    return dohHost;
            //}

            return dohHost;
        }

        public static string SendSignal(string signal)
        {
            var pipeClient = new NamedPipeClientStream(".", Constants.VISAFE_GUI_PIPE, PipeDirection.InOut, PipeOptions.None);

            string returnedSignal = null;
            pipeClient.Connect();

            var ss = new StreamString(pipeClient);

            try
            {
                ss.WriteString(signal);
                returnedSignal = ss.ReadString();
            }
            catch (Exception exc)
            {
                //_eventLog.WriteEntry(exc.Message, EventLogEntryType.Error);
            }
            finally
            {
                pipeClient.Close();
            }

            return returnedSignal;
        }

        public static string CheckDevice(string deviceId)
        {
            try
            {
                var client = new RestClient(Constants.CHECK_DEVICE_API);
                client.Timeout = 15000;

                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { deviceId = deviceId });

                request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };

                IRestResponse response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string rawResponse = response.Content;
                    CheckDeviceResponse responseContent = JsonConvert.DeserializeObject<CheckDeviceResponse>(rawResponse);
                    int statusCode = responseContent.status_code;

                    if (statusCode == 1)
                    {
                        return responseContent.groupId;
                    }
                    else
                    {
                        return "";
                    }
                } 
                else
                {
                    return "";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }
    }
}
