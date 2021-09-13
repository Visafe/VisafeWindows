using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Management;
using RestSharp;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace Visafe
{

    public class DeviceInfoObtainer
    {
        public DeviceInfoObtainer() {}

        public static NetworkInterface GetActiveEthernetOrWifiNetworkInterface()
        {
            var Nic = NetworkInterface
                .GetAllNetworkInterfaces()
                .FirstOrDefault(
                    a => a.OperationalStatus == OperationalStatus.Up &&
                    (a.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || a.NetworkInterfaceType == NetworkInterfaceType.Ethernet) &&
                    a.GetIPProperties().GatewayAddresses.Any(g => g.Address.AddressFamily.ToString() == "InterNetwork")
                );

            return Nic;
        }

        public static string GetMac()
        {
            var CurrentInterface = GetActiveEthernetOrWifiNetworkInterface();
            if (CurrentInterface == null) return "";

            string mac = CurrentInterface.GetPhysicalAddress().ToString();

            return mac;
        }

        public static string GetIpAddress()
        {
            string IPAddress = string.Empty;
            IPHostEntry Host = default(IPHostEntry);
            string Hostname = null;
            Hostname = System.Environment.MachineName;
            Host = Dns.GetHostEntry(Hostname);
            foreach (IPAddress IP in Host.AddressList)
            {
                if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    IPAddress = Convert.ToString(IP);
                }
            }

            return IPAddress;
        }
        public string GetUrl()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string visafeFolder = Path.Combine(appDataFolder, "Visafe");

            if (Directory.Exists(visafeFolder) == false)
            {
                Directory.CreateDirectory(visafeFolder);
            }

            string urlConfig = Path.Combine(visafeFolder, Constant.URL_CONFIG_FILE);
            //string urlConfig = Path.Combine(Directory.GetCurrentDirectory(), URL_CONFIG_FILE);

            string giatri = "";

            if (File.Exists(urlConfig))
            {
                using (StreamReader sr = new StreamReader(urlConfig))
                {
                    giatri = sr.ReadLine();
                    sr.Close();
                }

                if (giatri?.Length == null)
                {
                    giatri = "";
                }
            }

            return giatri;
        }

        //if ID is avalable, it will not generate anymore. if not, it will generate new userID and save in file config 
        public string GetID()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string visafeFolder = Path.Combine(appDataFolder, "Visafe");

            if (Directory.Exists(visafeFolder) == false)
            {
                Directory.CreateDirectory(visafeFolder);
            }

            string userIdConfig = Path.Combine(visafeFolder, Constant.USERID_CONFIG_FILE);

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

        //generate random string for id user
        public string random_id()
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
            catch (Exception e)
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

        public string GetDeviceId()
        {
            string deviceId = random_id();

            try
            {
                var client = new RestClient(Constant.GET_DEVICE_ID_API);
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
    }
}
