using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

///////////////////////////////
///        CUSTOM           ///
///////////////////////////////
using NodeSystem.Packets;
using NodeSystem.ServiceLayer;
using NodeSystem.Interfaces;
using NodeSystem.Exceptions;
using System.Net;

namespace NodeSystem
{
    /// <summary>
    /// A class that manages the child node, which is one of many nodes that connnect to the distributed node network.
    /// All children communicate with the parent in order to relay information to other children within the network.
    /// </summary>
    public class ChildNode : RootNode
    {
        #region MemberVars

        /// Maintains the endpoint of the parent node.
        /// This value will remain null until it is registered with the parent.
        protected IPEndPoint m_iepParentNode = null;

        #endregion MemberVars

        #region Properties

        /// Last Modified: 9/11/10
        /// <summary>
        /// Desc: Maintains then name of the node.
        /// </summary>
        public string NodeName { get; set; }

        #endregion Properties

        #region Methods

        /// Last Modified: 10/17/10
        /// <summary>
        /// Initializes the child node to run on a specific port.
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
        public ChildNode(int nHostPort, INodeSecurityModel nodeSecurityModel)
            : base(nHostPort, nodeSecurityModel) {}

        /// Last Modified: 10/17/10
        /// <summary>
        /// Initializes the child node to run on a specific port without a security model.
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
        public ChildNode(int nHostPort) : this(nHostPort, null) { }

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
        public ChildNode() { }

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
        public new static string GetNodeType()
        {
            return "CN";
        }

        /// Last Modified: 10/24/10
        /// <summary>
        /// Closes the child node by shutting down the local server
        ///  and unregistering with parent node.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public override void Close()
        {
            //Unregister the node and ignore any exceptions.
            try
            {
                UnregisterWithParent();
            }
            catch (NodeException) { }

            base.Close();
        }

        /// Last Modified: 10/4/09
        /// <summary>
        /// Sends a message to the parent node.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="sEntityId">
        /// The name or identity of the entity of the entity to send the message to.
        /// </param>
        /// <param name="sMsg">
        /// A string that contains the message to send.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: Refer to the return statement.
        /// -----------------------------------------------------
        /// Return Value:
        /// bool -- Returns true if successful.
        public override bool SendMsg(string sEntityId, string sMsg)
        {
            //Create the packet.
            Packet p = new Packet(
                Packet.EventTag.Message,   //Protocol
                NodeName,                   //SrcLink
                sEntityId,                  //DestLink
                sMsg,
                LogicalTime);               //Logical Time.

            return SendToParent(p);
        }

        /// Last Modified: 11/29/09
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="o">
        /// An object to send.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: Refer to the return statement.
        /// -----------------------------------------------------
        /// Return Value:
        /// bool -- Returns true if successful.
        public override bool SendMsg(object o)
        {
            throw new NotImplementedException();
        }

        /// Last Modified: 9/18/10
        /// <summary>
        /// Sends a message to the parent node.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="p">
        /// A packet that contains information about the event.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: Refer to the return statement.
        /// -----------------------------------------------------
        /// Exceptions:
        /// 1) NodeSocketInvalid
        /// 2) NodeUnknownException
        /// -----------------------------------------------------
        /// Return Value:
        /// bool -- Returns true if successful.
        private bool SendToParent(Packet p)
        {
            //Only send if the parent node is valid.
            if (m_iepParentNode != null)
            {
                //Send the packet synchronously.
                SendTo(p, m_iepParentNode);

                //Update the clock.
                Tick();

                //Successful.
                return true;
            }   //END OF if (m_iepParentNode != null)...

            //The send operation failed.
            return false;
        }

        /// Last Modified: 10/24/10
        /// <summary>
        /// Attempts to register the child node with the parent.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="ipAddr">
        /// Contains the IPAddress of the parent.
        /// </param>
        /// <param name="nPort">
        /// Contains the port of the parent.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Exceptions:
        /// 1) NodeSocketInvalid
        /// 2) NodeUnknownException
        /// -----------------------------------------------------
        /// Return Value:
        private void RegisterWithParent(IPAddress ipAddr, int nPort)
        {
            //Create the parent endpoint.
            m_iepParentNode = new IPEndPoint(ipAddr, nPort);

            Register r = new Register(NodeSocket.GetLocalIP(), HostPort, "", GetNodeType());

            //Create a new packet to send to the parent.
            Packet p = new Packet(
                Packet.EventTag.Register,  //Protocol
                NodeName,                  //SrcName -- Unique name to register the node with.
                ParentNode.GetNodeType(),  //DestName -- Not used in registration.
                r.ToString(),              //Msg
                LogicalTime);              //Logical Time.

            //Send the packet synchronously.
            SendToParent(p);
        }

        /// Last Modified: 10/24/10
        /// <summary>
        /// Attempts to unregister the child with the parent.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: The child must be registered with the parent.
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Exceptions:
        /// 1) NodeSocketInvalid
        /// 2) NodeUnknownException
        /// -----------------------------------------------------
        /// Return Value:
        private void UnregisterWithParent()
        {
            //Only attempt to unregister the child if the parent is valid.
            if (m_iepParentNode != null)
            {
                Register r = new Register(NodeSocket.GetLocalIP(), HostPort, "", GetNodeType());
                
                //Create a new packet to send to the parent.
                Packet p = new Packet(
                    Packet.EventTag.Unregister, //Protocol
                    NodeName,                   //SrcName -- Unique name to un-register the node with.
                    ParentNode.GetNodeType(),   //DestName -- Not used in registration.
                    r.ToString(),               //Msg
                    LogicalTime);               //Logical Time.

                //Send the packet synchronously.
                SendToParent(p);
            }   //END OF if (m_iepParentNode != null)...
        }

        #endregion Methods

        #region Events

        /// Last Modified: 10/24/10
        /// <summary>
        /// This event is fired immediately after the local server is created.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="ipAddr">
        /// The IPAddress of the parent node.
        /// </param>
        /// <param name="nPort">
        /// The port of the parent node.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        protected override void OnInit(IPAddress ipAddr, int nPort)
        {
            try
            {
                RegisterWithParent(ipAddr, nPort);
            }
            catch (NodeException ne)
            {
                Close();
                OnNodeEvent(NodeEvent.NETWORK_ERROR, GetNodeType(), ne.Message);
            }
        }

        /// Last Modified: 9/15/10
        /// <summary>
        /// This event occurs when the child node has been registered
        ///  with the parent.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="p">
        /// A packet that contains information about the event.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        protected override void OnRegistered(Packet p)
        {
            OnNodeEvent(NodeEvent.REGISTRATION_SUCCESSFUL, ParentNode.GetNodeType(), p.Msg);
        }

        /// Last Modified: 9/15/10
        /// <summary>
        /// This event occurs when the child cannot register with the parent node.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="p">
        /// A packet that contains information about the event.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        protected override void OnRegistrationFailure(Packet p)
        {
            NodeEvent ne = (NodeEvent)Enum.Parse(typeof(NodeEvent), p.Msg);
            if (ne == NodeEvent.ALREADY_REGISTERED)
            {
                //Clear the parent during registration failure.
                m_iepParentNode = null;

                OnNodeEvent(NodeEvent.REGISTRATION_FAILURE, ParentNode.GetNodeType(), "");
            }
        }

        /// Last Modified: 9/15/10
        /// <summary>
        /// This event occurs when the parent notifies the child
        ///  that a new node has been added.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="p">
        /// A packet that contains information about the event.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        protected override void OnNodeAdded(Packet p)
        {
            OnNodeEvent(NodeEvent.NODE_ADDED, ParentNode.GetNodeType(), p.Msg);
        }

        /// Last Modified: 9/15/10
        /// <summary>
        /// This event occurs when the parent notifies the child
        ///  that a node has been removed.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="p">
        /// A packet that contains information about the event.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        protected override void OnNodeRemoved(Packet p)
        {
            OnNodeEvent(NodeEvent.NODE_REMOVED, ParentNode.GetNodeType(), p.Msg);
        }

        /// Last Modified: 11/28/09
        /// <summary>
        /// This event occurs when the child node receives a message.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="p">
        /// A packet that contains information about the event.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        protected override void OnMessage(Packet p)
        {
            OnNodeEvent(NodeEvent.MESSAGE, p.SourceName, p.Msg);
        }

        /// Last Modified: 11/28/09
        /// <summary>
        /// No longer in use.
        /// NOTE: TO use to notify the user that the message sent
        ///  did not reach its target.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="p">
        /// A packet that contains information about the event.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        protected override void OnInvalidAddress(Packet p)
        {
            //The address doesn't exist or the user has disconnected but the parent
            // is unaware of the situation.

            //Update the Status.
            OnNodeEvent(NodeEvent.INVALID_NODE, ParentNode.GetNodeType(), p.Msg);
        }

        /// Last Modified: 11/28/09
        /// <summary>
        /// This event is triggered when the parent has shutdown.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        protected override void OnShutdown()
        {
            //Release the parent information.
            m_iepParentNode = null;

            //Update the Status.
            OnNodeEvent(NodeEvent.PARENT_SHUTDOWN, ParentNode.GetNodeType(), "");
        }

        /// Last Modified: 10/24/10
        /// <summary>
        /// This event occurs when a packet receive error occurs.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        protected override void OnPacketReceiveError(Exception ex)
        {
            //Close on node exceptions.
            if (ex.GetType() == typeof(NodeException))
            {
                //The network error is severe enough to close the connection.
                Close();
                OnNodeEvent(NodeEvent.SEVERE_NETWORK_ERRROR, GetNodeType(), ex.Message);
            }

            //Update listeners of the node event.
            OnNodeEvent(NodeEvent.NETWORK_ERROR, GetNodeType(), ex.Message);
        }

        #endregion Events

    }   //END OF CLASS
}   //END OF NAMESPACE
