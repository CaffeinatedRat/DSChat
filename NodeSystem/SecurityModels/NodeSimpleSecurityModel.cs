using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///////////////////////////////
///        CUSTOM           ///
///////////////////////////////
using NodeSystem.Interfaces;
using Common;

namespace NodeSystem.SecurityModels
{
    class NodeSimpleSecurityModel : INodeSecurityModel
    {
        #region Properties

        /// Last Modified: 10/17/10
        /// <summary>
        /// Manages the encryption scheme used.
        /// </summary>
        public SimpleCryptography.EncryptionScheme EncryptionScheme { get; set; }

        /// Last Modified: 10/17/10
        /// <summary>
        /// Manages the 16-character word that is used as a key for encryption & decryption.
        /// </summary>
        public string Passphrase { get; set; }

        #endregion Properties

        #region Methods

        /// Last Modified: 10/17/10
        /// <summary>
        /// Initializes the simple security model.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="es">
        /// Determines the encryption type.  Refer to <typeparamref name="SimpleCryptography.EncryptionScheme"/> for more info.
        /// </param>
        /// <param name="passphrase">
        /// Is a 16-character word that is used as a key.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA -- No postconditions exist.
        /// -----------------------------------------------------
        /// Return Value:
        public NodeSimpleSecurityModel(SimpleCryptography.EncryptionScheme es, string passphrase)
        {
            this.EncryptionScheme = es;
            this.Passphrase = passphrase;
        }

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
        public byte[] ByteEncrypt(byte[] rawBytes)
        {
            //Allow a helper class to do all the work.
            return SimpleCryptography.PassphraseByteEncrypt(this.Passphrase
                , this.EncryptionScheme
                , rawBytes);
        }

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
        public byte[] ByteDecrypt(byte[] rawBytes, ref int byteCount)
        {
            //Allow a helper class to do all the work.
            return SimpleCryptography.PassphraseByteDecrypt(this.Passphrase
                , this.EncryptionScheme
                , rawBytes
                , ref byteCount);
        }


        #endregion Methods
    }
}
