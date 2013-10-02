using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///////////////////////////////
///        CUSTOM           ///
///////////////////////////////
using NodeSystem;

namespace DSChat.Helpers
{
    /// <summary>
    /// An intermediary class that manages the appropriate structure depending on what is available.
    /// </summary>
    public class ChatIntermediary
    {
        #region MemberVars

        private ChatForm m_chatform = null;
        private List<string> m_offlineChat = null;

        #endregion MemberVars

        #region Properties

        /// <summary>
        /// Safely return the visible state.
        /// </summary>
        /// <returns>true if the form is visible.</returns>
        public bool IsVisible
        {
            get { return (m_chatform != null) ? m_chatform.Visible : false; }
        }

        /// Last Updated: 10/31/10
        /// <summary>
        /// Provides the in focus opacity of the chatform.  If the chatform is not valid then the opacity is always 100% by default.
        /// </summary>
        /// <returns>
        /// Returns the in focus opacity, which is 100% if the form is invalid.
        /// </returns>
        public double InFocusOpacity
        {
            get { return (m_chatform != null) ? m_chatform.InFocusOpacity : 1.0; }
            set { if (m_chatform != null) m_chatform.InFocusOpacity = value; }
        }

        /// Last Modified: 10/31/10
        /// <summary>
        /// A callback that is triggered when the opacity has changed.
        /// </summary>
        public ChatForm.OnOpacityChangedEventDelegate OpacityChangedEventCallback
        {
            set
            {
                if (m_chatform != null)
                    m_chatform.OpacityChangedEventCallback = value;
            }
        }

        /// Last Modified: 10/31/10
        /// <summary>
        /// A callback that is triggered when the user has noticed the message's arrival.
        /// </summary>
        public ChatForm.OnMessageNoticedEventDelegate MessageNoticedEventCallback
        {
            set
            {
                if (m_chatform != null) 
                    m_chatform.MessageNoticedEventCallback = value;
            }
        }

        #endregion Properties

        #region Methods

        public ChatIntermediary() { }

        /// <summary>
        /// Attempts to write a message to the chat log.  If the chatform has not been initialized
        ///  then the information will be stored in an offline buffer until the user opens the window.
        /// </summary>
        /// <param name="sMsg">
        /// The message to write to the chat log.
        /// </param>
        public void WriteToChatLog(string sMsg)
        {
            //A chatform that is not instantiated is one that has not been created
            // by the user's thread, and therefore, cannot be accessed until the user
            // has decided to open the form.
            if (m_chatform == null)
            {
                //Instantiate on demand.
                if (m_offlineChat == null)
                    m_offlineChat = new List<string>();

                m_offlineChat.Add(sMsg);
            }
            else
                m_chatform.WriteMessageToChatLog(sMsg);
        }

        /// <summary>
        /// Creates an instance.
        /// </summary>
        /// <param name="chatform"></param>
        public void Create(string friendsUserId, string currentUserId, ChildNode cn)
        {
            //Set the chatform and dump the offline message into the new chatform.
            if (m_chatform == null)
            {
                m_chatform = new ChatForm(friendsUserId, currentUserId, cn);

                if (m_offlineChat != null)
                    m_chatform.WriteMessageToChatLog(m_offlineChat);
            }
        }

        /// <summary>
        /// Close the chatform safely.
        /// </summary>
        public void Close()
        {
            if (m_chatform != null) m_chatform.Close();
        }

        /// <summary>
        /// Safely show the form.
        /// </summary>
        public void Show()
        {
            if (m_chatform != null) m_chatform.Show();
        }

        /// <summary>
        /// Determines if the chatintermediary contains the specific chatform.
        /// </summary>
        /// <param name="cf">A chatform object.</param>
        public bool Contains(ChatForm cf)
        {
            return (cf == m_chatform);
        }

        #endregion Methods
    }
}
