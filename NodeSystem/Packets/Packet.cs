using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///////////////////////////////
///        CUSTOM           ///
///////////////////////////////
using System.Xml;
using Common;

namespace NodeSystem.Packets
{
    public class Packet : ObjectEx
    {
        /// Last Modified: 9/15/10
        /// <summary>
        /// Provides an enumeration of a tagprotocol, or an event
        ///  identity for the packet being sent.
        /// </summary>
        public enum EventTag
        {
            Register,
            RegistrationFailure,
            Registered,
            Unregister,
            Shutdown,
            Message,
            InvalidAddress,
            NodeAdded,
            NodeRemoved,
            Ping
        };

        #region MemberVars

        /// Maintains the packet's unique identifier.
        private long m_lPacketId = 0L;

        #endregion MemberVars

        #region Properties

        /// Last Modified: 10/10/09
        /// <summary>
        /// Maintains the packet's tag.
        /// </summary>
        public EventTag Tag { get; set; }

        /// Last Modified: 10/10/09
        /// <summary>
        /// Maintains the packet's message.
        /// </summary>
        public string Msg { get; set; }

        /// Last Modified: 10/10/09
        /// <summary>
        /// Maintains the packet's creation date.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// Last Modified: 10/10/09
        /// <summary>
        /// Maintains the logical time of the host that sent
        ///  the information.
        /// </summary>
        public long LogicalTime { get; set; }

        /// Last Modified: 10/10/09
        /// <summary>
        /// The name of the source.
        /// </summary>
        public string SourceName { get; set; }

        /// Last Modified: 10/10/09
        /// <summary>
        /// The name of the destination.
        /// </summary>
        public string DestinationName { get; set; }

        /// Last Modified: 10/17/09
        /// <summary>
        /// Maintains the unique id of the packet.
        /// </summary>
        public long PacketId 
        {
            get { return m_lPacketId; }
        }

        #endregion Properties

        #region Methods

        /// Last Modified: 10/10/09
        /// <summary>
        /// Initializes the packet.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Review the parameters statement.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="tag">
        /// Contains the protocol tag of the packet.
        /// </param>
        /// <param name="Src">
        /// The name of the source host.
        /// </param>
        /// <param name="Dest">
        /// The name of the destination host.
        /// </param>
        /// <param name="sMsg">
        /// Contains the message of the packet.
        /// </param>
        /// <param name="lLogicalTime">
        /// Contains the current logical time for the host.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public Packet(EventTag tag,
            string Src,
            string Dest, 
            string sMsg,
            long lLogicalTime)
        {
            //Initialize the properties.
            Tag = tag;
            CreationDate = DateTime.Now;
            Msg = sMsg;
            LogicalTime = lLogicalTime;

            SourceName = Src;
            DestinationName = Dest;

            //Generate the packet id. 
            m_lPacketId = Math.Abs(DateTime.Now.GetHashCode());
        }

        /// Last Modified: 10/10/09
        /// <summary>
        /// Initializes the packet.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Review the parameters statement.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="tag">
        /// Contains the protocol tag of the packet.
        /// </param>
        /// <param name="sMsg">
        /// Contains the message of the packet.
        /// </param>
        /// <param name="lLogicalTime">
        /// Contains the current logical time for the host.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public Packet(EventTag tag, string sMsg, long lLogicalTime)
        {
            //Initialize the properties.
            Tag = tag;
            CreationDate = DateTime.Now;
            Msg = sMsg;
            LogicalTime = lLogicalTime;
            SourceName = DestinationName = "";

            //Generate the packet id. 
            m_lPacketId = Math.Abs(DateTime.Now.GetHashCode());
        }

        /// Last Modified: 11/28/09
        /// <summary>
        /// Initializes the packet with another packet.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Review the parameters statement.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="p">
        /// Contains a packet to copy.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public Packet(Packet p)
        {
            //Initialize the properties.
            Tag = p.Tag;
            CreationDate = p.CreationDate;
            Msg = p.Msg;
            LogicalTime = p.LogicalTime;
            m_lPacketId = p.PacketId;
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
        public Packet(String sXML) : base (sXML){}

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
        public Packet(Byte[] byArray, int nSize) : base (byArray, nSize){}

        /// Last Modified: 10/25/10
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
        /// string -- Returns the object in XML String form.
        public override string ToString()
        {
            //Transform all HTML tags.
            string transformedMsg = Msg.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
            //transformedMsg = transformedMsg.Replace("\"", "&quot;").Replace("'", "&apos;");

            StringBuilder sb = new StringBuilder();
            sb.Append("<PACKET ID=\"");
            sb.Append(PacketId.ToString());
            sb.Append("\" REALTIME=\"");
            sb.Append(CreationDate.ToString());
            sb.Append("\" LOGICALTIME=\"");
            sb.Append(LogicalTime.ToString());
            sb.Append("\" SRC=\"");
            sb.Append(SourceName);
            sb.Append("\" DEST=\"");
            sb.Append(DestinationName);
            sb.Append("\">");
            sb.Append("<TAG>");
            sb.Append(Tag.ToString());
            sb.Append("</TAG>");
            sb.Append("<MESSAGE>");
            sb.Append(transformedMsg);
            sb.Append("</MESSAGE>");
            sb.Append("</PACKET>");

            return sb.ToString();
        }

        /// Last Modified: 10/25/10
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
        public override void ToObject(string sXML)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(sXML);

            XmlElement eRoot = (XmlElement)xmlDoc.GetElementsByTagName("PACKET").Item(0);
            CreationDate = Convert.ToDateTime(eRoot.GetAttribute("REALTIME"));
            m_lPacketId = Convert.ToInt64(eRoot.GetAttribute("ID"));
            LogicalTime = Convert.ToInt64(eRoot.GetAttribute("LOGICALTIME"));
            SourceName = eRoot.GetAttribute("SRC");
            DestinationName = eRoot.GetAttribute("DEST");

            XmlElement eTag = (XmlElement)eRoot.GetElementsByTagName("TAG").Item(0);
            Tag = (EventTag)Enum.Parse(typeof(EventTag), eTag.InnerText, true);

            XmlElement eMsg = (XmlElement)eRoot.GetElementsByTagName("MESSAGE").Item(0);

            //Transform all HTML tags.
            Msg = eMsg.InnerXml.Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">");
            //Msg = transformedMsg.Replace("&quot;", "\"").Replace("&apos;", "'");
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
        public override bool IsXmlObject(string sXML)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(sXML);
            return (xmlDoc.GetElementsByTagName("PACKET").Item(0) != null);
        }

        /// Last Modified: 11/29/09
        /// <summary>
        /// Determines if the XML String is the packet object
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
        /// bool -- Returns true if the XML supplied is this object in XML form.
        public static bool IsPacket(string sXML)
        {
            Packet p = new Packet(EventTag.InvalidAddress, "", 0L);
            return p.IsXmlObject(sXML);
        }

        /// Last Modified: 10/11/09
        /// <summary>
        /// Inverts the path by swapping the packet's source
        ///  and destination.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Review the parameters statement.
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public void InvertPath()
        {
            string s = DestinationName;
            DestinationName = SourceName;
            SourceName = s;
        }
        
        #endregion Methods

    }   //END OF CLASS
}   //END OF NAMESPACE
