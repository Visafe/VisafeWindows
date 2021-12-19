using System;
using RestSharp;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using RestSharp.Extensions.MonoHttp;
using System.Net;
using static Visafe.Helper;
using System.IO;

namespace Visafe
{
    public partial class Form2 : Form
    {
        private DeviceInfoObtainer deviceInfoObtainer = new DeviceInfoObtainer();

        private string _inviteUrl;
        
        public string InviteUrl { get; set; }

        public Form2()
        {
            InitializeComponent();

            _inviteUrl = this.deviceInfoObtainer.GetUrl();

            text_url.Text = this._inviteUrl;
        }

        /// <summary>
        /// Make a request to add the device to a group.
        /// </summary>
        /// <returns></returns>
        private bool SendInvitingUrl()
        {
            string url = text_url.Text;

            status_label.Text = "Đang lưu...";

            string Pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=. ]+$";
            Regex Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if ((url == "") || !url.Contains("http"))
            {
                status_label.Text = "";
                MessageBox.Show("URL không hợp lệ", Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (!Rgx.IsMatch(url))
            {
                status_label.Text = "";
                MessageBox.Show("URL không hợp lệ", Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                string realUrl = Helper.UrlLengthen(url);

                string deviceId;
                string groupId;
                string groupName;
                string deviceName;
                string macAddress;
                string ipAddress;
                string deviceType;
                string deviceOwner;
                string deviceDetail;

                //deviceId = this.deviceInfoObtainer.GetID();

                string signalDataString = "signal << get_id;";
                var tempId = Helper.SendSignal(signalDataString);

                if (tempId != null)
                {
                    deviceId = tempId;
                }
                else
                {
                    deviceId = "";
                }

                try
                {
                    Uri myUri = new Uri(realUrl);
                    groupId = HttpUtility.ParseQueryString(myUri.Query).Get("groupId");
                    groupName = HttpUtility.ParseQueryString(myUri.Query).Get("groupName");
                }
                catch
                {
                    groupId = "";
                    groupName = "";
                }


                try
                {
                    deviceName = System.Environment.GetEnvironmentVariable("COMPUTERNAME");
                }
                catch
                {
                    deviceName = "unknown";
                }

                try
                {
                    macAddress = DeviceInfoObtainer.GetMac();
                }
                catch
                {
                    macAddress = "unknown";
                }

                try
                {
                    ipAddress = DeviceInfoObtainer.GetIpAddress();
                }
                catch
                {
                    ipAddress = "unknown";
                }

                deviceType = "Windows";

                try
                {
                    deviceOwner = Environment.UserName;
                }
                catch
                {
                    deviceOwner = "unknown";
                }

                try
                {
                    deviceDetail = Environment.OSVersion.ToString();
                }
                catch
                {
                    deviceDetail = "unknown";
                }

                //establish secure channel
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var client = new RestClient(realUrl);
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;

                try
                {
                    request.AddJsonBody(new
                    {
                        deviceId = deviceId,
                        groupName = groupName,
                        groupId = groupId,
                        deviceName = deviceName,
                        macAddress = macAddress,
                        ipAddress = ipAddress,
                        deviceType = deviceType,
                        deviceOwner = deviceOwner,
                        deviceDetail = deviceDetail,
                    });

                    var response = client.Execute(request);

                    JoiningGroupResp respContent = JsonConvert.DeserializeObject<JoiningGroupResp>(response.Content);

                    //var responseString = response.Content.ReadAsStringAsync();

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        status_label.Text = "";
                        MessageBox.Show(respContent.local_msg, Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    else
                    {
                        string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                        string visafeFolder = Path.Combine(appDataFolder, "Visafe");

                        if (Directory.Exists(visafeFolder) == false)
                        {
                            Directory.CreateDirectory(visafeFolder);
                        }

                        string urlConfig = Path.Combine(visafeFolder, Constant.URL_CONFIG_FILE);

                        if (!File.Exists(urlConfig))
                        {
                            File.Create(urlConfig).Dispose();
                        }

                        FileInfo fi = new FileInfo(urlConfig);
                        using (TextWriter writer = new StreamWriter(fi.Open(FileMode.Truncate)))
                        {
                            writer.WriteLine(url);
                            writer.Close();
                        }

                        status_label.Text = "";
                        MessageBox.Show(Constant.SAVING_SUCCESS_MSG, Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true;
                    }
                }
                catch (Exception e)
                {
                    status_label.Text = "";
                    Console.WriteLine(e.Message);
                    MessageBox.Show(Constant.SAVING_ERROR_MSG + "\n\nURL không hợp lệ hoặc thiết bị đã tham gia vào nhóm hoặc thiết bị đang ở nhóm khác", Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            bool sent = SendInvitingUrl();

            this.Close();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
