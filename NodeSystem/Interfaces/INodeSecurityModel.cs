using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeSystem.Interfaces
{
    public interface INodeSecurityModel
    {
        /// Last Modified: 10/17/10
        /// <summary>
        /// Transforms a series of raw bytes into a series of encrypted bytes.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// <param name="rawBytes">
        /// An array of raw bytes to encryption.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: Refer to the return statement.
        /// -----------------------------------------------------
        /// <returns>
        /// Returns a series of encrypted bytes based on the transformation.
        /// </returns>
        byte[] ByteEncrypt(byte[] rawBytes);

        /// Last Modified: 10/18/10
        /// <summary>
        /// Transforms a series of encrypted bytes into a series of decrypted bytes.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// <param name="rawBytes">
        /// An array of raw bytes to decrypted.
        /// </param>
        /// <param name="byteCount">
        /// The byte count of the raw bytes.  This argument also contains the number of transformed bytes after transformation.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: Refer to the return statement.
        /// -----------------------------------------------------
        /// <returns>
        /// Returns a decrypted series of bytes based on the transformation.
        /// </returns>
        byte[] ByteDecrypt(byte[] rawBytes, ref int byteCount);
    }
}
