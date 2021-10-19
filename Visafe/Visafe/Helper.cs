using System;
using System.Net;

namespace Visafe
{
    public static class Helper
    {
        public class JoiningGroupResp
        {
            public string status { get; set; }
            public string msg { get; set; }
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
    }
}
