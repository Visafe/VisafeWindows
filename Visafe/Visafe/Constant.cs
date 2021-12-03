using System;

namespace Visafe
{
    class Constant
    {
        public const string GET_DEVICE_ID_API = "https://app.visafe.vn/api/v1/control/gen-device-id";
        public const string VERSION_INFO_URL = "https://raw.githubusercontent.com/Visafe/VisafeWindows/main/version_info.json";
        public const string HELP_SITE_URL = "https://docs.visafe.vn/bat-dau-su-dung-visafe/cach-su-dung-visafe/huong-dan-su-dung-visafe-windows-client";
        public const string URL_CONFIG_FILE = "urlconfig.conf";
        public const string USERID_CONFIG_FILE = "userid.conf";
        public const string SETTING_MODE_FILE = "setting_mode.conf";
        public const string VERSION_FILE_NAME = "version.txt";
        public const string NOTI_TITLE = "VISAFE";
        public const int NOTI_TIME = 500;
        public const string DNS_BASE_URL = "dns.visafe.vn";
        public const string ADMIN_DASHBOARD_URL = "https://app.visafe.vn";
        public const string VISAFE_DOC_URL = "https://docs.visafe.vn";
        public const string SAVING_ERROR_MSG = "KHÔNG THỂ LƯU URL";
        public const string SAVING_SUCCESS_MSG = "Đã tham gia vào nhóm";
        public const string STARTED_NOTI_STRING = "started";
        public const string VISAFE_SERVICE_PIPE = "VisafeServicePipe";
        public const string SECURITY_MODE = "security";
        public const string FAMILY_MODE = "family";
        public const string SECURITY_PLUS_MODE = "securityplus";
        public const string CUSTOM_MODE = "custom";

        public const string ERR_START_SERVICE_MSG = "Không thể khởi động Visafe";
        public const string ERR_SAVING_CONFIG_MSG = "Không thể lưu thiết lập";

        public const int TIMEOUT = 5; // amount of time to time out (second)
    }
}
