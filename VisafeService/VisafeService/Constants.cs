using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisafeService
{
    public static class Constants
    {
        public const string VERSION_INFO_URL = "https://raw.githubusercontent.com/VisafeTeam/VisafeWindows/main/version_info.json";
        public const string VERSION_FILE_NAME = "version.txt";
        public const string INSTALL_PATH = "C:\\Program Files (x86)\\VisafeWindows";
        public const string USERID_CONFIG_FILE = "userid.conf";
        public const string USER_DNS_FILE = "userdns.conf";
        public const string GET_DEVICE_ID_API = "https://app.visafe.vn/api/v1/control/gen-device-id";
        public const string ROUTING_API = "https://app.visafe.vn/api/v1/routing";
        public const string DEFAULT_DOH_HOST = "dns.visafe.vn";
        public const string LOCAL_DNS_SERVER = "127.0.0.2";
        public const string DNSPROXY_ARGS = " -l " + LOCAL_DNS_SERVER + " -b 103.192.236.123:53 -b 103.192.236.124:53 -b 117.122.125.106:53 -b 8.8.8.8:53 -f 203.119.73.106:53 -f 117.122.125.106:53 -f 8.8.8.8:53";
    }
}
