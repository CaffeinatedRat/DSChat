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
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Threading;
using NodeSystem;
using NodeSystem.ServiceLayer;
using NodeSystem.SecurityModels;
using NodeSystem.Exceptions;
using Common;
using DSChat.Helpers;

namespace DSChat
{
    public partial class DSChatForm : Form
    {
        #region DataTypes

        /// <summary>
        /// This enumeration provides variability in the use of the cross-thread UpdateRegisteredUsersListBox method.
        /// </summary>
        public enum ListViewArgumentType
        {
            AddItem,
            RemoveItem,
            ModifyItem,
            RemoveAllItems
        };

        /// <summary>
        /// Describes the various connection states of the application.
        /// </summary>
        public enum ConnectionState
        {
            Connected,
            IsConnecting,
            Disconnected
        };

        #endregion DataTypes

        #region Constants

        /// <summary>
        /// Provides a timeout value for the mutex, which is set to 5 seconds.
        /// </summary>
        private const int MUTEX_TIMEOUT = 5000;

        /// <summary>
        /// The amount of time the child will wait for a successful registration message before timing out.
        /// </summary>
        private const int CONNECTION_TIMEOUT = 5000;

        #endregion Constants

        #region MemberVars

        /// Maintains a main node.
        private RootNode m_Node = null;

        //Maintains a collection of chats.
        //NOTE: Instantiate when in use to prevent unnecessary memory overhead.
        private Dictionary<string, ChatIntermediary> m_Chats = new Dictionary<string, ChatIntermediary>();

        //Determines if this application is hosting.
        protected bool m_IsHosting = false;

        /// <summary>
        /// Causes the tooltip to appear once.
        /// </summary>
        private bool m_IsToolTipSeen = false;

        /// <summary>
        /// Synchronization for the IsConnected property.
        /// </summary>
        private static Mutex m_mutexIsConnected = new Mutex(false, "IsConnectedMutex");

        /// <summary>
        /// The thread that waits for the child to connect before timing out.
        /// </summary>
        private Thread m_connectionWaitingThread = null;

        #endregion MemberVars

        #region Properties

        /// Last Modified: 9/13/10
        /// <summary>
        /// Determines the connection state of the DSChat program.
        /// </summary>
        protected ConnectionState IsConnected { get; set; }

        /// Last Modified: 10/31/10
        /// <summary>
        /// Maintains a global in focus opacity for all chatforms.
        /// </summary>
        protected double GlobalInFocusOpacity { get; set; }

        #endregion Properties

        #region Methods

        /// Last Modified: 9/12/10
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
        public DSChatForm()
        {
            InitializeComponent();

            //By default is disconnected.
            IsConnected = ConnectionState.Disconnected;

            //Show the local IPAddress.
            lblMyAddress.Text = NodeSocket.GetLocalIP().ToString();

            //Default opacity.
            GlobalInFocusOpacity = 1.0;

            LoadSettings();

            //Change the behavior of the form based on the saved settings.
            grpConnectionInfo.Enabled = !cbHosting.Checked;
            m_IsHosting = cbHosting.Checked;

            //Update the title based on the version.
            this.Text = "DSChat v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            //Only shown during debugging.
#if (DEBUG)
            debugToolStripMenuItem.Visible = true;
#endif
        }

        /// Last Modified: 10/24/10
        /// <summary>
        /// Connect the node depending on the hosting state.
        /// Hosting Enabled: Begin running the server.
        /// Hosting Disabled: Connects to the host.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS:
        /// 1) The host server will be running.
        /// 2) The child node will attempt to connect to the server.
        /// -----------------------------------------------------
        /// Return Value:
        private void Connect()
        {
            try
            {
                NodeSimpleSecurityModel nssm = null;
                if(!String.IsNullOrEmpty(txtPassphrase.Text))
                    nssm = new NodeSimpleSecurityModel(SimpleCryptography.EncryptionScheme.DES, txtPassphrase.Text);

                //The host never generates an asynchronous event and either fails
                // or succeeds immediately.
                SafelyToggleInterfaceState(true);

                //Determine if the node will be a host/parent.
                if (m_IsHosting)
                {
                    //Obtain the port to listen on.
                    int nPort = int.Parse(txtListeningPort.Text);
                    
                    //Create the parent node and start listening.
                    ParentNode pn = new ParentNode(nPort, nssm);
                    pn.NodeEventCallback = ParentNode_OnNodeEvent;
                    pn.Start();

                    //Update the hosting status.
                    lblStatus.Text = "Running...";

                    m_Node = pn;

                    //Update the notification icon to reflect the node's name.
                    cmiNodeType.Text = notificationIcon.Text = "DSChat [Parent]";

                    //Neutral Max means everything is okay.
                    SafelySetIcon(DSChat.Properties.Resources.MAX);

                    IsConnected = ConnectionState.Connected;
                }
                //This would be a child node.
                else
                {
                    //Obtain the port to listen on for responses.
                    int nListeningPort = int.Parse(txtListeningPort.Text);

                    //Obtain the server address & port of the parent\host.
                    int nHostPort = int.Parse(txtHostPort.Text);
                    IPAddress ipAddy = NodeSocket.GetIP4HostAddresses(txtServer.Text);

                    //Create the child node and register it with the parent with the
                    // node name provided.
                    ChildNode cn = new ChildNode(nListeningPort, nssm);
                    cn.NodeName = txtName.Text;
                    cn.NodeEventCallback = ChildNode_OnNodeEvent;
                    cn.Start(ipAddy, nHostPort);

                    m_Node = cn;

                    //Update the notification icon to reflect the node's name.
                    cmiNodeType.Text = notificationIcon.Text = "DSChat [" + txtName.Text + "]";

                    //The connection has not been made and synchornization is not required here
                    // as the thread has not been spun up yet.
                    IsConnected = ConnectionState.IsConnecting;

                    SetStatusLabel("Connecting...");

                    m_connectionWaitingThread = new Thread(ThreadProc_WaitingForConnection);
                    m_connectionWaitingThread.Start((object)this);
                }
            }
            catch (Exception ex)
            {
                SetStatusLog(ex.Message);
                Disconnect();
            }
        }

        /// Last Modified: 9/12/10
        /// <summary>
        /// Disconnects the node depending on the hosting state.
        /// Hosting Enabled: Shuts down the server immediately (synchronously) and notifies the children that the server is no longer available.
        /// Hosting Disabled: Disconnects from the host immediately (synchronously).
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS:
        /// 1) The child node must be connected to the host.
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS:
        /// 1) The host server will no longer be running and all children will be notified.
        /// 2) The child node will no longer be connected to the host.
        /// -----------------------------------------------------
        /// Return Value:
        private void Disconnect()
        {
            //Clean-up all open chats.
            m_Chats.Clear();

            //Disconnect the node.
            if (m_Node != null)
            {
                m_Node.Close();
                m_Node = null;
            }

            //Update the interface.
            //Both of the host & child terminate their services immediately.
            SafelyToggleInterfaceState(false);

            //Red-Max is angry and notifies users they are disconnected.
            SafelySetIcon(DSChat.Properties.Resources.RedMax);

            //Control the connection flag via the disconnect & connect methods as they may be called out
            // of their original content.
            m_mutexIsConnected.WaitOne(MUTEX_TIMEOUT);
            IsConnected = ConnectionState.Disconnected;
            m_mutexIsConnected.ReleaseMutex();

            //Clear all of the individuals.
            UpdateRegisteredUsersListBox(null, ListViewArgumentType.RemoveAllItems, 0);
        }

        #region SettingsManagement

        /// Last Modified: 10/3/10
        /// <summary>
        /// Loads the DSChat's settings from the file DSSettings.Xml.  If the file
        ///  does not exist then terminate this method safely.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS:
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS:
        /// -----------------------------------------------------
        /// Return Value:
        private void LoadSettings()
        {
            try
            {
                XmlTextReader reader = new XmlTextReader("DSSettings.xml");

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(reader);

                XmlElement eSetup = (XmlElement)xmlDoc.GetElementsByTagName("Setup").Item(0);
                
                XmlElement eHost = (XmlElement)eSetup.GetElementsByTagName("Host").Item(0);
                cbHosting.Checked = (eHost.GetAttribute("IsHosting") == "1");
                txtListeningPort.Text = eHost.GetAttribute("Port");

                XmlElement eConnection = (XmlElement)eSetup.GetElementsByTagName("Connection").Item(0);
                txtServer.Text = eConnection.GetAttribute("Server");
                txtHostPort.Text = eConnection.GetAttribute("Port");
                txtName.Text = eConnection.GetAttribute("Name");

                XmlElement eSecurity = (XmlElement)eSetup.GetElementsByTagName("Security").Item(0);
                txtPassphrase.Text = SimpleCryptography.PassphraseStrDecrypt("k5d*3@2!d3v9", SimpleCryptography.EncryptionScheme.AES, eSecurity.InnerText);

                //Note that this tag does not exist in earlier settings, so ignore it if it is missing.
                XmlElement eOpacity = (XmlElement)eSetup.GetElementsByTagName("Opacity").Item(0);
                if (eOpacity != null)
                {
                    double Opacity = 1.0;
                    if (double.TryParse(eOpacity.InnerText, out Opacity))
                        GlobalInFocusOpacity = Opacity;
                }

                reader.Close();
            }
            catch (FileNotFoundException) {}
        }

        /// Last Modified: 10/3/10
        /// <summary>
        /// Saves DSChat's settings to an XML formatted file.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: The user's settings will be saved to a file labeled DSSettings.xml.
        /// -----------------------------------------------------
        /// <returns></returns>
        private void SaveSettings()
        {
            XmlDocument doc = new XmlDocument();
            XmlTextWriter writer = new XmlTextWriter("DSSettings.xml", null);
            writer.Formatting = Formatting.Indented;

            writer.WriteStartElement("Setup");
            writer.WriteAttributeString("verson", "1.3");

            writer.WriteStartElement("Host");
            writer.WriteAttributeString("IsHosting", cbHosting.Checked ? "1" : "0");
            writer.WriteAttributeString("Port", txtListeningPort.Text);
            writer.WriteEndElement();

            writer.WriteStartElement("Connection");
            writer.WriteAttributeString("Port", txtHostPort.Text);
            writer.WriteAttributeString("Server", txtServer.Text);
            writer.WriteAttributeString("Name", txtName.Text);
            writer.WriteEndElement();

            writer.WriteStartElement("Security");
            writer.WriteString(SimpleCryptography.PassphraseStrEncrypt("k5d*3@2!d3v9", SimpleCryptography.EncryptionScheme.AES, txtPassphrase.Text));
            writer.WriteEndElement();

            writer.WriteStartElement("Opacity");
            writer.WriteString(GlobalInFocusOpacity.ToString());
            writer.WriteEndElement();

            writer.Close();
        }

        #endregion SettingsManagement

        #region Invocation_CrossThreadAccess

        /// Last Modified: 9/13/10
        /// <summary>
        /// Safely writes a message to the status log window.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="text">
        /// The text to add to the status window.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public void SetStatusLog(String text)
        {
            //Call across threads.
            if (this.txtStatusLog.InvokeRequired)
            {
                this.txtStatusLog.BeginInvoke(
                    new MethodInvoker(
                        delegate() { SetStatusLog(text); }));
            }
            else
            {
                //Overload protection.
                if (txtStatusLog.Text.Length > 10000) txtStatusLog.Text = "";

                //Add a newline when needed.
                if (!String.IsNullOrEmpty(this.txtStatusLog.Text))
                    this.txtStatusLog.Text += "\r\n";

                //Write to the textbox and add a timestamp.
                DateTime dt = DateTime.Now;
                this.txtStatusLog.Text += String.Format("[{0} {1}]: {2}",
                    dt.ToShortDateString(),
                    dt.ToShortTimeString(),
                    text);
            }   //END OF if (this.txtStatusLog.InvokeRequired)...
        }

        /// Last Modified: 9/14/10
        /// <summary>
        /// Safely updates the listbox allowing an item to be
        ///  added or removed from the listbox.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="sItem">
        /// Item to add or remove from the list box.
        /// This value can be null or an empty string if the <typeparamref name="ListViewArgumentType.RemoveAllItems"/>
        /// </param>
        /// <param name="lvat">
        /// An enumeration value that defines the action of this method.
        /// </param>
        /// <param name="nImgIdx">
        /// An image index that is used to modify the image in the listview.
        /// NOTE: This value is disregarded with <typeparamref name="ListViewArgumentType.RemoveItem"/> &amp; <typeparamref name="ListViewArgumentType.RemoveAllItems"/>
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: The registered user's listbox will updated.
        /// -----------------------------------------------------
        /// Return Value:
        public void UpdateRegisteredUsersListBox(string sItem, ListViewArgumentType lvat, int nImgIdx)
        {
            //Call across threads.
            if (this.txtStatusLog.InvokeRequired)
            {
                this.txtStatusLog.BeginInvoke(
                    new MethodInvoker(
                        delegate() { UpdateRegisteredUsersListBox(sItem, lvat, nImgIdx); }));
            }
            else
            {
                switch(lvat)
                {
                    case ListViewArgumentType.AddItem:
                        {
                            lvRegisteredUsers.Items.Add(sItem, sItem, nImgIdx);
                        }
                        break;

                    case ListViewArgumentType.RemoveItem:
                        {
                            lvRegisteredUsers.Items.RemoveByKey(sItem);
                        }
                        break;

                    case ListViewArgumentType.ModifyItem:
                        {
                            if (lvRegisteredUsers.Items.ContainsKey(sItem))
                                lvRegisteredUsers.Items[sItem].ImageIndex = nImgIdx;
                        }
                        break;

                    case ListViewArgumentType.RemoveAllItems:
                        {
                            //Clear them all.
                            lvRegisteredUsers.Items.Clear();
                        }
                        break;
                }   //END OF switch(lvat)....
            }   //END OF if (this.txtStatusLog.InvokeRequired)...
        }

        /// Last Modified: 9/14/10
        /// <summary>
        /// Safely sets the status of the status label.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="sStatus">
        /// A short label to apply to the status label.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: The status label will have been updated.
        /// -----------------------------------------------------
        /// Return Value:
        public void SetStatusLabel(string sStatus)
        {
            //Call across threads.
            if (this.txtStatusLog.InvokeRequired)
            {
                this.txtStatusLog.BeginInvoke(
                    new MethodInvoker(
                        delegate() { SetStatusLabel(sStatus); }));
            }
            else
            {
                lblStatus.Text = sStatus;
            }
        }

        /// Last Modified: 9/14/10
        /// <summary>
        /// Safely toggles the interface between a connected & disconnected state.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="bIsConnected">
        /// Determines if the interface is connected.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: The interface will be updated to reflect
        ///  its status.
        /// -----------------------------------------------------
        /// Return Value:
        public void SafelyToggleInterfaceState(bool bIsConnected)
        {
            //Call across threads.
            if (this.txtStatusLog.InvokeRequired)
            {
                this.txtStatusLog.BeginInvoke(
                    new MethodInvoker(
                        delegate() { SafelyToggleInterfaceState(bIsConnected); }));
            }
            else
            {
                //Update the interface to disable certain fields after successful connection.
                if (bIsConnected)
                {
                    //Disable the various buttons that will not be operational while connected.
                    grpHostingInfo.Enabled = grpSecurity.Enabled = grpConnectionInfo.Enabled = false;

                    //Notify the user that his or her next action will result in a disconnection.
                    connectToolStripMenuItem.Text = btnConnect.Text = "Disconnect";
                }
                else
                {
                    //Determine if the node is a child or a parent\host.
                    if (!cbHosting.Checked)
                    {
                        //NOTE: Re-enable the ConnectionInfo group only for the child node.
                        grpConnectionInfo.Enabled = true;
                    }

                    //Re-enable many of the options that were not available during connection.
                    grpHostingInfo.Enabled = grpSecurity.Enabled = true;

                    lblStatus.Text = "Not Connected";

                    //Notify the user that his or her next action will result in a disconnection.
                    connectToolStripMenuItem.Text = btnConnect.Text = "Connect";
                }   //END OF if (bIsConnected)...
            }   //END OF if (this.txtStatusLog.InvokeRequired)...
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
                notificationIcon.Icon = this.Icon = ico;
            }
        }

        #endregion Invocation_CrossThreadAccess

        #endregion Methods

        #region Events

        #region MainFormEvents

        /// Last Modified: 9/12/10
        /// <summary>
        /// When the user closes the form through the X button,
        ///  a close event is generated on the node.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: Spawns a close event for the node.
        /// -----------------------------------------------------
        /// Return Value:
        private void DSChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Close the node.
            if (m_Node != null) m_Node.Close();

            if (m_Chats != null)
                foreach (KeyValuePair<string, ChatIntermediary> kvp in m_Chats)
                    kvp.Value.Close();

            //Save the user's settings.
            SaveSettings();
        }

        /// Last Modified: 10/24/10
        /// <summary>
        /// Occurs when form is minimized or restored, causing the
        ///  notification icon to appear when minimized and disappear
        ///  when restored.
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
        private void DSChatForm_Resize(object sender, EventArgs e)
        {
            //Show the notification icon when the form is minimized.
            if (this.WindowState == FormWindowState.Minimized)
            {
                if (!m_IsToolTipSeen)
                {
                    notificationIcon.ShowBalloonTip(1000);
                    m_IsToolTipSeen = true;
                }

                this.Hide();
                cmitemVisibility.Text = "Show DSChat";
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                cmitemVisibility.Text = "Hide DSChat";
            }
        }

        #endregion MainFormEvents

        /// Last Modified: 9/12/10
        /// <summary>
        /// When the user closes the form through the exit button,
        ///  a close event is generated on the node.
        /// NOTE: This event is also generated when the user
        ///  selects exit from the file menu.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: Spawns a close event for the node.
        /// -----------------------------------------------------
        /// Return Value:
        private void btnExit_OnClick(object sender, EventArgs e)
        {
            this.Close();
        }

        /// Last Modified: 9/12/10
        /// <summary>
        /// An event that is triggered when the user attempts
        ///  to run the node.
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
        private void btnConnect_OnClick(object sender, EventArgs e)
        {
            //Connect when only disconnected.
            if (IsConnected == ConnectionState.Disconnected)
            {
                Connect();
            }
            else
            {
                if ((m_connectionWaitingThread != null) && (m_connectionWaitingThread.IsAlive))
                    m_connectionWaitingThread.Abort();

                Disconnect();
            }
        }

        /// Last Modified: 9/12/10
        /// <summary>
        /// An event that is triggered when the user toggles the
        ///  hosting option, which will enable or disable the 
        ///  connection info group..
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
        private void cbHosting_OnClick(object sender, EventArgs e)
        {
            grpConnectionInfo.Enabled = !((CheckBox)sender).Checked;
            m_IsHosting = ((CheckBox)sender).Checked;
        }

        /// Last Modified: 10/24/10
        /// <summary>
        /// An event that is triggered when an event occurs on a parent node.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="nodeEvent">
        /// The node event that has occcurred.
        /// </param>
        /// <param name="sEntityId">
        /// The id or name of the entity that spawned the event.
        /// </param>
        /// <param name="sData">
        /// Associated data with the event.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public void ParentNode_OnNodeEvent(RootNode.NodeEvent nodeEvent, string sEntityId, string sData)
        {
            switch (nodeEvent)
            {
                case RootNode.NodeEvent.REGISTERED:
                    {
                        UpdateRegisteredUsersListBox(sEntityId, ListViewArgumentType.AddItem, 0);
                        SetStatusLog("User " + sEntityId + " has joined on " + sData);
                    }
                    break;

                case RootNode.NodeEvent.UNREGISTERED:
                    {
                        UpdateRegisteredUsersListBox(sEntityId, ListViewArgumentType.RemoveItem, 0);
                        SetStatusLog("User " + sEntityId + " has left.");
                    }
                    break;

                case RootNode.NodeEvent.ALREADY_REGISTERED:
                    {
                        SetStatusLog("User " + sEntityId + " has already joined.");
                    }
                    break;

                case RootNode.NodeEvent.NETWORK_ERROR:
                    {
                        SetStatusLog(sData);
                    }
                    break;

                case RootNode.NodeEvent.SEVERE_NETWORK_ERRROR:
                    {
                        SetStatusLog(sData);

                        //Need synchronization here.
                        Disconnect();
                    }
                    break;
            }
            //END OF switch (nodeEvent)...
        }

        /// Last Modified: 9/18/10
        /// <summary>
        /// An event that is triggered when an event occurs on a child node.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="nodeEvent">
        /// The node event that has occcurred.
        /// </param>
        /// <param name="sEntityId">
        /// The id or name of the entity that spawned the event.
        /// </param>
        /// <param name="sData">
        /// Associated data with the event.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public void ChildNode_OnNodeEvent(RootNode.NodeEvent nodeEvent, string sEntityId, string sData)
        {
            switch (nodeEvent)
            {
                case RootNode.NodeEvent.REGISTRATION_SUCCESSFUL:
                    {
                        //If the string is empty, don't bother.
                        if (!String.IsNullOrEmpty(sData))
                        {
                            //Tokenize the string and update the registered users list box
                            // with the current list of active users.
                            string[] sUsersList = sData.Split(new char[] { '~' });
                            foreach (string s in sUsersList)
                                UpdateRegisteredUsersListBox(s, ListViewArgumentType.AddItem, 0);
                        }
                
                        SafelyToggleInterfaceState(true);

                        //Notify the form that a successful connection has been made.
                        m_mutexIsConnected.WaitOne(MUTEX_TIMEOUT);
                        IsConnected = ConnectionState.Connected;
                        m_mutexIsConnected.ReleaseMutex();

                        SetStatusLabel("Connected");
                        SetStatusLog("Successfully registered with the host.");

                        //Neutral Max means everything is okay.
                        SafelySetIcon(DSChat.Properties.Resources.MAX);
                    }
                    break;

                case RootNode.NodeEvent.REGISTRATION_FAILURE:
                    {
                        Disconnect();

                        SetStatusLabel("Not connected");
                        SetStatusLog(sEntityId + " has already registered.  Use a different name.");
                    }
                    break;

                case RootNode.NodeEvent.NODE_ADDED:
                    {
                        UpdateRegisteredUsersListBox(sData, ListViewArgumentType.AddItem, 0);
                        SetStatusLog("User " + sData + " has joined.");
                    }
                    break;

                case RootNode.NodeEvent.NODE_REMOVED:
                    {
                        UpdateRegisteredUsersListBox(sData, ListViewArgumentType.RemoveItem, 0);
                        SetStatusLog("User " + sData + " has left.");

                        //Ignore any invalid ChatIntermediaries.
                        ChatIntermediary ci = null;
                        if (m_Chats.ContainsKey(sData))
                        {
                            ci = m_Chats[sData];
                        }
                        else
                        {
                            ci = new ChatIntermediary();
                            m_Chats.Add(sData, ci);
                        }

                        ci.WriteToChatLog("User " + sData + " has left.");
                    }
                    break;

                case RootNode.NodeEvent.INVALID_NODE:
                    {
                        SetStatusLog(sData + " is not a valid node.");
                    }
                    break;

                case RootNode.NodeEvent.PARENT_SHUTDOWN:
                    {
                        Disconnect();

                        SetStatusLabel("Not connected");
                        SetStatusLog("The server has shutdown.");
                    }
                    break;

                case RootNode.NodeEvent.NETWORK_ERROR:
                    {
                        SetStatusLog(sData);
                    }
                    break;

                case RootNode.NodeEvent.MESSAGE:
                    {
                        //Ignore any invalid ChatIntermediaries.
                        ChatIntermediary ci = null;
                        if (m_Chats.ContainsKey(sEntityId))
                        {
                            ci = m_Chats[sEntityId];
                        }
                        else
                        {
                            ci = new ChatIntermediary();
                            m_Chats.Add(sEntityId, ci);
                        }
                        //END OF if (m_Chats.ContainsKey(sEntityId))...

                        ci.WriteToChatLog(sData);
                        UpdateRegisteredUsersListBox(sEntityId, ListViewArgumentType.ModifyItem, (ci.IsVisible ? 0 : 1));
                        SafelySetIcon(DSChat.Properties.Resources.GreenMax);
                    }
                    break;

                case RootNode.NodeEvent.SEVERE_NETWORK_ERRROR:
                    {
                        SetStatusLog(sData);

                        //Need synchronization here.
                        Disconnect();
                    }
                    break;
            }
            //END OF switch (nodeEvent)...
        }

        /// Last Modified: 9/21/10
        /// <summary>
        /// An event that is triggered when a user double clicks on the registered
        ///  user's listview.
        /// The host should ignore these events at this time.
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
        private void lvRegisteredUsers_Click(object sender, EventArgs e)
        {
            //Ignore messages from the host.
            if (m_IsHosting) return;

            string sEntityId = ((ListView)sender).SelectedItems[0].Text;

            ChatIntermediary ci = null;

            if (!m_Chats.ContainsKey(sEntityId))
            {
                ci = new ChatIntermediary();
                m_Chats.Add(sEntityId, ci);
            }
            else
                ci = m_Chats[sEntityId];

            //Set common properties.
            ci.Create(sEntityId, txtName.Text, (ChildNode)m_Node);
            ci.InFocusOpacity = GlobalInFocusOpacity;
            ci.OpacityChangedEventCallback = OnOpacityChanged_ChatForm;
            ci.MessageNoticedEventCallback = OnMessageNoticed_ChatForm;
            
            //When a user clicks on this item, restore the original icon.
            UpdateRegisteredUsersListBox(sEntityId, ListViewArgumentType.ModifyItem, 0);
            SafelySetIcon(DSChat.Properties.Resources.MAX);

            //Show the chat window.
            ci.Show();
        }

        /// Last Modified: 10/31/10
        /// <summary>
        /// Connects or disconnects the user.
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
        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Connect on a null node only.
            if (IsConnected == ConnectionState.Disconnected)
                Connect();
            else
                Disconnect();
        }

        /// Last Modified: 10/31/10
        /// <summary>
        /// Ocurrs when the opacity of any chatform changes.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="opacity">
        /// The opacity chnage.  Domain [0.0, 1.0].
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        private void OnOpacityChanged_ChatForm(ChatForm chatform, double opacity)
        {
            //Not important enough for synchronization.
            GlobalInFocusOpacity = opacity;

            //Apply the changes globally.
            foreach (KeyValuePair<string, ChatIntermediary> kvp in m_Chats)
                if (!kvp.Value.Contains(chatform))
                    kvp.Value.InFocusOpacity = opacity;

            //Need to set all other chats, but not this one to prevent an infinite loop.
        }

        /// Last Modified: 10/31/10
        /// <summary>
        /// Ocurrs when a message has been noticed by the user.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: The DSChat should restore the main
        ///  icons to their normal standard.
        /// -----------------------------------------------------
        /// Return Value:
        private void OnMessageNoticed_ChatForm()
        {
            SafelySetIcon(DSChat.Properties.Resources.MAX);
        }

        #region Threading

        /// Last Modified: 11/11/10
        /// <summary>
        /// A thread that simply waits for a specific amount of time before
        ///  requesting the supplied chatform disconnect.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="obj">
        /// Contains the chatform to manipulate.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: 
        /// -----------------------------------------------------
        /// Return Value:
        private static void ThreadProc_WaitingForConnection(object obj)
        {
            DSChatForm chatform = (DSChatForm)obj;

            //Wait for the time-out.
            Thread.Sleep(CONNECTION_TIMEOUT);
            
            try
            {
                m_mutexIsConnected.WaitOne(MUTEX_TIMEOUT);

                //Force a disconnect if the chatform hasn't connected at this time.
                if (chatform.IsConnected == ConnectionState.IsConnecting)
                {
                    chatform.Disconnect();
                    chatform.SetStatusLog("The connection timed-out as a connection could not be established.");
                }
            }
            finally
            {
                //If the mutex is waiting, always release it.
                m_mutexIsConnected.ReleaseMutex();
            }
        }

        #endregion Threading

        #region NotifyIconEvents

        /// Last Modified: 10/24/10
        /// <summary>
        /// Occurs when the notification icon is double clicked resulting
        ///  in the DSChat form becoming visible.
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
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        /// Last Modified: 10/24/10
        /// <summary>
        /// Toggle the visiblity state based on the option in the notify icon area.
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
        private void cmitemVisibility_Click(object sender, EventArgs e)
        {
            
            if (this.Visible)
            {
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
            }
            else
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
        }

        /// Last Modified: 10/24/10
        /// <summary>
        /// Occurs when the "Exit" item menu is clicked causing
        ///  the DSChat form to close.
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
        private void cmitemExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// Last Modified: 10/31/10
        /// <summary>
        /// Clears the status log.
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
        private void clearStatusLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtStatusLog.Text = "";
        }

        #endregion NotifyIconEvents

        #region DebugEvents

        // Last Modified: 10/31/10
        /// <summary>
        /// Forces the chatform to show up for debugging purposes.
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
        private void showChatFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sEntityId = "DEBUG";

            ChatIntermediary ci = null;

            if (!m_Chats.ContainsKey(sEntityId))
            {
                ci = new ChatIntermediary();
                m_Chats.Add(sEntityId, ci);
            }
            else
                ci = m_Chats[sEntityId];

            //Set common properties.
            ci.Create(sEntityId, txtName.Text, (ChildNode)m_Node);
            ci.InFocusOpacity = GlobalInFocusOpacity;
            ci.OpacityChangedEventCallback = OnOpacityChanged_ChatForm;
            ci.MessageNoticedEventCallback = OnMessageNoticed_ChatForm;

            //When a user clicks on this item, restore the original icon.
            UpdateRegisteredUsersListBox(sEntityId, ListViewArgumentType.ModifyItem, 0);

            //Show the chat window.
            ci.Show();
        }

        #endregion DebugEvents

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About aboutform = new About();
            //aboutform.Show(this);
            aboutform.ShowDialog(this);
        }

        #endregion Events

    }   //END OF CLASS
}   //END OF NAMESPACE
