using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///////////////////////////////
///        CUSTOM           ///
///////////////////////////////
using System.Xml;

namespace NodeSystem.Packets
{
    public abstract class ObjectEx
    {
        #region Methods

        /// Last Modified: 11/29/09
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public ObjectEx() { }

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
        public ObjectEx(String sXML)
        {
            ToObject(sXML);
        }

        /// Last Modified: 11/29/09
        /// <summary>
        /// Initializes the object based on the serialized byte array.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Review the parameters statement.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="byArray">
        /// An array of bytes that is the serialized version of the object.
        /// </param>
        /// <param name="nSize">
        /// An integer that represents the number of bytes in the array to use.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public ObjectEx(Byte[] byArray, int nSize)
        {
            //Convert the byte array into a string form.
            ToObject(Encoding.ASCII.GetString(byArray, 0, nSize));
        }

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
            throw new Exception("This method is not implement.");
        }

        /// Last Modified: 11/29/09
        /// <summary>
        /// Serializes this object into a series of bytes.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// -----------------------------------------------------
        /// POSTCONDITIONS: Refer to the return statement.
        /// -----------------------------------------------------
        /// Return Value:
        /// Byte[] -- Returns this object in a byte array format.
        public Byte[] ToByteArray()
        {
            return ASCIIEncoding.ASCII.GetBytes(this.ToString());
        }

        /// Last Modified: 11/29/09
        /// <summary>
        /// Decodes the XML String into an object instance of this class.
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
        public abstract void ToObject(String sXML);

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
        /// bool -- Returns true if the XML supplied is this object in XML form.
        public abstract bool IsXmlObject(String sXML);

        #endregion Methods

    }   //END OF CLASS
}   //END OF NAMESPACE
