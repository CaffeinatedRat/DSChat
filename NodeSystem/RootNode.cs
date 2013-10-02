using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///////////////////////////////
///        CUSTOM           ///
///////////////////////////////
using System.Net;
using System.IO;
using System.Threading;
using NodeSystem.ServiceLayer;
using NodeSystem.Packets;
using NodeSystem.Interfaces;

namespace NodeSystem
{
    /// <summary>
    /// A class that manages the basic infrastructure of the distributed node network.
    /// </summary>
    public abstract class RootNode
    {
        #region DataTypes

        public enum NodeEvent
        {
            REGISTERED,
            REGISTRATION_SUCCESSFUL,
            REGISTRATION_FAILURE,
            ALREADY_REGISTERED,
            UNREGISTERED,
            NODE_ADDED,
            NODE_REMOVED,
            NETWORK_ERROR,
            SEVERE_NETWORK_ERRROR,
            PARENT_SHUTDOWN,
            INVALID_NODE,
            MESSAGE
        };

        #endregion DataTypes

        #region Delegates

        /// Callback used to handle events.
        public delegate void OnNodeEventDelegate(NodeEvent nodeEvent, string sEntityId, string sData);

        #endregion Delegates

        #region MemberVars

        /// Maintains the main socket connection for
        ///  the node to act as a server.
        protected NodeSocket m_nodeSocket = null;

        /// Maintains the logical clock, initialized
        ///  with a value of zero.
        private long m_lLogicalTime = 0L;

        /// Maintains a mutex for protecting the logical clock.
        private Mutex m_mutexLogicalTick = new Mutex();

        /// Maintains the set status callback.
        //private OnSetStatusDelegate m_dOnSetStatusDelegate = null;

        /// Maintains the registered callback.
        private OnNodeEventDelegate m_dOnNodeEvent = null;

        #endregion MemberVars

        #region Properties

        /// Last Modified: 11/28/09
        /// <summary>
        /// Returns the port of the host.
        /// </summary>
        public int HostPort { get; set; }

        /// Last Modified: 11/28/09
        /// <summary>
        /// Returns the logical time.
        /// </summary>
        public long LogicalTime
        {
            get { return m_lLogicalTime; }
        }

        /// Last Modified: 9/18/10
        /// <summary>
        /// A callback that is triggered when a node event has occurred.
        /// </summary>
        public OnNodeEventDelegate NodeEventCallback
        {
            set { m_dOnNodeEvent = value; }
        }

        #endregion Properties

        #region Methods

        /// Last Modified: 10/17/10
        /// <summary>
        /// Initializes the object with a port to host on, as well as the specific security model to use.
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
        public RootNode(int nHostPort, INodeSecurityModel nodeSecurityModel)
        {
            //Create the new node socket and host it at the supplied port.
            m_nodeSocket = new NodeSocket(nHostPort, nodeSecurityModel);
            m_nodeSocket.SetOnAsynchReceive = OnPacketReceive;
            m_nodeSocket.SetOnAsynchReceiveError = OnPacketReceiveError;

            HostPort = nHostPort;
        }

        /// Last Modified: 10/17/10
        /// <summary>
        /// Initializes the object with a port to host on without a security model.
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
        public RootNode(int nHostPort) : this (nHostPort, null){}

        /// Last Modified: 11/29/09
        /// <summary>
        /// Initializes the object, which will not be hosted.
        /// This is for nodes that do not receive information.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public RootNode() {}

        /// Last Modified: 10/4/09
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
        ~RootNode()
        {
            //Just close the socket, do not call Close() or this will
            // invoke the close method of either the parent or child node.
            if (m_nodeSocket != null)
                m_nodeSocket.Close();
        }

        /// Last Modified: 12/5/09
        /// <summary>
        /// Return the node type.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions.
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: Refer to the return statement.
        /// -----------------------------------------------------
        /// Return Value:
        /// string -- Returns the name of the node type.
        public static string GetNodeType()
        {
            return "ROOTNODE";
        }

        /// Last Modified: 12/5/09
        /// <summary>
        /// Increments the logical clock.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions.
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: One tick will have passed.
        /// -----------------------------------------------------
        /// Return Value:
        public void Tick()
        {
            //Protect the clock.
            m_mutexLogicalTick.WaitOne();
            m_lLogicalTime++;
            m_mutexLogicalTick.ReleaseMutex();
        }

        /// Last Modified: 12/7/09
        /// <summary>
        /// Increments the logical by a specific value.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="lTicks">
        /// The number of ticks to increment the logical clock by.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: Logical clock will have advanced
        ///  by a specific number of ticks.
        /// -----------------------------------------------------
        /// Return Value:
        public void Tick(long lTicks)
        {
            //Protect the clock.
            m_mutexLogicalTick.WaitOne();
            m_lLogicalTime += lTicks;
            m_mutexLogicalTick.ReleaseMutex();
        }

        /// Last Modified: 10/24/10
        /// <summary>
        /// Starts the local server on the node by registering
        ///  it with the parent node.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments:
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="ipAddr">
        /// Contains the IPAddress of the parent to register with.
        /// </param>
        /// <param name="nPort">
        /// Contains the host of the parent node.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        public virtual void Start(IPAddress ipAddr, int nPort)
        {
            //Start receiving events immediately.
            //NOTE: An invalid node socket is used by a parent or the server.
            if (m_nodeSocket != null)
            {
                m_nodeSocket.AsynchReceive();
                OnInit(ipAddr, nPort);
            }
        }

        /// Last Modified: 10/24/10
        /// <summary>
        /// Starts the local server on the node by registering
        ///  it with the parent node.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments:
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public virtual void Start()
        {
            //Register the node with the parent.
            Start(null, 0);
        }

        /// Last Modified: 9/13/10
        /// <summary>
        /// Explicitly close the node and the socket.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: The node's socket should be opened, but
        ///  can be null as well.
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: Close's the socket's node, where
        ///  the socket has to be restarted.
        /// -----------------------------------------------------
        /// Return Value:
        public virtual void Close()
        {
            if (m_nodeSocket != null)
            {
                m_nodeSocket.Close();
                m_nodeSocket = null;
            }
        }

        /// Last Modified: 11/28/09
        /// <summary>
        /// Sends a packet to the supplied IPEndPoint.
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
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Exceptions:
        /// 1) NodeSocketInvalid
        /// 2) NodeUnknownException
        /// -----------------------------------------------------
        /// Return Value:
        public void SendTo(Packet p, IPEndPoint ipep)
        {
            if (m_nodeSocket != null)
                m_nodeSocket.SendTo(p, ipep);
        }

        #region SAFE_CallbackMethods

        /// Last Modified: 9/18/10
        /// <summary>
        /// Allows for safe access of the OnNodeEvent callback method.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments:
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="nodeEvent">
        /// The 
        /// </param>
        /// <param name="sEntityId">
        /// The name or id of the entity generating the event.
        /// </param>
        /// <param name="sData">
        /// A string that describes the data associated with the event.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        protected void OnNodeEvent(NodeEvent nodeEvent, string sEntityId, string sData)
        {
            if (m_dOnNodeEvent != null) m_dOnNodeEvent(nodeEvent, sEntityId, sData);
        }

        #endregion SAFE_CallbackMethods

        #endregion Methods

        #region AbstractMethods

        public abstract bool SendMsg(string sEntityId, string sMsg);
        public abstract bool SendMsg(object obj);

        #endregion AbstractMethods

        #region Events

        /// Last Modified: 11/28/09
        /// <summary>
        /// This event is triggered when a packet is begin received.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments:
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="p">
        /// A packet that contains the information being sent.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        protected virtual void OnPacketReceive(Packet p)
        {
            //Obtain the local time.
            if (m_lLogicalTime < p.LogicalTime)
                m_lLogicalTime = p.LogicalTime;

            //Trigger various events.
            if (p.Tag == Packet.EventTag.Register)
                OnRequestToRegister(p);
            else if (p.Tag == Packet.EventTag.RegistrationFailure)
                OnRegistrationFailure(p);
            else if (p.Tag == Packet.EventTag.Registered)
                OnRegistered(p);
            else if (p.Tag == Packet.EventTag.Message)
                OnMessage(p);
            else if (p.Tag == Packet.EventTag.InvalidAddress)
                OnInvalidAddress(p);
            else if (p.Tag == Packet.EventTag.Shutdown)
                OnShutdown();
            else if (p.Tag == Packet.EventTag.Unregister)
                OnRequestToUnregister(p);
            else if (p.Tag == Packet.EventTag.NodeAdded)
                OnNodeAdded(p);
            else if (p.Tag == Packet.EventTag.NodeRemoved)
                OnNodeRemoved(p);
        }

        #region EventsToOverride

        protected virtual bool OnRequestToRegister(Packet p) { return false;  }
        protected virtual void OnRegistrationFailure(Packet p) { }
        protected virtual void OnRequestToUnregister(Packet p) { }
        protected virtual void OnMessage(Packet p) { }
        protected virtual void OnRegistered(Packet p) { }
        protected virtual void OnInvalidAddress(Packet p) { }
        protected virtual void OnShutdown() { }
        protected virtual void OnNodeAdded(Packet p) { }
        protected virtual void OnNodeRemoved(Packet p) { }

        #endregion EventsToOverride

        #region AbstractEvents

        protected abstract void OnInit(IPAddress ipAddr, int nPort);
        protected abstract void OnPacketReceiveError(Exception ex);

        #endregion AbstractEvents

        #endregion Events

    }   //END OF CLASS
}   //END OF NAMESPACE
