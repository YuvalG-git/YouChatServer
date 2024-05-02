using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.Encryption
{
    /// <summary>
    /// The "AESServiceProvider" class provides methods for encrypting and decrypting data using the AES algorithm.
    /// </summary>
    /// <remarks>
    /// This class includes methods for encrypting and decrypting data, as well as utility methods for converting between different data formats.
    /// It uses the AES encryption algorithm for encryption and decryption operations.
    /// </remarks>
    internal class AESServiceProvider
    {
        #region Public Static Methods

        /// <summary>
        /// The "EncryptData" method encrypts a string message using a symmetric key and returns the encrypted message as a base64-encoded string.
        /// </summary>
        /// <param name="SymmetricKey">The symmetric key used for encryption.</param>
        /// <param name="Message">The message to encrypt.</param>
        /// <returns>The encrypted message as a base64-encoded string.</returns>
        /// <remarks>
        /// This method uses the AES encryption algorithm to encrypt the input string message.
        /// </remarks>
        public static string EncryptData(string SymmetricKey, string Message)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Message);
            byte[] encryptedBuffer = EncryptDataToBytes(SymmetricKey, buffer);
            string EncryptedMessage = System.Convert.ToBase64String(encryptedBuffer.ToArray());
            return EncryptedMessage;
        }

        /// <summary>
        /// The "EncryptDataToBytes" method encrypts a byte array message using a symmetric key and returns the encrypted message as a byte array.
        /// </summary>
        /// <param name="SymmetricKey">The symmetric key used for encryption.</param>
        /// <param name="Message">The byte array message to encrypt.</param>
        /// <returns>The encrypted message as a byte array.</returns>
        /// <remarks>
        /// This method uses the AES encryption algorithm to encrypt the input byte array message.
        /// </remarks>
        public static byte[] EncryptDataToBytes(string SymmetricKey, byte[] Message)
        {
            byte[] Key = Encoding.UTF8.GetBytes(SymmetricKey);
            byte[] IV = new byte[16];
            byte[] EncryptedMessageAsBytes = Encrypt(Message, Key, IV);
            return EncryptedMessageAsBytes;
        }

        /// <summary>
        /// The "DecryptData" method decrypts a base64-encoded string message using a symmetric key and returns the decrypted message as a string.
        /// </summary>
        /// <param name="SymmetricKey">The symmetric key used for decryption.</param>
        /// <param name="Message">The base64-encoded string message to decrypt.</param>
        /// <returns>The decrypted message as a string.</returns>
        /// <remarks>
        /// This method uses the AES decryption algorithm to decrypt the input base64-encoded string message.
        /// </remarks>
        public static string DecryptData(string SymmetricKey, string Message)
        {
            byte[] buffer = System.Convert.FromBase64String(Message);
            byte[] decryptedBuffer = DecryptDataToBytes(SymmetricKey, buffer);
            string DecryptedMessage = System.Text.Encoding.UTF8.GetString(decryptedBuffer.ToArray());
            return DecryptedMessage;
        }

        /// <summary>
        /// The "DecryptDataToBytes" method decrypts a byte array message using a symmetric key and returns the decrypted message as a byte array.
        /// </summary>
        /// <param name="SymmetricKey">The symmetric key used for decryption.</param>
        /// <param name="Message">The byte array message to decrypt.</param>
        /// <returns>The decrypted message as a byte array.</returns>
        /// <remarks>
        /// This method uses the AES decryption algorithm to decrypt the input byte array message.
        /// </remarks>
        public static byte[] DecryptDataToBytes(string SymmetricKey, byte[] Message)
        {
            byte[] Key = Encoding.UTF8.GetBytes(SymmetricKey);
            byte[] IV = new byte[16];
            byte[] DecryptedMessageAsBytes = Decrypt(Message, Key, IV);
            return DecryptedMessageAsBytes;
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// The "Encrypt" method encrypts a byte array using the AES algorithm with the provided key and initialization vector (IV).
        /// </summary>
        /// <param name="plainText">The byte array containing the data to be encrypted.</param>
        /// <param name="Key">The byte array containing the encryption key.</param>
        /// <param name="IV">The byte array containing the initialization vector (IV).</param>
        /// <returns>A byte array containing the encrypted data.</returns>
        /// <remarks>
        /// This method uses the AES encryption algorithm to encrypt the input data.
        /// </remarks>
        private static byte[] Encrypt(byte[] plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(plainText, 0, plainText.Length);
                        csEncrypt.FlushFinalBlock();
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted string from the memory stream.
            return encrypted;
        }

        /// <summary>
        /// The "Decrypt" method decrypts a byte array using the AES algorithm with the provided key and initialization vector (IV).
        /// </summary>
        /// <param name="cipherText">The byte array containing the encrypted data to be decrypted.</param>
        /// <param name="Key">The byte array containing the decryption key.</param>
        /// <param name="IV">The byte array containing the initialization vector (IV).</param>
        /// <returns>A byte array containing the decrypted data.</returns>
        /// <remarks>
        /// This method uses the AES decryption algorithm to decrypt the input data.
        /// </remarks>
        private static byte[] Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            byte[] plaintextBytes;
            byte[] buffer = cipherText;
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(buffer))
                {
                    using (CryptoStream csDecrypt = new CryptoStream((Stream)msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (MemoryStream msPlainText = new MemoryStream())
                        {
                            csDecrypt.CopyTo(msPlainText);
                            plaintextBytes = msPlainText.ToArray();
                        }
                    }
                }
            }
            return plaintextBytes;
        }

        #endregion
    }
}
