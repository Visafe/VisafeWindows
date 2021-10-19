using System;

namespace Visafe
{
    class Constant
    {
        public const string GET_DEVICE_ID_API = "https://app.visafe.vn/api/v1/control/gen-device-id";
        public const string VERSION_INFO_URL = "https://raw.githubusercontent.com/VisafeTeam/VisafeWindows/main/version_info.json";
        public const string URL_CONFIG_FILE = "urlconfig.conf";
        public const string USERID_CONFIG_FILE = "userid.conf";
        public const string VERSION_FILE_NAME = "version.txt";
        public const string NOTI_TITLE = "VISAFE";
        public const int NOTI_TIME = 500;
        public const string DNS_BASE_URL = "dns.visafe.vn";
        public const string SAVING_ERROR_MSG = "KHÔNG THỂ LƯU URL";
        public const string SAVING_SUCCESS_MSG = "Đã lưu URL";
    }
}
