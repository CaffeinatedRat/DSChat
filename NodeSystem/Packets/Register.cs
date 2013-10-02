using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///////////////////////////////
///        CUSTOM           ///
///////////////////////////////
using System.Xml;
using System.Net;

namespace NodeSystem.Packets
{
    public class Register : ObjectEx
    {
        #region Properties
        /// Last Modified: 12/5/09
        /// <summary>
        /// Maintains the node type.  This is a custom value
        ///  defined by the nodes.
        /// </summary>
        public String NodeType { get; set; }

        /// Last Modified: 11/29/09
        /// <summary>
        /// Maintains the port of the node attempting to register.
        /// </summary>
        public int Port { get; set; }

        /// Last Modified: 11/29/09
        /// <summary>
        /// Maintains the IPAddress of the node attempting
        ///  to register.
        /// </summary>
        public IPAddress IPAddress { get; set; }

        /// Last Modified: 11/29/09
        /// <summary>
        /// Maintains the internal message for the register message.
        /// </summary>
        public String InternalMessage { get; set; }

        #endregion Properties

        #region Methods

        /// Last Modified: 11/29/09
        /// <summary>
        /// Initializes the register message.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Review the parameters statement.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="ipAddress">
        /// Maintains the IP Address of the node attempting to register.
        /// </param>
        /// <param name="nPort">
        /// Maintains the port of the node attempting to register.
        /// </param>
        /// <param name="sInternalMsg">
        /// Contains an internal message carrying various other data.
        /// </param>
        /// <param name="sNodeType">
        /// Contains the node type.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public Register(IPAddress ipAddress, int nPort, String sInternalMsg, String sNodeType)
        {
            //Initialize the properties.
            IPAddress = ipAddress;
            Port = nPort;
            InternalMessage = sInternalMsg;
            NodeType = sNodeType;
        }

        /// Last Modified: 11/29/09
        /// <summary>
        /// Initialized the object based on the XML String provided.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Review the parameters statement.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="sXML">
        /// A string that contains the XML data.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public Register(String sXML) : base (sXML){}

        /// Last Modified: 11/29/09
        /// <summary>
        /// Initializes the object based on the serialized byte array.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Review the parameters statement.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="byArray">
        /// An array of bytes that is the serialized version of
        ///  the object.
        /// </param>
        /// <param name="nSize">
        /// An integer that represents the number of bytes in the
        ///  array to use.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public Register(Byte[] byArray, int nSize) : base(byArray, nSize) { }

        /// Last Modified: 11/29/09
        /// <summary>
        /// Converts this object into a XML String form.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: Refer to the return statement.
        /// -----------------------------------------------------
        /// Return Value:
        /// String -- Returns the object in XML String form.
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<REGISTER IPADDRESS=\"");
            sb.Append(IPAddress.ToString());
            sb.Append("\" PORT=\"");
            sb.Append(Port.ToString());
            sb.Append("\" NODETYPE=\"");
            sb.Append(NodeType);
            sb.Append("\">");
            sb.Append("<MESSAGE>");
            sb.Append(InternalMessage);
            sb.Append("</MESSAGE>");
            sb.Append("</REGISTER>");

            return sb.ToString();
        }

        /// Last Modified: 11/29/09
        /// <summary>
        /// Decodes the XML String into an object instance
        ///  of this class.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Review the parameters statement.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="sXML">
        /// A string that contains the XML data.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public override void ToObject(String sXML)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(sXML);

            XmlElement eRoot = (XmlElement)xmlDoc.GetElementsByTagName("REGISTER").Item(0);
            IPAddress = IPAddress.Parse(eRoot.GetAttribute("IPADDRESS"));
            Port = Convert.ToInt32(eRoot.GetAttribute("PORT"));
            NodeType = eRoot.GetAttribute("NODETYPE");

            XmlElement eMsg = (XmlElement)eRoot.GetElementsByTagName("MESSAGE").Item(0);
            InternalMessage = eMsg.InnerXml;
        }

        /// Last Modified: 11/29/09
        /// <summary>
        /// Determines if the XML String is this object in XML form.
        /// Returns true if XML is this object.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Review the parameters statement.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="sXML">
        /// A string that contains the XML data.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: Refer to the return statement.
        /// -----------------------------------------------------
        /// Return Value:
        /// bool -- Returns true if the XML supplied is this
        ///  object in XML form.
        public override bool IsXmlObject(String sXML)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(sXML);
            return (xmlDoc.GetElementsByTagName("REGISTER").Item(0) != null);
        }

        /// Method: IsRegister
        /// Last Modified: 11/29/09
        /// <summary>
        /// Determines if the XML String is the register object
        ///  in XML form.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Review the parameters statement.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="sXML">
        /// A string that contains the XML data.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: Refer to the return statement.
        /// -----------------------------------------------------
        /// Return Value:
        /// bool -- Returns true if the XML supplied is this
        ///  object in XML form.
        public static bool IsRegister(String sXML)
        {
            Register r = new Register(null, -1, "", "");
            return r.IsXmlObject(sXML);
        }

        #endregion Methods

    }   //END OF CLASS
}   //END OF NAMESPACE
