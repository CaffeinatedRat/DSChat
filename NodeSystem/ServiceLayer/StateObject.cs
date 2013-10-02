using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///////////////////////////////
///        CUSTOM           ///
///////////////////////////////
using System.Net.Sockets;
using System.Net;

namespace NodeSystem.ServiceLayer
{
    internal class StateObject
    {
        #region Constants

        /// Maintains the maximum size of the buffer.
        public const int BUFFER_SIZE = 5120;

        #endregion Constants

        #region MemberVars

        /// Maintains the local socket.
        public Socket socket = null;

        /// Maintains the local endpoint.
        public IPEndPoint iep = null;

        /// Maintains the buffer.
        public byte[] buffer = new byte[BUFFER_SIZE];

        #endregion MemberVars
    }   //END OF CLASS
}   //END OF NAMESPACE
