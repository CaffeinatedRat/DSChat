namespace DSChat
{
    partial class DSChatForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DSChatForm));
            this.grpStatus = new System.Windows.Forms.GroupBox();
            this.lvRegisteredUsers = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.grpSecurity = new System.Windows.Forms.GroupBox();
            this.txtPassphrase = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.grpHostingInfo = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtListeningPort = new System.Windows.Forms.TextBox();
            this.cbHosting = new System.Windows.Forms.CheckBox();
            this.grpConnectionInfo = new System.Windows.Forms.GroupBox();
            this.lblServer = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtHostPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblServerIP = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblMyAddress = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtStatusLog = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearStatusLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showChatFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notificationIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextNotificationMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiNodeType = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cmitemVisibility = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmitemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grpStatus.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.grpSecurity.SuspendLayout();
            this.grpHostingInfo.SuspendLayout();
            this.grpConnectionInfo.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.contextNotificationMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpStatus
            // 
            this.grpStatus.Controls.Add(this.lvRegisteredUsers);
            this.grpStatus.Location = new System.Drawing.Point(316, 35);
            this.grpStatus.Name = "grpStatus";
            this.grpStatus.Size = new System.Drawing.Size(161, 218);
            this.grpStatus.TabIndex = 0;
            this.grpStatus.TabStop = false;
            this.grpStatus.Text = "Registered Users";
            // 
            // lvRegisteredUsers
            // 
            this.lvRegisteredUsers.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.lvRegisteredUsers.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.lvRegisteredUsers.AllowColumnReorder = true;
            this.lvRegisteredUsers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvRegisteredUsers.HoverSelection = true;
            this.lvRegisteredUsers.LargeImageList = this.imageList1;
            this.lvRegisteredUsers.Location = new System.Drawing.Point(6, 19);
            this.lvRegisteredUsers.MultiSelect = false;
            this.lvRegisteredUsers.Name = "lvRegisteredUsers";
            this.lvRegisteredUsers.ShowGroups = false;
            this.lvRegisteredUsers.Size = new System.Drawing.Size(149, 192);
            this.lvRegisteredUsers.SmallImageList = this.imageList1;
            this.lvRegisteredUsers.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvRegisteredUsers.TabIndex = 13;
            this.lvRegisteredUsers.UseCompatibleStateImageBehavior = false;
            this.lvRegisteredUsers.View = System.Windows.Forms.View.List;
            this.lvRegisteredUsers.Click += new System.EventHandler(this.lvRegisteredUsers_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "MAX.ico");
            this.imageList1.Images.SetKeyName(1, "msg.ico");
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnExit);
            this.groupBox4.Controls.Add(this.grpSecurity);
            this.groupBox4.Controls.Add(this.btnConnect);
            this.groupBox4.Controls.Add(this.grpHostingInfo);
            this.groupBox4.Controls.Add(this.grpConnectionInfo);
            this.groupBox4.Location = new System.Drawing.Point(12, 35);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(298, 218);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Setup";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(201, 188);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(88, 23);
            this.btnExit.TabIndex = 12;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_OnClick);
            // 
            // grpSecurity
            // 
            this.grpSecurity.Controls.Add(this.txtPassphrase);
            this.grpSecurity.Controls.Add(this.label7);
            this.grpSecurity.Location = new System.Drawing.Point(6, 135);
            this.grpSecurity.Name = "grpSecurity";
            this.grpSecurity.Size = new System.Drawing.Size(283, 47);
            this.grpSecurity.TabIndex = 8;
            this.grpSecurity.TabStop = false;
            this.grpSecurity.Text = "Security";
            // 
            // txtPassphrase
            // 
            this.txtPassphrase.Location = new System.Drawing.Point(72, 14);
            this.txtPassphrase.Name = "txtPassphrase";
            this.txtPassphrase.PasswordChar = '*';
            this.txtPassphrase.Size = new System.Drawing.Size(205, 20);
            this.txtPassphrase.TabIndex = 9;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 17);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Passphrase:";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(6, 188);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(102, 23);
            this.btnConnect.TabIndex = 10;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_OnClick);
            // 
            // grpHostingInfo
            // 
            this.grpHostingInfo.Controls.Add(this.label6);
            this.grpHostingInfo.Controls.Add(this.txtListeningPort);
            this.grpHostingInfo.Controls.Add(this.cbHosting);
            this.grpHostingInfo.Location = new System.Drawing.Point(6, 19);
            this.grpHostingInfo.Name = "grpHostingInfo";
            this.grpHostingInfo.Size = new System.Drawing.Size(283, 39);
            this.grpHostingInfo.TabIndex = 1;
            this.grpHostingInfo.TabStop = false;
            this.grpHostingInfo.Text = "Hosting Info";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(186, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Port:";
            // 
            // txtListeningPort
            // 
            this.txtListeningPort.Location = new System.Drawing.Point(217, 13);
            this.txtListeningPort.Name = "txtListeningPort";
            this.txtListeningPort.Size = new System.Drawing.Size(60, 20);
            this.txtListeningPort.TabIndex = 3;
            this.txtListeningPort.Text = "9000";
            // 
            // cbHosting
            // 
            this.cbHosting.AutoSize = true;
            this.cbHosting.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbHosting.Location = new System.Drawing.Point(9, 15);
            this.cbHosting.Name = "cbHosting";
            this.cbHosting.Size = new System.Drawing.Size(68, 17);
            this.cbHosting.TabIndex = 2;
            this.cbHosting.Text = "Hosting?";
            this.cbHosting.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbHosting.UseVisualStyleBackColor = true;
            this.cbHosting.Click += new System.EventHandler(this.cbHosting_OnClick);
            // 
            // grpConnectionInfo
            // 
            this.grpConnectionInfo.Controls.Add(this.lblServer);
            this.grpConnectionInfo.Controls.Add(this.txtServer);
            this.grpConnectionInfo.Controls.Add(this.label1);
            this.grpConnectionInfo.Controls.Add(this.txtHostPort);
            this.grpConnectionInfo.Controls.Add(this.label2);
            this.grpConnectionInfo.Controls.Add(this.txtName);
            this.grpConnectionInfo.Location = new System.Drawing.Point(6, 64);
            this.grpConnectionInfo.Name = "grpConnectionInfo";
            this.grpConnectionInfo.Size = new System.Drawing.Size(283, 65);
            this.grpConnectionInfo.TabIndex = 4;
            this.grpConnectionInfo.TabStop = false;
            this.grpConnectionInfo.Text = "Connection Info";
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(22, 40);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(32, 13);
            this.lblServer.TabIndex = 4;
            this.lblServer.Text = "Host:";
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(61, 37);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(120, 20);
            this.txtServer.TabIndex = 6;
            this.txtServer.Text = "HostGoesHere";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(186, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Port:";
            // 
            // txtHostPort
            // 
            this.txtHostPort.Location = new System.Drawing.Point(217, 37);
            this.txtHostPort.Name = "txtHostPort";
            this.txtHostPort.Size = new System.Drawing.Size(60, 20);
            this.txtHostPort.TabIndex = 7;
            this.txtHostPort.Text = "9000";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(17, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(61, 13);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(216, 20);
            this.txtName.TabIndex = 5;
            this.txtName.Text = "YourNameHere";
            // 
            // lblServerIP
            // 
            this.lblServerIP.AutoSize = true;
            this.lblServerIP.Location = new System.Drawing.Point(361, 16);
            this.lblServerIP.Name = "lblServerIP";
            this.lblServerIP.Size = new System.Drawing.Size(40, 13);
            this.lblServerIP.TabIndex = 7;
            this.lblServerIP.Text = "0.0.0.0";
            this.lblServerIP.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(301, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Server IP:";
            this.label5.Visible = false;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(49, 16);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(79, 13);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "Not Connected";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Status:";
            // 
            // lblMyAddress
            // 
            this.lblMyAddress.AutoSize = true;
            this.lblMyAddress.Location = new System.Drawing.Point(192, 16);
            this.lblMyAddress.Name = "lblMyAddress";
            this.lblMyAddress.Size = new System.Drawing.Size(40, 13);
            this.lblMyAddress.TabIndex = 3;
            this.lblMyAddress.Text = "0.0.0.0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(153, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "MyIP:";
            // 
            // txtStatusLog
            // 
            this.txtStatusLog.Location = new System.Drawing.Point(6, 32);
            this.txtStatusLog.Multiline = true;
            this.txtStatusLog.Name = "txtStatusLog";
            this.txtStatusLog.ReadOnly = true;
            this.txtStatusLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatusLog.Size = new System.Drawing.Size(453, 98);
            this.txtStatusLog.TabIndex = 0;
            this.txtStatusLog.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblServerIP);
            this.groupBox2.Controls.Add(this.txtStatusLog);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.lblStatus);
            this.groupBox2.Controls.Add(this.lblMyAddress);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(12, 259);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(465, 136);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Status";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.debugToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(484, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.btnExit_OnClick);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearStatusLogToolStripMenuItem,
            this.connectToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.editToolStripMenuItem.Text = "Action";
            // 
            // clearStatusLogToolStripMenuItem
            // 
            this.clearStatusLogToolStripMenuItem.Name = "clearStatusLogToolStripMenuItem";
            this.clearStatusLogToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.clearStatusLogToolStripMenuItem.Text = "Clear Status Log";
            this.clearStatusLogToolStripMenuItem.Click += new System.EventHandler(this.clearStatusLogToolStripMenuItem_Click);
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.connectToolStripMenuItem.Text = "Connect";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showChatFormToolStripMenuItem});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.debugToolStripMenuItem.Text = "Debug";
            this.debugToolStripMenuItem.Visible = false;
            // 
            // showChatFormToolStripMenuItem
            // 
            this.showChatFormToolStripMenuItem.Name = "showChatFormToolStripMenuItem";
            this.showChatFormToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.showChatFormToolStripMenuItem.Text = "Show ChatForm";
            this.showChatFormToolStripMenuItem.Click += new System.EventHandler(this.showChatFormToolStripMenuItem_Click);
            // 
            // notificationIcon
            // 
            this.notificationIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notificationIcon.BalloonTipText = "The program has been minimized.\r\nDouble-click to open.";
            this.notificationIcon.BalloonTipTitle = "DSChat";
            this.notificationIcon.ContextMenuStrip = this.contextNotificationMenu;
            this.notificationIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notificationIcon.Icon")));
            this.notificationIcon.Text = "DSChat";
            this.notificationIcon.Visible = true;
            this.notificationIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextNotificationMenu
            // 
            this.contextNotificationMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmiNodeType,
            this.toolStripSeparator2,
            this.cmitemVisibility,
            this.toolStripSeparator1,
            this.cmitemExit});
            this.contextNotificationMenu.Name = "contextNotificationMenu";
            this.contextNotificationMenu.Size = new System.Drawing.Size(142, 82);
            // 
            // cmiNodeType
            // 
            this.cmiNodeType.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.cmiNodeType.Name = "cmiNodeType";
            this.cmiNodeType.Size = new System.Drawing.Size(141, 22);
            this.cmiNodeType.Text = "DSChat";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(138, 6);
            // 
            // cmitemVisibility
            // 
            this.cmitemVisibility.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmitemVisibility.Name = "cmitemVisibility";
            this.cmitemVisibility.Size = new System.Drawing.Size(141, 22);
            this.cmitemVisibility.Text = "Hide DSChat";
            this.cmitemVisibility.Click += new System.EventHandler(this.cmitemVisibility_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(138, 6);
            // 
            // cmitemExit
            // 
            this.cmitemExit.Name = "cmitemExit";
            this.cmitemExit.Size = new System.Drawing.Size(141, 22);
            this.cmitemExit.Text = "Exit";
            this.cmitemExit.Click += new System.EventHandler(this.cmitemExit_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // DSChatForm
            // 
            this.AcceptButton = this.btnConnect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 405);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.grpStatus);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "DSChatForm";
            this.ShowInTaskbar = false;
            this.Text = "DSChat v1.1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DSChat_FormClosing);
            this.Resize += new System.EventHandler(this.DSChatForm_Resize);
            this.grpStatus.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.grpSecurity.ResumeLayout(false);
            this.grpSecurity.PerformLayout();
            this.grpHostingInfo.ResumeLayout(false);
            this.grpHostingInfo.PerformLayout();
            this.grpConnectionInfo.ResumeLayout(false);
            this.grpConnectionInfo.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextNotificationMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpStatus;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtPassphrase;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.CheckBox cbHosting;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtHostPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblServerIP;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblMyAddress;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtStatusLog;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.GroupBox grpSecurity;
        private System.Windows.Forms.GroupBox grpHostingInfo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtListeningPort;
        private System.Windows.Forms.GroupBox grpConnectionInfo;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.ListView lvRegisteredUsers;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.NotifyIcon notificationIcon;
        private System.Windows.Forms.ContextMenuStrip contextNotificationMenu;
        private System.Windows.Forms.ToolStripMenuItem cmitemVisibility;
        private System.Windows.Forms.ToolStripMenuItem cmitemExit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem cmiNodeType;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showChatFormToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearStatusLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;

    }
}

