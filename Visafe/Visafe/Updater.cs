using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using Newtonsoft.Json;
using System.Net;
using System.Diagnostics;

namespace Visafe
{
    class StableVersion
    {
        public string version { get; set; }
        public string url { get; set; }
        public string description { get; set; }
    }

    class VersionInfoResponse
    {
        public StableVersion stable { get; set; }
    }

    class Updater
    {
        private string _versionInfoUrl;
        private string _currentVersion;
        public string NewVersion = "";
        private string _newVersionUrl = "";
        public string NewVersionDescription = "";

        public Updater(string versionInfoUrl = "")
        {
            this._versionInfoUrl = versionInfoUrl;

            //string currentFolder = Directory.GetCurrentDirectory();
            string currentFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string verionFile = Path.Combine(currentFolder, Constant.VERSION_FILE_NAME);

            try
            {
                if (File.Exists(verionFile))
                {
                    string tempVer;
                    using (StreamReader sr = new StreamReader(verionFile))
                    {
                        tempVer = sr.ReadLine().Trim();
                        sr.Close();
                    }

                    if (tempVer?.Length == null)
                    {
                        _currentVersion = "";
                    }
                    else
                    {
                        _currentVersion = tempVer;
                    }
                }
                else
                {
                    _currentVersion = "";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _currentVersion = "";
            }

        }

        /// <summary>
        /// Return current version
        /// </summary>
        /// <returns></returns>
        public string GetVersion()
        {
            return _currentVersion;
        }

        /// <summary>
        /// Get the information about the lastest version of Visafe. 
        /// This function returns true if the version is different from the current version stored in the machine.
        /// </summary>
        /// <returns></returns>
        public bool CheckForUpdate()
        {
            if (this._currentVersion == "")
            {
                return false;
            }

            try
            {
                var client = new RestClient(this._versionInfoUrl);
                client.Timeout = 10000;

                var request = new RestRequest(Method.GET);
                request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                IRestResponse response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string rawResponse = response.Content;
                    VersionInfoResponse respContent = JsonConvert.DeserializeObject<VersionInfoResponse>(rawResponse);
                    _newVersionUrl = respContent.stable.url;
                    NewVersion = respContent.stable.version;
                    NewVersionDescription = respContent.stable.description;

                    if (NewVersion != _currentVersion)
                    {
                        //return upgrade(newVersionUrl, newVersion);
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return false;
        }

        /// <summary>
        /// Download the new installer and execute it to install the new version.
        /// </summary>
        /// <returns></returns>
        public bool Upgrade()
        {
            if (NewVersion == "" || _newVersionUrl == "")
            {
                return false;
            }
            else if (NewVersion == _currentVersion)
            {
                return false;
            }

            string installerPath = downloadInstaller(_newVersionUrl, NewVersion);

            if (installerPath == null)
            {
                return false;
            }

            string procesName = installerPath;
            string args = @"/SILENT";
            try
            {
                ProcessStartInfo p_info = new ProcessStartInfo();
                p_info.UseShellExecute = true;
                p_info.CreateNoWindow = false;
                p_info.WindowStyle = ProcessWindowStyle.Normal;
                p_info.Arguments = args;
                p_info.FileName = procesName;
                Process updateProcess = Process.Start(p_info);
            }
            catch (Exception exp)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Download the installer from the internet and store it in the Temp folder.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        private string downloadInstaller(string url, string version)
        {
            string fileName = String.Format("VisafeUpgrader_v{0}.exe", version);
            string fileLocation = Path.Combine(Path.GetTempPath(), fileName);

            if (File.Exists(fileLocation))
            {
                FileInfo fi = new FileInfo(fileLocation);

                //return true if the installer has been downloaded and existing for less than 1 day
                if (fi.LastWriteTime > DateTime.Now.AddDays(-1))
                {
                    return fileLocation;
                }
                else
                {
                    File.Delete(fileLocation);
                }
            }

            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.GET);
                byte[] response = client.DownloadData(request);
                File.WriteAllBytes(fileLocation, response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return fileLocation;
        }
    }
}
