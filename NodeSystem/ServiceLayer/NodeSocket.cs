using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///////////////////////////////
///        CUSTOM           ///
///////////////////////////////
using System.Net.Sockets;
using System.Net;
using System.IO;
using NodeSystem.Packets;
using NodeSystem.Interfaces;
using NodeSystem.Exceptions;

namespace NodeSystem.ServiceLayer
{
    public class NodeSocket
    {
        #region Delegates

        /// Callback used to handle an asynchronous receive event.
        public delegate void OnAsynchReceiveDelegate(Packet p);

        /// Callback used to handle threaded receive errors.
        public delegate void OnAsynchReceiveErrorDelegate(Exception ex);

        /// Callback used to handle an asynchronous send event.
        public delegate void OnAsynchSendDelegate(out Packet p);

        /// Callback used to handle threaded send errors.
        public delegate void OnAsynchSendErrorDelegate(Exception ex);

        #endregion Delegates

        #region MemberVars

        /// <summary>
        /// Maintains the socket internally as an encapsulated object.  This is to reduce the complexity of the
        ///  socket class itself if this class, NodeSocket, where derived from the Socket class.
        /// </summary>
        protected Socket m_Socket = null;

        /// <summary>
        /// Stores the security model used by the node.  By default this value is null.
        /// </summary>
        protected INodeSecurityModel m_nodeSecurityModel = null;

        /// <summary>
        /// Maintains the local endpoint of the node.
        /// </summary>
        protected IPEndPoint m_iepLocal = null;

        /// <summary>
        /// Maintains the initialized flag.
        /// </summary>
        private bool m_bInitialized = false;

        /// <summary>
        /// Maintains the receive node event callback.
        /// </summary>
        private OnAsynchReceiveDelegate m_delegateOAR = null;

        /// <summary>
        /// Maintains the recieve error node event callback.
        /// </summary>
        private OnAsynchReceiveErrorDelegate m_delegateOARE = null;

        /// <summary>
        /// Maintains the send node event callback.
        /// </summary>
        private OnAsynchSendDelegate m_delegateOSR = null;

        /// <summary>
        /// Maintains the send error node event callback.
        /// </summary>
        private OnAsynchSendErrorDelegate m_delegateOSRE = null;

        #endregion MemberVars

        #region Properties

        /// Last Modified: 11/28/09
        /// <summary>
        /// Assigns the OnAsynchReceive callback.
        /// </summary>
        public OnAsynchReceiveDelegate SetOnAsynchReceive
        {
            set { m_delegateOAR = value; }
        }

        /// Last Modified: 11/28/09
        /// <summary>
        /// Assigns the OnAsynchReceiveError callback.
        /// </summary>
        public OnAsynchReceiveErrorDelegate SetOnAsynchReceiveError
        {
            set { m_delegateOARE = value; }
        }

        /// Last Modified: 11/28/09
        /// <summary>
        /// Assigns the OnAsynchSend callback.
        /// </summary>
        public OnAsynchSendDelegate SetOnAsynchSend
        {
            set { m_delegateOSR = value; }
        }

        /// Last Modified: 11/28/09
        /// <summary>
        /// Assigns the OnAsynchSendError callback.
        /// </summary>
        public OnAsynchSendErrorDelegate SetOnAsynchSendError
        {
            set { m_delegateOSRE = value; }
        }

        /// Last Modified: 11/28/09
        /// <summary>
        /// Returns true if the asynchronous receive operation has begun.
        /// </summary>
        public bool IsInitialized
        {
            get { return m_bInitialized; }
        }

        /// Last Modified: 11/28/09
        /// <summary>
        /// Returns the port of the host.
        /// </summary>
        public int HostPort
        {
            get { return m_iepLocal.Port; }
        }

        #endregion Properties

        #region Methods

        /// Last Modified: 10/17/10
        /// <summary>
        /// Initialize a node socket as a server that can receive
        ///  information locally on the host port supplied, depending on the security model.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="nHostPort">
        /// The port the node will be hosted on.
        /// </param>
        /// <param name="nodeSecurityModel">
        /// An object that manages the security model of the node.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public NodeSocket(int nHostPort, INodeSecurityModel nodeSecurityModel)
        {
            //Create the local EndPoint and bind it to the socket.
            m_iepLocal = new IPEndPoint(IPAddress.Any, nHostPort);
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            m_Socket.Bind(m_iepLocal);
            m_nodeSecurityModel = nodeSecurityModel;
        }

        /// Last Modified: 10/17/10
        /// <summary>
        /// Initialize a node socket as a server that can receive
        ///  information locally on the host port supplied.
        /// NOTE: Does not use a security model.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="nHostPort">
        /// The port the node will be hosted on.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public NodeSocket(int nHostPort) : this (nHostPort, null) {}

        /// Last Modified: 11/28/09
        /// <summary>
        /// Initializes a node socket that can only send information.
        /// This constructor is used when performing asynchronous
        ///  write/send operations.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public NodeSocket()
        {
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        /// Last Modified: 11/28/09
        /// <summary>
        /// Clean up all resources.  This method will be called
        ///  when the garbage collector is activated.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        ~NodeSocket()
        {
            //Release the the local socket.
            Close();
        }

        /// Last Modified: 11/28/09
        /// <summary>
        /// Explicitly closes the socket and releases all allocated
        ///  resources.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public void Close()
        {
            //Release the the local socket.
            if(m_Socket != null)
                m_Socket.Close();
         }

        /// Last Modified: 10/24/10
        /// <summary>
        /// Starts the receiving events.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments:
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        public void AsynchReceive()
        {
            //Avoid invalid socket access.
            if (m_Socket == null) return;

            //If asynchronous receiving has begun then do not
            // call that method again.
            if (!m_bInitialized)
            {
                try
                {
                    //Cast the local end point.
                    EndPoint ep = (EndPoint)m_iepLocal;

                    //Prepare the state object.
                    StateObject state = new StateObject();
                    state.socket = m_Socket;
                    state.iep = m_iepLocal;

                    //Begin the node's local server using an asynchronous method.
                    m_Socket.BeginReceiveFrom(state.buffer, 0, StateObject.BUFFER_SIZE,
                        SocketFlags.None, ref ep,
                        new AsyncCallback(OnReceiveCallback), state);
                }
                catch (SocketException se)
                {
                    //MSDN NOTE: If you receive a SocketException, use the SocketException.ErrorCode property to obtain the specific error code.
                    // After you have obtained this code, refer to the Windows Sockets version 2 API error code documentation in the MSDN library
                    //for a detailed description of the error.
                    if (se.SocketErrorCode == SocketError.AddressAlreadyInUse)
                        throw new NodePortInUseException("That host port is already in use.", se);
                }
                catch (Exception ex)
                {
                    //Call the error callback if one exists.
                    throw new NodeUnknownException(ex.Message, ex);
                }

                //Set the initialized flag to true.
                m_bInitialized = true;
            }   //END OF if (!m_bInitialized)...
        }

        /// Last Modified: 11/28/09
        /// <summary>
        /// Starts the sending the packet.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments:
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public void AsynchSend(Packet p, IPEndPoint ipep)
        {
            //Avoid invalid socket access.
            if (m_Socket == null) return;

            //Prepare the state object.
            StateObject state = new StateObject();
            state.socket = m_Socket;
            state.iep = ipep;
            state.buffer = p.ToByteArray();

            //Begin sending the packet information.
            m_Socket.BeginSendTo(state.buffer, 0, StateObject.BUFFER_SIZE,
                SocketFlags.None, (EndPoint)ipep, 
                new AsyncCallback(OnSendCallback),
                state);
        }

        /// Last Modified: 10/24/10
        /// <summary>
        /// A static method that will send packet information
        ///  to the IPEndpoint supplied.  This method is blocking
        ///  and does not support asynchronous sending of information.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="p">
        /// A packet of information to send.
        /// </param>
        /// <param name="ipep">
        /// The destination endpoint to send the packet.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: The message will be sent or an exception will have been thrown.
        /// -----------------------------------------------------
        /// Exceptions:
        /// 1) NodeSocketInvalid
        /// 2) NodeUnknownException
        /// -----------------------------------------------------
        /// Return Value:
        public void SendTo(Packet p, IPEndPoint ipep)
        {
            Socket s = null;

            try
            {
                //Create the socket to send information to.
                s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                //Cast the endpoint.
                EndPoint ep = (EndPoint)ipep;

                //Perform any necessary encryptions based on the security model.
                byte[] byData = (m_nodeSecurityModel != null)
                    ? m_nodeSecurityModel.ByteEncrypt(p.ToByteArray())
                        : p.ToByteArray();

                //Marshall the string data into a byte stream and send the data.           
                s.SendTo(byData, SocketFlags.None, ep);
            }
            catch (ObjectDisposedException ode)
            {
                throw new NodeSocketInvalid("The Socket has been closed. ", ode);
            }
            catch (SocketException se)
            {
                //MSDN NOTE: If you receive a SocketException, use the SocketException.ErrorCode property to obtain the specific error code.
                // After you have obtained this code, refer to the Windows Sockets version 2 API error code documentation in the MSDN library
                //for a detailed description of the error.
                throw new NodeSocketInvalid(se.Message, se);
            }
            catch (Exception ex)
            {
                //Call the error callback if one exists.
                throw new NodeUnknownException(ex.Message, ex);
            }
            finally
            {
                //Close the socket.
                s.Close();
            }
        }

        /// Last Modified: 12/6/09
        /// <summary>
        /// Retrive the IPAddress of the computer name supplied.
        /// An IPAddress "xxx.xxx.xxx.xxx" is also valid.
        /// Worstcase: O(n) -- Where n is the number of IP Addresses.
        /// Bestcase: O(1) -- Where the first address is used.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: Refer to the return statement.
        /// -----------------------------------------------------
        /// Return Value:
        /// IPAddress -- Returns an IPAddress.
        ///  Null is returned if no address exists.
        public static IPAddress GetIP4HostAddresses(string sHostName)
        {
            //Obtain a list of local IPAddresses.
            IPAddress[] localIPs = Dns.GetHostAddresses(sHostName);

            //Iterate through each address until the first IPV4 address
            // is found, so use it.
            //Worstcase:O(n) -- Where n is the number of IP Addresses.
            //Bestcase:O(1) -- Where the first address is used.
            foreach (IPAddress ipaddr in localIPs)
                if (ipaddr.AddressFamily == AddressFamily.InterNetwork)
                    return ipaddr;

            //No address was found.
            return null;
        }

        /// Last Modified: 10/4/09
        /// <summary>
        /// Retrive the local IP address.
        /// Worstcase: O(n) -- Where n is the number of IP Addresses.
        /// Bestcase: O(1) -- Where the first address is used.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: Refer to the return statement.
        /// -----------------------------------------------------
        /// Return Value:
        /// IPAddress -- Returns an IPAddress.
        public static IPAddress GetLocalIP()
        {
            //Obtain a list of local IPAddresses.
            return GetIP4HostAddresses(Dns.GetHostName());
        }

        /// Last Modified: 11/8/10
        /// <summary>
        /// Checks if the supplied port is in use.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="portNumber">
        /// The port to check.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public static bool IsPortInUse(int portNumber)
        {
            try
            {
                using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, portNumber);
                    s.Bind(endpoint);
                }
            }
            catch (SocketException se)
            {
                if (se.SocketErrorCode == SocketError.AddressAlreadyInUse)
                    return true;

                //Throw all other exceptions upwards.
                throw;
            }

            return false;
        }

        #endregion Methods

        #region Events

        /// Last Modified: 10/24/10
        /// <summary>
        /// This event is fired when an asynchronous receive occurs.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="ar">
        /// An object that contains the received packet.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS:
        /// 1) Async receiving will continue successfull after receiving a message.
        /// 2) An exception will be thrown.
        /// -----------------------------------------------------
        /// Exceptions:
        /// 1) NodeSocketInvalid
        /// 2) NodeUnknownException
        /// -----------------------------------------------------
        /// Return Value:
        protected virtual void OnReceiveCallback(IAsyncResult ar)
        {
            try
            {
                //Retrieve the stateobject.
                StateObject state = (StateObject)ar.AsyncState;

                //Retrieve the socket of the local server
                // and cast the endpoint.
                Socket socket = state.socket;
                EndPoint ep = (EndPoint)state.iep;

                //Retrive the remaining data.
                int read = 0;
                if ((read = socket.EndReceiveFrom(ar, ref ep)) > 0)
                {
                    byte[] byData = state.buffer;

                    try
                    {
                        //Perform any necessary decryptions based on the security model.
                        if (m_nodeSecurityModel != null)
                            byData = m_nodeSecurityModel.ByteDecrypt(state.buffer, ref read);

                        //Marshall the byte stream into a packet object.
                        Packet p = new Packet(byData, read);

                        //Call the callback method if it is valid.
                        if (m_delegateOAR != null) m_delegateOAR(p);
                    }
                    catch (System.Security.Cryptography.CryptographicException ce)
                    {
                        //Call the error callback if one exists.
                        if (m_delegateOARE != null) m_delegateOARE(new NodeSecurityInvalidKeyException("Invalid Key.", ce));
                    }

                    //Continue receiving messages.
                    socket.BeginReceiveFrom(state.buffer, 0, StateObject.BUFFER_SIZE,
                        SocketFlags.None, ref ep,
                        new AsyncCallback(OnReceiveCallback), state);

                }   //END OF if ((read = socket.EndReceiveFrom(ar, ref ep)) > 0)...
            }
            catch (System.ObjectDisposedException ode)
            {
                //MSDN NOTE: This occurs when the socket is closed.
                if (m_delegateOARE != null) m_delegateOARE(new NodeSocketInvalid("The Socket has been closed. ", ode));
            }
            catch (System.Net.Sockets.SocketException se)
            {
                //MSDN NOTE: If you receive a SocketException, use the SocketException.ErrorCode property to obtain the specific error code.
                // After you have obtained this code, refer to the Windows Sockets version 2 API error code documentation in the MSDN library
                //for a detailed description of the error.
                if (m_delegateOARE != null) m_delegateOARE(new NodeSocketInvalid(se.Message, se));
            }
            catch (Exception ex)
            {
                //Call the error callback if one exists.
                if (m_delegateOARE != null) m_delegateOARE(new NodeUnknownException(ex.Message, ex));
            }
        }

        /// Last Modified: 10/24/10
        /// <summary>
        /// This event is fired when an asynchronous send occurs.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="ar">
        /// An object that contains the send packet.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: 
        /// 1) The message will have been sent successfully.
        /// 2) An exception has been thrown.
        /// -----------------------------------------------------
        /// Exceptions:
        /// 1) NodeSocketInvalid
        /// 2) NodeUnknownException
        /// -----------------------------------------------------
        /// Return Value:
        protected virtual void OnSendCallback(IAsyncResult ar)
        {
            try
            {
                //Retrieve the stateobject.
                StateObject state = (StateObject)ar.AsyncState;

                //Retrieve the socket of the local server
                // and cast the endpoint.
                Socket socket = state.socket;
                EndPoint ep = (EndPoint)state.iep;

                //Retrive the remaining data.
                int read = 0;
                if ((read = socket.EndSendTo(ar)) > 0)
                {
                    //Call the callback method if it is valid.
                    if (m_delegateOAR != null)
                    {
                        Packet p = null;
                        m_delegateOAR(p);

                        if (p != null)
                        {
                            byte[] byData = p.ToByteArray();

                            try
                            {
                                //Perform any necessary decryptions based on the security model.
                                if (m_nodeSecurityModel != null)
                                    state.buffer = m_nodeSecurityModel.ByteEncrypt(byData);
                                else
                                    state.buffer = byData;
                            }
                            catch (System.Security.Cryptography.CryptographicException ce)
                            {
                                //Call the error callback if one exists.
                                if (m_delegateOARE != null) m_delegateOARE(new NodeSecurityInvalidKeyException("Invalid Key.", ce));
                            }

                            //Continue sending valid messages.
                            socket.BeginSendTo(state.buffer, 0, StateObject.BUFFER_SIZE,
                                SocketFlags.None, (EndPoint)state.iep,
                                new AsyncCallback(OnSendCallback), state);
                        }
                    }   //END OF if (m_delegateOAR != null)...

                }   //END OF if ((read = socket.EndSendTo(ar)) > 0)...
            }
            catch (System.ObjectDisposedException ode)
            {
                //MSDN NOTE: This occurs when the socket is closed.
                if (m_delegateOARE != null) m_delegateOARE(new NodeSocketInvalid("The Socket has been closed. ", ode));
            }
            catch (System.Net.Sockets.SocketException se)
            {
                //MSDN NOTE: If you receive a SocketException, use the SocketException.ErrorCode property to obtain the specific error code.
                // After you have obtained this code, refer to the Windows Sockets version 2 API error code documentation in the MSDN library
                //for a detailed description of the error.
                if (m_delegateOARE != null) m_delegateOARE(new NodeSocketInvalid(se.Message, se));
            }
            catch (Exception ex)
            {
                //Call the error callback if one exists.
                if (m_delegateOARE != null) m_delegateOARE(new NodeUnknownException(ex.Message, ex));
            }
        }

        #endregion Events

    }   //END OF CLASS
}   //END OF NAMESPACE
