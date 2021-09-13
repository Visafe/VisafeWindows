using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using Newtonsoft.Json;
using System.Net;
using System.Diagnostics;

namespace VisafeService
{
    class StableVersion
    {
        public string version { get; set; }
        public string url { get; set; }
    }
    class VersionInfoResponse {
        public StableVersion stable { get; set; }
    }

    class Updater
    {
        private string _versionInfoUrl;
        private string _currentVersion;

        public Updater(string versionInfoUrl = "") {
            this._versionInfoUrl = versionInfoUrl;

            string currentFolder = Directory.GetCurrentDirectory();
            string verionFile = Path.Combine(currentFolder, Constants.VERSION_FILE_NAME);

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
            } catch (Exception e)
            {
                Console.WriteLine(e);
                _currentVersion = "";
            }
           
        }

        public string GetVersion()
        {
            return _currentVersion;
        }

        public bool CheckNow()
        {
            string newVersion = "";
            string newVersionUrl = "";
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

                IRestResponse response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string rawResponse = response.Content;
                    VersionInfoResponse respContent =  JsonConvert.DeserializeObject<VersionInfoResponse>(rawResponse);
                    newVersionUrl = respContent.stable.url;
                    newVersion = respContent.stable.version;

                    if (newVersion != _currentVersion) {
                        return upgrade(newVersionUrl, newVersion);
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

        private bool upgrade(string url, string version)
        {
            (string installerPath, bool succeeded) = downloadInstaller(url, version);

            if (succeeded != true)
            {
                return succeeded;
            }

            string command = @"/S /C " + installerPath;
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo("cmd.exe", command)

                };
                process.Start();

            }
            catch (Exception exp)
            {
                return false;
            }
            return true;
        }

        private (string installerPath, bool succeeded) downloadInstaller(string url, string version)
        {
            string fileName = String.Format("VisafeUpgrader_v{0}.exe", version);
            string fileLocation = Path.Combine(Path.GetTempPath(), fileName);

            if (File.Exists(fileLocation)) {
                FileInfo fi = new FileInfo(fileLocation);

                //return true if the installer has been downloaded and existing for less than 1 day
                if (fi.LastWriteTime > DateTime.Now.AddDays(-1))
                {
                    return (fileLocation, true);
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

            return (null, false);
        }
    }
}
