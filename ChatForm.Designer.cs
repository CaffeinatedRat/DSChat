namespace DSChat
{
    partial class ChatForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChatForm));
            this.webChatLog = new System.Windows.Forms.WebBrowser();
            this.rtxChat = new System.Windows.Forms.RichTextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbClearChat = new System.Windows.Forms.ToolStripButton();
            this.tsbBuzz = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tsbOpacity = new System.Windows.Forms.ToolStripComboBox();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // webChatLog
            // 
            this.webChatLog.AllowNavigation = false;
            this.webChatLog.AllowWebBrowserDrop = false;
            this.webChatLog.IsWebBrowserContextMenuEnabled = false;
            this.webChatLog.Location = new System.Drawing.Point(0, 28);
            this.webChatLog.MinimumSize = new System.Drawing.Size(20, 20);
            this.webChatLog.Name = "webChatLog";
            this.webChatLog.Size = new System.Drawing.Size(576, 151);
            this.webChatLog.TabIndex = 23;
            this.webChatLog.WebBrowserShortcutsEnabled = false;
            // 
            // rtxChat
            // 
            this.rtxChat.BackColor = System.Drawing.SystemColors.Window;
            this.rtxChat.BulletIndent = 5;
            this.rtxChat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxChat.Location = new System.Drawing.Point(12, 185);
            this.rtxChat.MaxLength = 5000;
            this.rtxChat.Name = "rtxChat";
            this.rtxChat.ShowSelectionMargin = true;
            this.rtxChat.Size = new System.Drawing.Size(497, 129);
            this.rtxChat.TabIndex = 24;
            this.rtxChat.Text = "";
            this.rtxChat.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.rtxChat_KeyPress);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(515, 185);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(61, 129);
            this.btnSend.TabIndex = 26;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClearChat,
            this.tsbBuzz,
            this.toolStripLabel1,
            this.tsbOpacity});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(577, 25);
            this.toolStrip1.TabIndex = 28;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbClearChat
            // 
            this.tsbClearChat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbClearChat.Image = global::DSChat.Properties.Resources.EraserImg;
            this.tsbClearChat.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbClearChat.Name = "tsbClearChat";
            this.tsbClearChat.Size = new System.Drawing.Size(23, 22);
            this.tsbClearChat.Text = "Clear Chat";
            this.tsbClearChat.Click += new System.EventHandler(this.tsbClearChat_Click);
            // 
            // tsbBuzz
            // 
            this.tsbBuzz.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbBuzz.Image = global::DSChat.Properties.Resources.BuzzImg;
            this.tsbBuzz.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbBuzz.Name = "tsbBuzz";
            this.tsbBuzz.Size = new System.Drawing.Size(23, 22);
            this.tsbBuzz.Text = "Buzz User";
            this.tsbBuzz.Click += new System.EventHandler(this.tsbBuzz_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(48, 22);
            this.toolStripLabel1.Text = "Opacity";
            // 
            // tsbOpacity
            // 
            this.tsbOpacity.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.tsbOpacity.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.tsbOpacity.Items.AddRange(new object[] {
            "100%",
            "90%",
            "80%",
            "70%",
            "60%",
            "50%",
            "40%",
            "30%",
            "20%",
            "10%"});
            this.tsbOpacity.MaxDropDownItems = 10;
            this.tsbOpacity.Name = "tsbOpacity";
            this.tsbOpacity.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.tsbOpacity.Size = new System.Drawing.Size(75, 25);
            this.tsbOpacity.Text = "100%";
            this.tsbOpacity.ToolTipText = "Change the Opacity of the active window.";
            this.tsbOpacity.TextChanged += new System.EventHandler(this.tsbOpacity_TextChanged);
            // 
            // ChatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 318);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.webChatLog);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.rtxChat);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ChatForm";
            this.Text = "ChatForm";
            this.Activated += new System.EventHandler(this.ChatForm_OnActivated);
            this.Deactivate += new System.EventHandler(this.ChatForm_OnDeactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChatForm_FormClosing);
            this.Enter += new System.EventHandler(this.ChatForm_OnEnter);
            this.Leave += new System.EventHandler(this.ChatForm_OnLeave);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser webChatLog;
        private System.Windows.Forms.RichTextBox rtxChat;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbClearChat;
        private System.Windows.Forms.ToolStripButton tsbBuzz;
        private System.Windows.Forms.ToolStripComboBox tsbOpacity;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;

    }
}