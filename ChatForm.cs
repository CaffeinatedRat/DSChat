using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

///////////////////////////////
///        CUSTOM           ///
///////////////////////////////
using NodeSystem;
using Common;
using System.Threading;
using System.Runtime.InteropServices;

namespace DSChat
{
    public partial class ChatForm : Form
    {
        #region DataTypes

        /// <summary>
        /// Manages a simple structure for user message.
        /// </summary>
        internal class UserMessage
        {
            public string UserId { get; set; }
            public DateTime msgTime { get; set; }
            public string Msg { get; set; }
            public bool IsMe { get; set; }

            public UserMessage(string UserId, DateTime dt, string Msg, bool isMe)
            {
                this.UserId = UserId;
                this.msgTime = dt;
                this.Msg = Msg;
                this.IsMe = isMe;
            }
        }

        #endregion DataTypes


        #region Delegates

        /// <summary>
        /// Notifies the listener that the opacity has changed.
        /// </summary>
        /// <param name="opacity">
        /// The value of the opacity.  Domain [0.0, 1.0].
        /// </param>
        public delegate void OnOpacityChangedEventDelegate(ChatForm caller, double opacity);

        /// <summary>
        /// A simple event that occurs when the user has noticed the event.
        /// </summary>
        public delegate void OnMessageNoticedEventDelegate();

        #endregion Delegates

        #region Constants

        /// <summary>
        /// Provides a timeout value for the mutex, which is set to 5 seconds.
        /// </summary>
        private const int MUTEX_TIMEOUT = 5000;
          
        #endregion Constants

        #region MemberVars

        /// <summary>
        /// Determine when to close the form.  By default this value is false.
        /// </summary>
        private bool m_bTerminateForm = false;

        /// <summary>
        /// Maintains a colleciton of user messages that are displayed to the webbrowser.
        /// </summary>
        private List<UserMessage> m_ChatLog = new List<UserMessage>();

        /// <summary>
        /// Mutex used to protect the chat log data.
        /// </summary>
        private Mutex m_mutexChatLog = new Mutex(false, "ChatLogMutex");

        /// <summary>
        /// Controls the in focus opacity of the form.
        /// </summary>
        private double m_InFocusOpacity = 1.0;

        #endregion MemberVars

        #region Properties

        /// Last Modified: 9/21/10
        /// <summary>
        /// Maintains the UserId of the individual "friend" that this user is chatting with.
        /// </summary>
        public string FriendsUserId { get; set; }

        /// Last Modified: 9/21/10
        /// <summary>
        /// Maintains the UserId of this user.
        /// </summary>
        public string YourUserId { get; set; }

        /// Last Modified: 9/21/10
        /// <summary>
        /// Maintains the current child node used to communicate with the host.
        /// </summary>
        public ChildNode Node { get; set; }

        /// Last Modified: 10/31/10
        /// <summary>
        /// Controls the in focus opacity of the form.
        /// </summary>
        public double InFocusOpacity
        {
            get
            {
                return m_InFocusOpacity;
            }
            set
            {
                m_InFocusOpacity = value;
                tsbOpacity.Text = (m_InFocusOpacity * 100.0).ToString() + "%";
            }
        }

        /// Last Modified: 10/31/10
        /// <summary>
        /// A callback that is triggered when the user has noticed the message's arrival.
        /// </summary>
        public OnMessageNoticedEventDelegate MessageNoticedEventCallback { private get; set; }

        /// Last Modified: 10/31/10
        /// <summary>
        /// A callback that is triggered when the opacity has changed.
        /// </summary>
        public OnOpacityChangedEventDelegate OpacityChangedEventCallback { private get; set; }

        #endregion Properties

        #region Methods

        /// Last Modified: 9/21/10
        /// <summary>
        /// Initializes the object.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public ChatForm()
        {
            InitializeComponent();

            //rtxChat.DetectUrls = true;
            //rtxChat.Rtf = @"{\rtf1\ansi This is in \b bold\b0.}";

            //Assign the default properties.
            FriendsUserId = "";
            YourUserId = "";
            Node = null;

            //Update the caption of the form.
            this.Text = "No User";

            //Nullify
            MessageNoticedEventCallback = null;
        }

        /// Last Modified: 9/21/10
        /// <summary>
        /// A customized constructor that allows for instantiation of the forms primary properties.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="friendsUserId">
        /// The UserId of the individual "friend" that this user is chatting with.
        /// </param>
        /// <param name="currentUserId">
        /// The UserId of this user.
        /// </param>
        /// <param name="cn">
        /// The current child node used to communicate with the host.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public ChatForm(string friendsUserId, string currentUserId, ChildNode cn)
            : this()
        {
            //Assign the properties.
            FriendsUserId = friendsUserId;
            YourUserId = currentUserId;
            Node = cn;

            //Update the caption of the form.
            this.Text = FriendsUserId;
        }

        /// Last Modified: 9/22/10
        /// <summary>
        /// Forces the window to close permanently rather than just hiding it.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public new void Close()
        {
            m_bTerminateForm = true;
            base.Close();
        }

        /// Last Modified: 10/24/10
        /// <summary>
        /// Purges the chat log.
        /// NOTE: Thread-safe.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// -----------------------------------------------------
        /// POSTCONDITIONS: The chat log will be purged.
        /// -----------------------------------------------------
        private void ClearChatLog()
        {
            //Write lock.
            if (!m_mutexChatLog.WaitOne(MUTEX_TIMEOUT))
                return;

            m_ChatLog = new List<UserMessage>();

            //Do not update
            m_mutexChatLog.ReleaseMutex();
        }

        /// Last Modified: 9/26/10
        /// <summary>
        /// Attempts to write to the chat log with supplied messenger and message, and
        ///  provides an option to clear the log with the next message.
        /// NOTE: Thread-safe.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="sMessenger">
        /// The entityId of the messenger.
        /// </param>
        /// <param name="message">
        /// The message to write.
        /// </param>
        /// <param name="bClearChatLog">
        /// Clears the log if true.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: If successful the chatlog will be updated with the new info.
        /// -----------------------------------------------------
        /// <returns>
        /// Returns true if the chat log was successfully written to.
        /// </returns>
        private bool WriteToChatLog(string sMessenger, string message, bool bClearChatLog)
        {
            //Write lock.
            if (!m_mutexChatLog.WaitOne(MUTEX_TIMEOUT))
                return false;

            if (bClearChatLog)
                m_ChatLog = new List<UserMessage>();

            //Need synchronization here to protect against race conditions.
            m_ChatLog.Add(new UserMessage(sMessenger, DateTime.Now, message, (sMessenger == YourUserId)));

            //Do not update
            m_mutexChatLog.ReleaseMutex();

            return true;
        }

        /// Last Modified: 9/26/10
        /// <summary>
        /// Overridden method that accepts a chat list and uses it as the foundation of chat. This method is normally used to initialize a chatform with a chat that has
        ///  taken place offline.
        /// TimeComplexity[All cases]: O(n) -- where n is the size of the chatlist.
        /// NOTE: Thread-safe.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="sMessenger">
        /// The entityId of the messenger.
        /// </param>
        /// <param name="message">
        /// The message to write.
        /// </param>
        /// <param name="bClearChatLog">
        /// Clears the log if true.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: If successful the chatlog will be updated with the new info.
        /// -----------------------------------------------------
        /// <returns>
        /// Returns true if the chat log was successfully written to.
        /// </returns>
        private bool WriteToChatLog(string sMessenger, List<string> chatlist)
        {
            //Write lock.
            if (!m_mutexChatLog.WaitOne(MUTEX_TIMEOUT))
                return false;

            //Always instantiate.
            m_ChatLog = new List<UserMessage>();

            //Need synchronization here to protect against race conditions.
            foreach(string s in chatlist)
                m_ChatLog.Add(new UserMessage(sMessenger, DateTime.Now, s, (sMessenger == YourUserId)));

            //Do not update
            m_mutexChatLog.ReleaseMutex();

            return true;
        }

        /// Last Modified: 9/26/10
        /// <summary>
        /// Attempts to write to the chat log with supplied messenger and message.
        /// NOTE: Thread-safe.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="sMessenger">
        /// The entityId of the messenger.
        /// </param>
        /// <param name="message">
        /// The message to write.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: If successful the chatlog will be updated with the new info.
        /// -----------------------------------------------------
        /// <returns>
        /// Returns true if the chat log was successfully written to.
        /// </returns>
        private bool WriteToChatLog(string sMessenger, string message)
        {
            return WriteToChatLog(sMessenger, message, false);
        }

        #region Invocation_CrossThreadAccess

        /// Last Modified: 9/22/10
        /// <summary>
        /// Safely refreshes and syncs the chatlog with the webbrowser used to display the chat log information.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: If successful the chatlog will be updated on screen.
        /// -----------------------------------------------------
        /// Return Value:
        private void RefreshChatLog()
        {
            if (this.webChatLog.InvokeRequired)
            {
                this.webChatLog.BeginInvoke(
                    new MethodInvoker(
                        delegate() { RefreshChatLog(); }));
            }
            else
            {
                //Read-lock.
                m_mutexChatLog.WaitOne(MUTEX_TIMEOUT);
                
                if(webChatLog.Document == null)
                    webChatLog.DocumentText = "<HTML><BODY></BODY></HTML>";

                //Open a new document and html, body & table tags.
                webChatLog.Document.OpenNew(true);
                webChatLog.Document.Write("<HTML><BODY style=\"padding:0px;margins:0px;\"><table border=\"0\" width=\"100%\" style=\"font-size:0.8em;\" cellspacing=\"0\" cellpadding=\"0\">");

                //Refresh the webbrowser with all info in the chatlog.
                //foreach (UserMessage um in m_ChatLog)
                //Make the most recent entry the top.
                for(int x = m_ChatLog.Count - 1; x >= 0; x--)
                {
                    webChatLog.Document.Write("<tr><td width=\"40%\" align=\"left\" valign=\"top\"><b>");
                    webChatLog.Document.Write("<font color=\"" + (m_ChatLog[x].IsMe ? "#000000" : "#0000FF") + "\">");
                    webChatLog.Document.Write(m_ChatLog[x].UserId);
                    webChatLog.Document.Write(" (");
                    webChatLog.Document.Write(m_ChatLog[x].msgTime.ToString());
                    webChatLog.Document.Write("):</font></b></td><td width=\"60%\" align=\"left\" valign=\"top\">");
                    webChatLog.Document.Write(m_ChatLog[x].Msg + "<br/>");
                    webChatLog.Document.Write("</td></tr>");
                }

                //Close the table, body & html tags.
                webChatLog.Document.Write("</table></BODY></HTML>");

                m_mutexChatLog.ReleaseMutex();

                //Automatically scroll to the bottom of the window.
                //webChatLog.Document.Window.ScrollTo(0, webChatLog.Document.Window.Size.Height);

                //Reset focus to the textbox.
                //rtxChat.Focus();
            
            }   //END OF if (this.webChatLog.InvokeRequired)...
        }

        /// Last Modified: 10/26/10
        /// <summary>
        /// Safely sets the icon for the form & the notification area.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="ico">
        /// An icon to assign.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: The form & notification area will be updated with the new icon.
        /// -----------------------------------------------------
        /// Return Value:
        public void SafelySetIcon(Icon ico)
        {
            //Call across threads.
            if (this.InvokeRequired)
            {
                this.BeginInvoke(
                    new MethodInvoker(
                        delegate() { SafelySetIcon(ico); }));
            }
            else
            {
                //Red-Max is angry and notifies users they are disconnected.
                this.Icon = ico;
            }
        }

        #endregion Invocation_CrossThreadAccess

        #endregion Methods

        #region Events

        #region MainFormEvents

        /// Last Modified: 9/22/10
        /// <summary>
        /// This event is triggered when ever the form is closed, usually via the controlbox.
        /// In the case of the Chatform, the form needs to be hidden so that the chat can continue
        ///  even if the user closes the chat window.  He or she can then reopen that window by clicking
        ///  on the user in the DSChat form.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        private void ChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Hide the form until the window is forced closed.
            if (!m_bTerminateForm)
            {
                this.Hide();
                e.Cancel = true;
            }
        }

        /// Last Modified: 10/24/10
        /// <summary>
        /// Increases the opacity of the form when it has focus.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: The form will be opaque.
        /// -----------------------------------------------------
        /// Return Value:
        private void ChatForm_OnActivated(object sender, EventArgs e)
        {
            this.Opacity = m_InFocusOpacity;

            //Neutral Max means everything is okay.
            SafelySetIcon(DSChat.Properties.Resources.MAX);
            if (MessageNoticedEventCallback != null) MessageNoticedEventCallback();
        }

        /// Last Modified: 10/24/10
        /// <summary>
        /// Decreases the opacity of the form when it has lost focus.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: The form will be translucent.
        /// -----------------------------------------------------
        /// Return Value:
        private void ChatForm_OnDeactivate(object sender, EventArgs e)
        {
            //Always 20% of the InFocusOpacity.
            //this.Opacity = (m_InFocusOpacity * 0.20);

            //Neutral Max means everything is okay.
            SafelySetIcon(DSChat.Properties.Resources.MAX);
            if (MessageNoticedEventCallback != null) MessageNoticedEventCallback();
        }


        private void ChatForm_OnEnter(object sender, EventArgs e)
        {
            //Neutral Max means everything is okay.
            SafelySetIcon(DSChat.Properties.Resources.MAX);
            if (MessageNoticedEventCallback != null) MessageNoticedEventCallback();
        }

        private void ChatForm_OnLeave(object sender, EventArgs e)
        {
            if (MessageNoticedEventCallback != null) MessageNoticedEventCallback();
        }


        #endregion MainFormEvents

        /// Last Modified: 9/22/10
        /// <summary>
        /// This event occurs when the user clicks on the send button, which will
        ///  send the message in the chat window to the other user as well as reflect that
        ///  message in the chat log.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        private void btnSend_Click(object sender, EventArgs e)
        {
            //Write to the chat log & update the display.
            WriteToChatLog(YourUserId, rtxChat.Text);
            RefreshChatLog();

            //Send the message & update the chat log.
            if(Node != null)
                Node.SendMsg(FriendsUserId, rtxChat.Text);

            //Reset the text
            rtxChat.Text = "";
        }

        /// Last Modified: 9/22/10
        /// <summary>
        /// Mimics the send button click when the user presses enter.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        private void rtxChat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x0D)
                btnSend.PerformClick();
        }

        #region ToolbarEvents

        /// Last Modified: 10/24/10
        /// <summary>
        /// Clears the chat log when clicked.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: The chat log will be empty.
        /// -----------------------------------------------------
        /// Return Value:
        private void tsbClearChat_Click(object sender, EventArgs e)
        {
            ClearChatLog();
            RefreshChatLog();
        }

        /// Last Modified: 10/24/10
        /// <summary>
        /// Buzzes the user to alert them.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: The chat log will be empty.
        /// -----------------------------------------------------
        /// Return Value:
        private void tsbBuzz_Click(object sender, EventArgs e)
        {
            //Write to the chat log & update the display.
            WriteToChatLog(YourUserId, "<b><font color=\"red\">You have buzzed " + FriendsUserId + "!</font></b>");
            RefreshChatLog();

            //Send the message & update the chat log.
            if(Node != null)
                Node.SendMsg(FriendsUserId, "<buzz/><b><font color=\"red\">" + YourUserId + " has buzzed you!</font></b>");
        }

        /// Last Modified: 10/24/10
        /// <summary>
        /// Allows custom opacity changes for the specific window.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: The chat log will be empty.
        /// -----------------------------------------------------
        /// Return Value:
        private void tsbOpacity_TextChanged(object sender, EventArgs e)
        {
            //Cast the item in the combobox as a string and make sure it is valid.
            string opacityStr = (string)tsbOpacity.Text;

            //Terminate if the string is not valid.
            if (String.IsNullOrEmpty(opacityStr))
                return;

            //Strip out the percent sign & terminate if the value is not numeric.
            double opacity = 0.0;
            if (double.TryParse(opacityStr.Replace("%", ""), out opacity))
            {
                //Verify the domain is between 0.1 & 1.0
                if (opacity >= 10.0 && opacity <= 100.0)
                {
                    this.Opacity = m_InFocusOpacity = opacity / 100;
                    if (OpacityChangedEventCallback != null) OpacityChangedEventCallback(this, InFocusOpacity);
                }
            }
        }

        #endregion ToolbarEvents

        /// Last Modified: 9/22/10
        /// <summary>
        /// This method call writes a message to the chatlog externally.
        /// NOTE: Thread-safe.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="msg">
        /// An incoming message from another user.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public void WriteMessageToChatLog(string msg)
        {
            //Write to the chat log & update the display.
            WriteToChatLog(FriendsUserId, msg);
            RefreshChatLog();

            //Green Max is alert with a message.
            SafelySetIcon(DSChat.Properties.Resources.GreenMax);
        }         

        /// Last Modified: 10/2/10
        /// <summary>
        /// This method call writes a list of message to the chatlog externally.
        /// NOTE: Thread-safe.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="chatlist">
        /// A list of messages to write.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public void WriteMessageToChatLog(List<string> chatlist)
        {
            //Write to the chat log & update the display.
            WriteToChatLog(FriendsUserId, chatlist);
            RefreshChatLog();

            //Green Max is alert with a message.
            SafelySetIcon(DSChat.Properties.Resources.GreenMax);
        }

        #endregion Events
    }
}
