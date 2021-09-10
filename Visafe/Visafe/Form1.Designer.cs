namespace Visafe
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.context = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.item_turnon = new System.Windows.Forms.ToolStripMenuItem();
            this.item_turnoff = new System.Windows.Forms.ToolStripMenuItem();
            this.item_exit = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.text_url = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button_save = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.status_label = new System.Windows.Forms.Label();
            this.context.SuspendLayout();
            this.SuspendLayout();
            // 
            // context
            // 
            this.context.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.context.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openSettingsToolStripMenuItem,
            this.item_turnon,
            this.item_turnoff,
            this.item_exit});
            this.context.Name = "context";
            this.context.Size = new System.Drawing.Size(181, 114);
            // 
            // openSettingsToolStripMenuItem
            // 
            this.openSettingsToolStripMenuItem.Name = "openSettingsToolStripMenuItem";
            this.openSettingsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openSettingsToolStripMenuItem.Text = "Mở cài đặt";
            this.openSettingsToolStripMenuItem.Click += new System.EventHandler(this.openSettingsToolStripMenuItem_Click);
            // 
            // item_turnon
            // 
            this.item_turnon.Name = "item_turnon";
            this.item_turnon.Size = new System.Drawing.Size(180, 22);
            this.item_turnon.Text = "Bật";
            this.item_turnon.Click += new System.EventHandler(this.item_turnon_Click);
            // 
            // item_turnoff
            // 
            this.item_turnoff.Name = "item_turnoff";
            this.item_turnoff.Size = new System.Drawing.Size(180, 22);
            this.item_turnoff.Text = "Tắt tạm thời";
            this.item_turnoff.Click += new System.EventHandler(this.item_turnoff_Click);
            // 
            // item_exit
            // 
            this.item_exit.Name = "item_exit";
            this.item_exit.Size = new System.Drawing.Size(180, 22);
            this.item_exit.Text = "Thoát";
            this.item_exit.Click += new System.EventHandler(this.item_exit_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.context;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Visafe";
            this.notifyIcon1.Visible = true;
            // 
            // text_url
            // 
            this.text_url.Location = new System.Drawing.Point(9, 37);
            this.text_url.Margin = new System.Windows.Forms.Padding(2);
            this.text_url.Name = "text_url";
            this.text_url.Size = new System.Drawing.Size(477, 20);
            this.text_url.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Link tham gia nhóm (Invite link)";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(357, 72);
            this.button_save.Margin = new System.Windows.Forms.Padding(2);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(56, 29);
            this.button_save.TabIndex = 3;
            this.button_save.Text = "Lưu";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(430, 72);
            this.button_cancel.Margin = new System.Windows.Forms.Padding(2);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(56, 29);
            this.button_cancel.TabIndex = 4;
            this.button_cancel.Text = "Đóng";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // status_label
            // 
            this.status_label.AutoSize = true;
            this.status_label.ForeColor = System.Drawing.Color.Red;
            this.status_label.Location = new System.Drawing.Point(12, 72);
            this.status_label.Name = "status_label";
            this.status_label.Size = new System.Drawing.Size(0, 13);
            this.status_label.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 112);
            this.Controls.Add(this.status_label);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.text_url);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cấu hình Visafe";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.context.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip context;
        private System.Windows.Forms.ToolStripMenuItem item_turnon;
        private System.Windows.Forms.ToolStripMenuItem item_turnoff;
        private System.Windows.Forms.ToolStripMenuItem item_exit;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.TextBox text_url;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.ToolStripMenuItem openSettingsToolStripMenuItem;
        private System.Windows.Forms.Label status_label;
    }
}

