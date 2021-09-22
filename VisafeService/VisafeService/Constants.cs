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
    }
}
