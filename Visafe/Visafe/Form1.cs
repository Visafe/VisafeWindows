using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Net;
using Visafe.Properties;
using System.IO.Pipes;
using System.Text.RegularExpressions;
using RestSharp.Extensions.MonoHttp;
using static Visafe.Helper;
using RestSharp;
using Newtonsoft.Json;
using System.Threading;
using System.Security.AccessControl;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Visafe
{
    public partial class Form1 : Form
    {
        private EventLog _eventLog;

        private Updater _updater;

        private string _currentMode;

        private Form2 _custSettForm;

        public Form1()
        {
            _eventLog = new EventLog("Application");
            _eventLog.Source = "Visafe";

            _updater = new Updater(Constant.VERSION_INFO_URL);

            _currentMode = Helper.LoadCurrentMode();

            _custSettForm = new Form2();
            _custSettForm.Hide();

            InitializeComponent();
        }

        //function to start visafe application
        private void Form1_Load(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;

            //bool started = startService(_currentMode);

            var task = Task.Run(() => startService(_currentMode));
            if (task.Wait(TimeSpan.FromMinutes(5)))
            {
                notifyIcon1.Icon = Resources.turnon;
            }
            else
            {
                MessageBox.Show(Constant.ERR_START_SERVICE_MSG, Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            setCheckBoxOnStart(_currentMode);

            notifyIcon1.Visible = true;
            item_turnoff.Visible = true;
            item_turnon.Visible = false;
            Hide();
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;

            checkForUpdate();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
        }

        //function used to start service
        private bool startService(string mode)
        {
            string signalDataString = "signal << check_start";
            string sendResult = Helper.SendSignal(signalDataString);
            int elapsed = 0;
            while (sendResult != Constant.STARTED_NOTI_STRING) {
                sendResult = Helper.SendSignal(signalDataString);
                Thread.Sleep(2000);
                elapsed += 2000;
                if (elapsed > 8000)
                {
                    MessageBox.Show(Constant.ERR_START_SERVICE_MSG, Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (mode != Constant.CUSTOM_MODE)
            {
                signalDataString = "signal << " + mode + ";";

                sendResult = Helper.SendSignal(signalDataString);

                if (sendResult == null || sendResult == "error")
                {
                    MessageBox.Show(Constant.ERR_START_SERVICE_MSG, Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else
                {
                    return true;
                }
            } 
            else
            {
                signalDataString = "signal << " + mode + ";";

                sendResult = Helper.SendSignal(signalDataString);

                if (sendResult == null)
                {
                    MessageBox.Show(Constant.ERR_START_SERVICE_MSG, Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else if (sendResult == "no_group")
                {
                    signalDataString = "signal << " + Constant.SECURITY_MODE + ";";

                    sendResult = Helper.SendSignal(signalDataString);

                    if (sendResult == null || sendResult == "error")
                    {
                        MessageBox.Show(Constant.ERR_START_SERVICE_MSG, Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        stopService();
                        return false;
                    }
                    else
                    {
                        setCheckBoxOnStart(Constant.SECURITY_MODE);
                        MessageBox.Show("Thiết bị chưa tham gia nhóm, vui lòng nhấn Cài đặt để thiết lập. \n\nThiết lập sẽ được chuyển về chế độ An toàn thông tin (mặc định).", 
                            Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        //function used to stop service
        private void stopService()
        {
            string signalDataString = "signal << stop;";

            var sendResult = Helper.SendSignal(signalDataString);

            if (sendResult == null)
            {
                //show message box
                MessageBox.Show("Không thể tắt Visafe", Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exitService()
        {
            string signalDataString = "signal << exit;";

            //var sendResult = Helper.SendSignal(signalDataString);
            var sendResult = Helper.SendSignal(signalDataString);

            if (sendResult == null)
            {
                //show message box
                MessageBox.Show("Không thể tắt Visafe", Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkForUpdate()
        {
            bool newVersion = _updater.CheckForUpdate();

            if (newVersion == true)
            {
                string message = "Visafe có bản cập nhật mới, bạn có muốn cài đặt?";

                if (_updater.NewVersionDescription != "")
                {
                    message = message + "\n\n" + "Những sự thay đổi ở bản cập nhật:";
                    message = message + "\n" + _updater.NewVersionDescription;
                }

                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(message, Constant.NOTI_TITLE, buttons);
                if (result == DialogResult.Yes)
                {
                    string signalDataString1 = "signal << update;";

                    var sendResult1 = Helper.SendSignal(signalDataString1);

                    if (sendResult1 == null)
                    {
                        //show message box
                        MessageBox.Show("Không thể cập nhật Visafe", Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    _updater.Upgrade();
                }
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

        //show the form if the notify icon is clicked by the left mouse
        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                FormDisplay();
            }
        }

        //when click Turn on in tray icon
        //restart program and service
        private void item_turnon_Click(object sender, EventArgs e)
        {
            //start service
            //bool started = startService(this._currentMode);

            var task = Task.Run(() => startService(_currentMode));
            if (task.Wait(TimeSpan.FromSeconds(15)))
            {
                //start program
                notifyIcon1.Icon = Resources.turnon;

                item_turnoff.Visible = true;
                item_turnon.Visible = false;
                ShowInTaskbar = false;
            }
            else
            {
                MessageBox.Show(Constant.ERR_START_SERVICE_MSG, Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //WindowState = FormWindowState.Minimized;
            Hide();
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

        //When click Exit in tray icon
        //close application
        //stop service
        private void item_exit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Thiết bị của bạn có thể bị ảnh hưởng bởi tấn công mạng. \nBạn muốn tắt bảo vệ?", "Bạn đang tắt chế độ bảo vệ!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                var task = Task.Run(() => exitService());
                if (task.Wait(TimeSpan.FromSeconds(15)))
                {
                    Application.Exit();
                }
                else
                {
                    Application.Exit();
                }
            }
        }

        private void openSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            this.ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            button_save.Text = "Đang lưu...";
            this.Refresh();

            //bool started = startService(this._currentMode);
            bool started = false;

            var task = Task.Run(() => startService(_currentMode));
            if (task.Wait(TimeSpan.FromSeconds(15)))
            {
                started = task.Result;
            }
            else
            {
                MessageBox.Show(Constant.ERR_SAVING_CONFIG_MSG, Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Application.Exit();
            }

            if (started == true)
            {
                SaveCurrentMode(_currentMode);
                MessageBox.Show("Đã lưu thiết lập", Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            button_save.Text = "Lưu";
            this.Refresh();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Hide();
            WindowState = FormWindowState.Minimized;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Constant.ADMIN_DASHBOARD_URL);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Constant.VISAFE_DOC_URL);
        }

        private void FormDisplay()
        {
            setCheckBox(this._currentMode);
            this.Show();
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
        }

        private void securityCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //this._currentMode = Constant.SECURITY_MODE;
            CheckBox cb = (CheckBox)sender;
            if (cb.CheckState == CheckState.Checked)
            {
                setCheckBox(Constant.SECURITY_MODE);
            }
        }

        private void familyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //this._currentMode = Constant.FAMILY_MODE;
            CheckBox cb = (CheckBox)sender;
            if (cb.CheckState == CheckState.Checked)
            {
                setCheckBox(Constant.FAMILY_MODE);
            }
        }

        private void securityPlusCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //this._currentMode = Constant.SECURITY_PLUS_MODE;
            CheckBox cb = (CheckBox)sender;
            if (cb.CheckState == CheckState.Checked)
            {
                setCheckBox(Constant.SECURITY_PLUS_MODE);
            }
        }

        private void customCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //this._currentMode = Constant.CUSTOM_MODE;
            CheckBox cb = (CheckBox)sender;

            if (cb.CheckState == CheckState.Checked)
            {
                setCheckBox(Constant.CUSTOM_MODE);
            }
        }

        private void customSettingButton_Click(object sender, EventArgs e)
        {
            Form2 settingForm = new Form2();
            settingForm.ShowDialog();
        }

        private void setCheckBox(string mode)
        {
            switch (mode) {
                case Constant.SECURITY_MODE:
                    _currentMode = Constant.SECURITY_MODE;
                    this.familyCheckBox.Checked = false;
                    this.securityPlusCheckBox.Checked = false;
                    this.customCheckBox.Checked = false;
                    break;
                case Constant.FAMILY_MODE:
                    _currentMode = Constant.FAMILY_MODE;
                    this.securityCheckBox.Checked = false;
                    this.securityPlusCheckBox.Checked = false;
                    this.customCheckBox.Checked = false;
                    break;
                case Constant.SECURITY_PLUS_MODE:
                    _currentMode = Constant.SECURITY_PLUS_MODE;
                    this.securityCheckBox.Checked = false;
                    this.familyCheckBox.Checked = false;
                    this.customCheckBox.Checked = false;
                    break;
                case Constant.CUSTOM_MODE:
                    _currentMode = Constant.CUSTOM_MODE;
                    this.securityCheckBox.Checked = false;
                    this.familyCheckBox.Checked = false;
                    this.securityPlusCheckBox.Checked = false;
                    break;
            }
        }

        private void setCheckBoxOnStart(string mode)
        {
            switch (mode)
            {
                case Constant.SECURITY_MODE:
                    this._currentMode = Constant.SECURITY_MODE;
                    this.securityCheckBox.Checked = true;
                    this.familyCheckBox.Checked = false;
                    this.securityPlusCheckBox.Checked = false;
                    this.customCheckBox.Checked = false;
                    break;
                case Constant.FAMILY_MODE:
                    this._currentMode = Constant.FAMILY_MODE;
                    this.securityCheckBox.Checked = false;
                    this.familyCheckBox.Checked = true; 
                    this.securityPlusCheckBox.Checked = false;
                    this.customCheckBox.Checked = false;
                    break;
                case Constant.SECURITY_PLUS_MODE:
                    this._currentMode = Constant.SECURITY_PLUS_MODE;
                    this.securityCheckBox.Checked = false;
                    this.familyCheckBox.Checked = false;
                    this.securityPlusCheckBox.Checked = true;
                    this.customCheckBox.Checked = false;
                    break;
                case Constant.CUSTOM_MODE:
                    this._currentMode = Constant.CUSTOM_MODE;
                    this.securityCheckBox.Checked = false;
                    this.familyCheckBox.Checked = false;
                    this.securityPlusCheckBox.Checked = false;
                    this.customCheckBox.Checked = true;
                    break;
            }
        }

        private void helpLink_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo helpSite = new ProcessStartInfo(Constant.HELP_SITE_URL);
                Process.Start(helpSite);
            }
            catch
            {
                MessageBox.Show("Không thể đi đến URL", Constant.NOTI_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
    }
}
