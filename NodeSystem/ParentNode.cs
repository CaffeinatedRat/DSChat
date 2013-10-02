using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    /// A class that manages the parent node, which is the centeralized element in the distributed node network.
    /// All children communicate with the parent in order to relay information to other children within the network.
    /// </summary>
    public class ParentNode : RootNode
    {
        #region MemberVars

        /// Maintains a table of children node endpoints. 
        //private IPEndPoint[] m_iepRoutingTable = new IPEndPoint[8];
        private Dictionary<string, IPEndPoint> m_iepRoutingTable = null;
        
        #endregion MemberVars

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
        public ParentNode(int nHostPort, INodeSecurityModel nodeSecurityModel)
            : base(nHostPort, nodeSecurityModel)
        {
            m_iepRoutingTable = new Dictionary<string, IPEndPoint>();
        }

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
        public ParentNode(int nHostPort) : this(nHostPort, null) {}

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
            return "PN";
        }

        /// Last Modified: 10/24/10
        /// <summary>
        /// Closes the parent node by shutting down the local
        ///  server and notifying all children of the event.
        /// All cases: O(n) -- Where n is size of the routing table.
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
            //Iterate through all endpoints in the table.
            //Time Complexity:
            //All cases: O(n) -- Where n is size of the routing table.
            //foreach (IPEndPoint iep in m_iepRoutingTable)
            foreach(KeyValuePair<string, IPEndPoint> kvp in m_iepRoutingTable)
            {
                //Create the datapacket to notify the child of registration.
                Packet p = new Packet(
                    Packet.EventTag.Shutdown,  //Protocol
                    "Goodbye",                 //Msg
                    LogicalTime);              //LogicalTime.

                //Notify the children the parent is shutting down.
                //NOTE: Send the packet synchronously.
                
                //Ignore all exceptions, since we are closing this node anyways.
                try
                {
                    SendTo(p, kvp.Value);
                }
                catch (NodeException) { }
            }   //END OF foreach (IPEndPoint iep in m_iepRoutingTable)...

            base.Close();
        }

        /// Last Modified: 10/4/09
        /// <summary>
        /// Not implemented.
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
            throw new NotImplementedException();
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
        /// Attempts to notify the child node of registration,
        ///  whether it was successful or not.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="ipAddr">
        /// Contains the IPAddress of the child.
        /// </param>
        /// <param name="nPort">
        /// Contains the port of the child.
        /// </param>
        /// <param name="sName">
        /// Contains the name of the child node to notify of registration.
        /// </param>
        /// <param name="bSuccessful">
        /// Is true if registration was successful.
        /// </param>
        /// <param name="sData">
        /// Contains the status of registration.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Exceptions:
        /// 1) NodeSocketInvalid
        /// 2) NodeUnknownException
        /// 3) Exception
        /// -----------------------------------------------------
        /// Return Value:
        private void NotifyChildOfRegistration(IPAddress ipAddr,
            int nPort,
            string sName,
            bool bSuccessful,
            string sData)
        {
            //Create the datapacket to notify the child of registration.
            Packet p = new Packet(
                bSuccessful ? Packet.EventTag.Registered : Packet.EventTag.RegistrationFailure,    //Protocol
                GetNodeType(),   //SrcLink
                sName,           //DestLink
                sData,           //Msg
                LogicalTime);    //LogicalTime.

            //Notify the children of a successful registration.
            //NOTE: Send the packet synchronously.
            SendTo(p, new IPEndPoint(ipAddr, nPort));

            //Update the clock.
            Tick();
        }

        /// Last Modified: 9/18/10
        /// <summary>
        /// Notifies all of the children about the new node that
        ///  has been added.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="sNameOfNode">
        /// Contains the name of the node being manipulated.
        /// </param>
        /// <param name="bNodeAdded">
        /// If true, notifies all children that the node is being added
        ///  otherwise it is being removed.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: All children will have been notified of the node event.
        /// -----------------------------------------------------
        /// Exceptions:
        /// 1) NodeSocketInvalid
        /// 2) NodeUnknownException
        /// 3) Exception
        /// -----------------------------------------------------
        /// Return Value:
        private void NotifyAllChildrenOfNodeEvent(string sNameOfNode, bool bNodeAdded)
        {
            foreach (KeyValuePair<string, IPEndPoint> kvp in m_iepRoutingTable)
            {
                if (kvp.Key != sNameOfNode)
                {
                    //Create the datapacket to notify the child of registration.
                    Packet p = new Packet(
                        bNodeAdded ? Packet.EventTag.NodeAdded : Packet.EventTag.NodeRemoved,  //Protocol
                        GetNodeType(),           //SrcLink
                        kvp.Key,                 //DestLink
                        sNameOfNode,             //Msg
                        LogicalTime);            //LogicalTime.

                    //Notify all children of the new node.
                    //NOTE: Send the packet synchronously.
                    SendTo(p, kvp.Value);

                    //Update the clock.
                    Tick();
                }   //END OF if (kvp.Key != sNameOfNode)...
            }   //END OF foreach (KeyValuePair<string, IPEndPoint> kvp in m_iepRoutingTable)...
        }

        /// Last Modified: 9/18/10
        /// <summary>
        /// Register the endpoint in the routing table and returns
        ///  the link associated with the endpoint.
        /// Time Complexity:
        /// 1) Bestcase\Averagecase: O(1) -- O(n) 
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="sNameOfChild">
        /// Retains the name of the child to register.
        /// </param>
        /// <param name="iepTarget">
        /// An endpoint to register.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: If successful, the endpoint will have
        ///  been registered.
        /// -----------------------------------------------------
        /// Return Value:
        /// bool -- Returns true if the endpoint was registered, else
        ///  false is returned if the endpoint already exists.
        private bool RegisterEndPoint(string sNameOfChild, IPEndPoint iepTarget)
        {
            //Register the new endpoint if it doesn't already exist.
            if (!m_iepRoutingTable.ContainsKey(sNameOfChild))
            {
                m_iepRoutingTable.Add(sNameOfChild, iepTarget);
                return true;
            }

            //The endpoint could not be registered.
            return false;
        }

        /// Last Modified: 9/18/10
        /// <summary>
        /// Removes the endpoint from the dictionary based on the
        ///  name of the child supplied.
        /// Time Complexity:
        /// 1) [All cases]: O(1) -- O(n) 
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="sNameOfChildNode">
        /// Contains the name of the child node to unregister.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: The endpoint of the childnode will be removed.
        /// -----------------------------------------------------
        /// Return Value:
        /// bool -- Returns true if the child node was successfully unregistered.
        private bool UnregisterEndPoint(string sNameOfChildNode)
        {
            return m_iepRoutingTable.Remove(sNameOfChildNode);
        }

        #endregion Methods

        #region Events

        /// Last Modified: 10/4/09
        /// <summary>
        /// This event is fired immediately after the local server is created.
        /// NOTE USED IN THIS INSTANCE.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="ipAddr">
        /// The IPAddress.
        /// </param>
        /// <param name="nPort">
        /// The port number.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        protected override void OnInit(IPAddress ipAddr, int nPort)
        {
            //Do nothing.
        }

        /// Last Modified: 10/24/10
        /// <summary>
        /// This event is triggered when a child is requesting
        ///  to register with the parent node.
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
        /// <returns>
        /// Returns true if the child could be registered.
        /// </returns>
        protected override bool OnRequestToRegister(Packet p)
        {
            //Obtain the register message.
            Register r = new Register(p.Msg);

            //Generate message to display.
            string sMsg = r.IPAddress.ToString() + ":" + r.Port.ToString();

            try
            {
                //Update the states based on the link.
                if (RegisterEndPoint(p.SourceName, new IPEndPoint(r.IPAddress, r.Port)))
                {
                    //temporarily package all of the user's into a tilde delimited string and attach
                    // it to the message.
                    StringBuilder sb = new StringBuilder();
                    foreach (KeyValuePair<string, IPEndPoint> kvp in m_iepRoutingTable)
                        if (kvp.Key != p.SourceName)
                            sb.Append(((sb.Length > 0) ? "~" : "") + kvp.Key);

                    //Notify the child of registration.
                    NotifyChildOfRegistration(r.IPAddress, r.Port, p.SourceName, true, sb.ToString());

                    //Notify all children of the new node.
                    NotifyAllChildrenOfNodeEvent(p.SourceName, true);

                    //Update listeners of the node event.
                    OnNodeEvent(NodeEvent.REGISTERED, p.SourceName, sMsg);
                    return true;
                }
                else
                {
                    //Notify the child of the failure.
                    NotifyChildOfRegistration(r.IPAddress,
                        r.Port,
                        p.SourceName,
                        false,
                        NodeEvent.ALREADY_REGISTERED.ToString());

                    //Update listeners of the node event.
                    OnNodeEvent(NodeEvent.ALREADY_REGISTERED, p.SourceName, sMsg);
                }
                //END OF if (RegisterEndPoint(p.SourceName, new IPEndPoint(r.IPAddress, r.Port)))...
            }
            catch (NodeException ne)
            {
                Close();
                OnNodeEvent(NodeEvent.NETWORK_ERROR, GetNodeType(), ne.Message);
            }

            //All other conditions are a failure.
            return false;
        }

        /// Last Modified: 10/24/10
        /// <summary>
        /// This event is triggered when the child is requesting
        ///  to unregister with the parent.
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
        protected override void OnRequestToUnregister(Packet p)
        {
            //Obtain the register message.
            Register r = new Register(p.Msg);
           
            //Unregister the child node and obtain the link.
            UnregisterEndPoint(p.SourceName);

            try
            {
                //Notify all children of the new node.
                NotifyAllChildrenOfNodeEvent(p.SourceName, false);

                //Update listeners of the node event.
                OnNodeEvent(NodeEvent.UNREGISTERED, p.SourceName, "");
            }
            catch (NodeException ne)
            {
                Close();
                OnNodeEvent(NodeEvent.NETWORK_ERROR, GetNodeType(), ne.Message);
            }
        }

        /// Last Modified: 10/24/10
        /// <summary>
        /// This event occurs when the parent node receives a message.
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
            //Default to the endpoint to null.
            IPEndPoint ipepRedirect = null;

            //Verify that the destination is valid.
            if (m_iepRoutingTable.ContainsKey(p.DestinationName))
            {
                ipepRedirect = m_iepRoutingTable[p.DestinationName];
            }
            //Otherwise bounce back.
            else
            {
                //Get the source IEP.
                ipepRedirect = m_iepRoutingTable[p.SourceName];

                //Change the type of tag
                p.Tag = Packet.EventTag.InvalidAddress;
                p.Msg = "Destination " + p.DestinationName + " does not exist.";

                //Bounce the packet back.
                p.InvertPath();

            }   //END OF if (epRedirect != null)...

            try
            {
                //Send the packet synchronously.
                SendTo(p, ipepRedirect);
            }
            catch (NodeException ne)
            {
                Close();
                OnNodeEvent(NodeEvent.NETWORK_ERROR, GetNodeType(), ne.Message);
            }
        }

        /// Last Modified: 10/24/10
        /// <summary>
        /// This event is triggered when ever there is an network error.
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
