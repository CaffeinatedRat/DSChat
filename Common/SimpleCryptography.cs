using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///////////////////////////////
///        CUSTOM           ///
///////////////////////////////
using System.IO;
using System.Security.Cryptography;

namespace Common
{
    class SimpleCryptography
    {
        #region DataType

        /// Last Modified: 10/10/10
        /// <summary>
        /// Provides a set of encryption schemes.
        /// </summary>
        public enum EncryptionScheme
        {
            AES,
            DES
        };

        #endregion DataType

        #region Methods

        /// Last Modified: 10/10/10
        /// <summary>
        /// Default constructor for the class, which does nothing do to the static nature.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA
        /// -----------------------------------------------------
        ///
        /// -----------------------------------------------------
        /// POSTCONDITIONS: NA
        /// -----------------------------------------------------
        /// <returns></returns>
        public SimpleCryptography()
        {
        }

        /// Last Modified: 10/10/10
        /// <summary>
        /// A static method that encrypts a byte array based on the scheme & passphrase provided.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// <param name="passphrase">
        /// Is a 16-character word that is used as a key for encryption.
        /// </param>
        /// <param name="es">
        /// Determines the encryption type being used.
        /// </param>
        /// <param name="rawBytes">
        /// An array of raw bytes to encryption.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: Refer to the return statement.
        /// -----------------------------------------------------
        /// <returns>
        /// Returns an encrypted series of bytes based on the passphrase.
        /// </returns>
        public static byte[] PassphraseByteEncrypt(string passphrase, EncryptionScheme es, byte[] rawBytes)
        {
            CryptoStream writerStream = null;
            MemoryStream mStream = null;

            try
            {

                //Make sure the passphrase is exactly 16-characters.
                passphrase = PreparePassphrase(passphrase);

                mStream = new MemoryStream();

                //Create the stream for encryption.
                writerStream = new CryptoStream(mStream,
                    GetCryptoTransform(EncryptionScheme.AES, true, passphrase, passphrase),
                    CryptoStreamMode.Write);

                //Write those bytes to the stream.
                writerStream.Write(rawBytes, 0, rawBytes.Length);
                writerStream.FlushFinalBlock();

                //Obtain the transformed bytes.
                byte[] transformedBytes = mStream.ToArray();

                //Return the encrypted series of bytes.
                return transformedBytes;
            }
            finally
            {
                //Clean up.
                mStream.Close();
                writerStream.Close();
            }
        }

        /// Last Modified: 10/12/10
        /// <summary>
        /// A static method that encrypts a msg based on the scheme & passphrase provided.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// <param name="passphrase">
        /// Is a 16-character word that is used as a key for encryption.
        /// </param>
        /// <param name="es">
        /// Determines the encryption type being used.
        /// </param>
        /// <param name="Msg">
        /// The string being encrypted.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: Refer to the return statement.
        /// -----------------------------------------------------
        /// <returns>
        /// Returns an encrypted string based on the passphrase.
        /// </returns>
        public static string PassphraseStrEncrypt(string passphrase, EncryptionScheme es, string Msg)
        {
            //Return the encrypted series of bytes as a string.
            return Convert.ToBase64String(PassphraseByteEncrypt(passphrase, es, Encoding.Default.GetBytes(Msg)));
        }

        /// Last Modified: 10/18/10
        /// <summary>
        /// A static method that decrypts a byte array based on the scheme & passphrase provided.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// <param name="passphrase">
        /// Is a 16-character word that is used as a key for decryption.
        /// </param>
        /// <param name="es">
        /// Determines the decryption type being used.
        /// </param>
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
        /// Returns a decrypted series of bytes based on the passphrase.
        /// </returns>
        public static byte[] PassphraseByteDecrypt(string passphrase, EncryptionScheme es, byte[] rawBytes, ref int byteCount)
        {
            MemoryStream mStream = null;
            CryptoStream readerStream = null;

            try
            {
                //Make sure the passphrase is exactly 16-characters.
                passphrase = PreparePassphrase(passphrase);

                //Instantiate the memory stream with the bytes of the encrypted message.
                mStream = new MemoryStream(rawBytes, 0, byteCount);

                //Create the stream for decryption.
                readerStream = new CryptoStream(mStream,
                    GetCryptoTransform(EncryptionScheme.AES, false, passphrase, passphrase),
                    CryptoStreamMode.Read);

                //Obtain the transformed bytes.
                byte[] transformedBytes = new byte[byteCount];
                byteCount = readerStream.Read(transformedBytes, 0, byteCount);

                //Return the series of bytes.
                return transformedBytes;
            }
            finally
            {
                //Cleanup.
                mStream.Close();
                readerStream.Close();
            }
        }

        /// Last Modified: 10/25/10
        /// <summary>
        /// A static method that decrypts a string based on the scheme & passphrase provided.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// <param name="passphrase">
        /// Is a 16-character word that is used as a key for decryption.
        /// </param>
        /// <param name="es">
        /// Determines the decryption type being used.
        /// </param>
        /// <param name="Msg">
        /// The string being decrypted.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: Refer to the return statement.
        /// -----------------------------------------------------
        /// <returns>
        /// Returns a decrypts string based on the passphrase.
        /// </returns>
        public static string PassphraseStrDecrypt(string passphrase, EncryptionScheme es, string Msg)
        {
            byte[] byRaw = Convert.FromBase64String(Msg);
            int byteCount = byRaw.Length;
            
            byte[] byTransformed = PassphraseByteDecrypt(passphrase, es, byRaw, ref byteCount);

            //Convert and return the series of bytes as a string.
            ASCIIEncoding enc = new ASCIIEncoding();
            return enc.GetString(byTransformed, 0, byteCount);
        }

        /// Last Modified: 10/10/10
        /// <summary>
        /// Returns the appropriate ICryptoTransform object based on the encryption scheme, direction,
        ///  key, and initialization vector.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// <param name="es">
        /// Encryption scheme being requested.
        /// </param>
        /// <param name="isEncryption">
        /// True for encryption.  False for decryption.
        /// </param>
        /// <param name="key">
        /// A key in string form to use.
        /// NOTE: This value MUST be 16-characters long.
        /// </param>
        /// <param name="IV">
        /// An initialization vector in string form to use.
        /// NOTE: This value MUST be 16-characters long.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: Refer to the return statement.
        /// -----------------------------------------------------
        /// <returns>
        /// Returns a decrypts string based on the passphrase.
        /// </returns>
        private static ICryptoTransform GetCryptoTransform(EncryptionScheme es, bool isEncryption, string key, string IV)
        {
            if (key.Length != 16)
                throw new Exception("The key is an incorrect length and must be 16-characters long.");

            if (IV.Length != 16)
                throw new Exception("The IV is an incorrect length and must be 16-characters long.");

            ASCIIEncoding enc = new ASCIIEncoding();

            switch (es)
            {
                case EncryptionScheme.AES:
                    {
                        AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                        aes.KeySize = 128;
                        aes.Key = enc.GetBytes(key);
                        aes.IV = enc.GetBytes(IV);
                        return isEncryption ? aes.CreateEncryptor() : aes.CreateDecryptor();
                    }

                case EncryptionScheme.DES:
                    {
                        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                        des.KeySize = 128;
                        des.Key = enc.GetBytes(key);
                        des.IV = enc.GetBytes(IV);
                        return isEncryption ? des.CreateEncryptor() : des.CreateDecryptor();
                    }
            }

            return null;
        }

        /// Last Modified: 10/10/10
        /// <summary>
        /// Prepares the passphrase and keeps it at 16-characters, by padding it if necessary.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// <param name="passphrase">
        /// The passphrase to prepare for use.
        /// </param>
        /// <param name="paddingChar">
        /// A character used to pad the passphrase if it is not 16-characters long.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: Refer to the return statement.
        /// -----------------------------------------------------
        /// <returns>
        /// Returns the passphrase after it has been prepared.
        /// </returns>
        private static string PreparePassphrase(string passphrase, char paddingChar)
        {
            return (passphrase.Length >= 16) ? passphrase.Substring(0, 16) : passphrase.PadRight(16, paddingChar);
        }

        /// Last Modified: 10/10/10
        /// <summary>
        /// Prepares the passphrase and keeps it at 16-characters, by padding it if necessary.
        /// NOTE: By default the padding character is -.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: Refer to the following arguments.
        /// -----------------------------------------------------
        /// <param name="passphrase">
        /// The passphrase to prepare for use.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: Refer to the return statement.
        /// -----------------------------------------------------
        /// <returns>
        /// Returns the passphrase after it has been prepared.
        /// </returns>
        private static string PreparePassphrase(string passphrase)
        {
            return PreparePassphrase(passphrase, '-');
        }

        #endregion Methods
    }
}
