using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Net;

namespace Visafe
{
    public static class Helper
    {
        public class JoiningGroupResp
        {
            public string status { get; set; }
            public string msg { get; set; }

            public string local_msg { get; set; }
        }

        public static string UrlLengthen(string url)
        {
            string newurl = url;

            bool redirecting = true;

            while (redirecting)
            {

                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(newurl);
                    request.AllowAutoRedirect = false;
                    request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.1.3) Gecko/20090824 Firefox/3.5.3 (.NET CLR 4.0.20506)";
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if ((int)response.StatusCode == 301 || (int)response.StatusCode == 302)
                    {
                        string uriString = response.Headers["Location"];
                        Console.WriteLine("Redirecting " + newurl + " to " + uriString + " because " + response.StatusCode);
                        newurl = uriString;
                        // and keep going
                    }
                    else
                    {
                        Console.WriteLine("Not redirecting " + url + " because " + response.StatusCode);
                        redirecting = false;
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add("url", newurl);
                    Console.WriteLine(ex); // change this to your own
                    redirecting = false;
                }
            }
            return newurl;
        }

        public static string SendSignal(string signal)
        {
            var pipeClient = new NamedPipeClientStream(".", Constant.VISAFE_SERVICE_PIPE, PipeDirection.InOut, PipeOptions.None);

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

        public static Dictionary<string, string> ParseSignalString(string signalDataString)
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
