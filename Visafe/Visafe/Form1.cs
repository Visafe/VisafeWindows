using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Net;
using Visafe.Properties;
using System.IO.Pipes;
using System.Text.RegularExpressions;
using RestSharp.Extensions.MonoHttp;
using System.Web.Script.Serialization;


namespace Visafe
{
    public partial class Form1 : Form
    {
        private bool isOn = false; //default state is off
        private EventLog _eventLog;

        private DeviceInfoObtainer deviceInfoObtainer = new DeviceInfoObtainer();

        public Form1()
        {
            _eventLog = new EventLog("Application");
            _eventLog.Source = "Visafe";


            InitializeComponent();
        }

        public void SendInvitingUrl()
        {
            string url = text_url.Text;
            //if (url != this.deviceInfoObtainer.GetUrl())
            //{

            status_label.Text = "Đang lưu...";

            string Pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=. ]+$";
            Regex Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if ((url == "") || !url.Contains("http"))
            {
                //notifyIcon1.ShowBalloonTip(Constant.NOTI_TIME, Constant.NOTI_TITLE, "Error", ToolTipIcon.Error);
                status_label.Text = "";
                MessageBox.Show("URL không hợp lệ", Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!Rgx.IsMatch(url))
            {
                status_label.Text = "";
                MessageBox.Show("URL không hợp lệ", Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string deviceId;
                string groupId;
                string groupName;
                string deviceName;
                string macAddress;
                string ipAddress;
                string deviceType;
                string deviceOwner;
                string deviceDetail;

                deviceId = this.deviceInfoObtainer.GetID();

                try
                {
                    Uri myUri = new Uri(url);
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
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";

                try
                {
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        string json = new JavaScriptSerializer().Serialize(new
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

                        streamWriter.Write(json);
                    }
              
                    var response = (HttpWebResponse)request.GetResponse();

                    //var responseString = response.Content.ReadAsStringAsync();

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        //notifyIcon1.ShowBalloonTip(Constant.NOTI_TIME, Constant.NOTI_TITLE, "Không thể lưu URL", ToolTipIcon.Error);
                        status_label.Text = "";
                        MessageBox.Show(Constant.SAVING_ERROR_MSG, Constant.NOTI_TITLE);
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

                        //notifyIcon1.ShowBalloonTip(Constant.NOTI_TIME, Constant.NOTI_TITLE, Constant.SAVING_SUCCESS_MSG, ToolTipIcon.Info);
                        status_label.Text = "";
                        MessageBox.Show(Constant.SAVING_SUCCESS_MSG, Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception e)
                {
                    status_label.Text = "";
                    Console.WriteLine(e.Message);
                    //notifyIcon1.ShowBalloonTip(Constant.NOTI_TIME, Constant.NOTI_TITLE, "Không thể lưu URL", ToolTipIcon.Error);
                    MessageBox.Show(Constant.SAVING_ERROR_MSG + "\n\nURL không hợp lệ hoặc thiết bị đã tham gia vào nhóm hoặc thiết bị đang ở nhóm khác", Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            //}
            //else
            //{
            //    MessageBox.Show("URL không thay đổi", Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
        }

        //function used to start service
        private void startService()
        {
            string user_id = this.deviceInfoObtainer.GetID();
            string signalDataString = "signal << start; user_id << " + user_id + ";";

            var sendResult = sendSignal(signalDataString);

            if (sendResult == null)
            {
                //push notification
                //notifyIcon1.BalloonTipIcon = ToolTipIcon.Error;
                //notifyIcon1.BalloonTipText = "Không thể khởi động Visafe";
                //notifyIcon1.BalloonTipTitle = Constant.NOTI_TITLE;
                //notifyIcon1.ShowBalloonTip(Constant.NOTI_TIME);

                //show message box
                MessageBox.Show("Không thể khởi động Visafe", Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                //push notification
                //notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                //notifyIcon1.BalloonTipText = "Visafe đã được kích hoạt";
                //notifyIcon1.BalloonTipTitle = Constant.NOTI_TITLE;
                //notifyIcon1.ShowBalloonTip(Constant.NOTI_TIME);

                //Show message box
                MessageBox.Show("Visafe đã được kích hoạt, bạn đang được bảo vệ khỏi các mối đe dọa trên không gian mạng", Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //function used to stop service
        private void stopService()
        {
            string signalDataString = "signal << stop;";

            var sendResult = sendSignal(signalDataString);

            if (sendResult == null)
            {
                //push notification
                //notifyIcon1.BalloonTipIcon = ToolTipIcon.Error;
                //notifyIcon1.BalloonTipText = "Không thể khởi động Visafe";
                //notifyIcon1.BalloonTipTitle = Constant.NOTI_TITLE;
                //notifyIcon1.ShowBalloonTip(Constant.NOTI_TIME);

                //show message box
                MessageBox.Show("Không thể khởi động Visafe", Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Disable close button
        private const int CP_DISABLE_CLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle = cp.ClassStyle | CP_DISABLE_CLOSE_BUTTON;
                return cp;
            }
        }

        //function to start visafe application
        private void Form1_Load(object sender, EventArgs e)
        {
            string user_id = this.deviceInfoObtainer.GetID();
            string invitingURL = this.deviceInfoObtainer.GetUrl();

            text_url.Text = invitingURL;

            notifyIcon1.Visible = true;
            item_turnoff.Visible = true;
            item_turnon.Visible = false;
            Hide();
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;

            //disable resizing, disable Minimize and Maximum button
            //this.FormBorderStyle = FormBorderStyle.FixedSingle;
            //this.MaximizeBox = false;
            //this.MinimizeBox = false;

            startService();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
        }

        //when click Turn on in tray icon
        //restart program and service
        private void item_turnon_Click(object sender, EventArgs e)
        {
            //start program
            notifyIcon1.Icon = Resources.turnon;

            item_turnoff.Visible = true;
            item_turnon.Visible = false;
            Hide();
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
            //start service
            startService();
        }

        // When click Turn off in tray icon
        //kill dnsproxy.exe
        //stop service
        private void item_turnoff_Click(object sender, EventArgs e)
        {
            item_turnoff.Visible = false;
            item_turnon.Visible = true;
            notifyIcon1.Icon = Resources.turnoff;
            stopService();
        }

        // When click Exit in tray icon
        //close application
        //stop service
        private void item_exit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Thiết bị của bạn có thể bị ảnh hưởng bởi tấn công mạng. \nBạn muốn tắt bảo vệ?", "Bạn đang tắt chế độ bảo vệ!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                stopService();
                Application.Exit();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void openSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            //this.TopMost = true;
            this.ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            //Hide();
            //WindowState = FormWindowState.Minimized;
            SendInvitingUrl();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Hide();
            WindowState = FormWindowState.Minimized;
        }

        private string sendSignal(string signal)
        {
            var pipeClient = new NamedPipeClientStream(".", "VisafeServicePipe", PipeDirection.InOut, PipeOptions.None);

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
                _eventLog.WriteEntry(exc.Message, EventLogEntryType.Error);
            }

            pipeClient.Close();

            return returnedSignal;
        }
    }
}
