using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.Encryption
{
    /// <summary>
    /// The "RSAServiceProvider" class provides RSA encryption and decryption services.
    /// </summary>
    /// <remarks>
    /// This class provides methods for generating public and private keys, encrypting and decrypting data.
    /// </remarks>
    internal class RSAServiceProvider
    {
        #region Private Fields

        /// <summary>
        /// The string object "PrivateKey" represents the private key used for encryption and decryption.
        /// </summary>
        private string PrivateKey;

        /// <summary>
        /// The string object "PublicKey" represents the public key used for encryption and decryption.
        /// </summary>
        private string PublicKey;

        /// <summary>
        /// The UnicodeEncoding object "Encoder" represents the encoding used for encoding and decoding strings.
        /// </summary>
        private UnicodeEncoding Encoder;

        /// <summary>
        /// The RSACryptoServiceProvider object "RSA" represents the RSA algorithm provider used for encryption and decryption.
        /// </summary>
        private RSACryptoServiceProvider RSA;

        #endregion

        #region Constructors

        /// <summary>
        /// The "RSAServiceProvider" constructor initializes a new instance of the <see cref="RSAServiceProvider"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is used to create a new instance of the RSAServiceProvider class, which provides RSA encryption and decryption services.
        /// It initializes the encoder, RSA crypto service provider, private key, and public key.
        /// </remarks>
        public RSAServiceProvider()
        {
            Encoder = new UnicodeEncoding();
            RSA = new RSACryptoServiceProvider();

            PrivateKey = RSA.ToXmlString(true);
            PublicKey = RSA.ToXmlString(false);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The "GetPrivateKey" method retrieves the private key associated with the current object instance.
        /// </summary>
        /// <returns>The private key associated with the current object instance.</returns>
        /// <remarks>
        /// This method returns the private key value stored in the PrivateKey property of the current object instance.
        /// </remarks>
        public string GetPrivateKey()
        {
            return this.PrivateKey;
        }

        /// <summary>
        /// The "GetPublicKey" method retrieves the public key associated with the current object instance.
        /// </summary>
        /// <returns>The public key associated with the current object instance.</returns>
        /// <remarks>
        /// This method returns the public key value stored in the PublicKey property of the current object instance.
        /// </remarks>
        public string GetPublicKey()
        {
            return this.PublicKey;
        }

        /// <summary>
        /// The "Decrypt" method decrypts a string of data using the specified private key.
        /// </summary>
        /// <param name="Data">The encrypted data to be decrypted.</param>
        /// <param name="PrivateKey">The private key used for decryption.</param>
        /// <returns>The decrypted data as a string.</returns>
        /// <remarks>
        /// This method first splits the input data string into an array of bytes using a delimiter (',').
        /// It then converts each byte string to a byte and stores them in a byte array.
        /// The method then initializes an RSA object with the specified private key.
        /// Next, it decrypts the byte array using the RSA object and returns the decrypted data as a string.
        /// </remarks>
        public string Decrypt(string Data, string PrivateKey)
        {
            // Split the input data string into an array of byte strings
            var DataArray = Data.Split(new char[] { ',' });
            byte[] DataByte = new byte[DataArray.Length];

            // Convert each byte string to a byte and store in a byte array
            for (int i = 0; i < DataArray.Length; i++)
            {
                DataByte[i] = Convert.ToByte(DataArray[i]);
            }

            // Initialize an RSA object with the specified private key
            RSA.FromXmlString(PrivateKey);

            // Decrypt the byte array using the RSA object
            var DecryptedByte = RSA.Decrypt(DataByte, false);

            // Convert the decrypted byte array to a string and return
            return Encoder.GetString(DecryptedByte);
        }

        /// <summary>
        /// Encrypt the data by public key
        /// </summary>
        /// <param name="Data">data to encrypt</param>
        /// <param name="PublicKey"></param>
        /// <returns>encripted data</returns>
        /// <summary>
        /// The "Encrypt" method encrypts a string of data using the specified public key.
        /// </summary>
        /// <param name="Data">The data to be encrypted.</param>
        /// <param name="PublicKey">The public key used for encryption.</param>
        /// <returns>The encrypted data as a string.</returns>
        /// <remarks>
        /// This method initializes a new RSACryptoServiceProvider object and sets it to use the specified public key.
        /// It then converts the input data string to a byte array.
        /// The method encrypts the byte array using the RSACryptoServiceProvider object and stores the encrypted data in a byte array.
        /// Finally, it converts the byte array to a comma-separated string of bytes and returns it.
        /// </remarks>
        public string Encrypt(string Data, string PublicKey)
        {
            // Initialize a new RSACryptoServiceProvider object
            var Rsa = new RSACryptoServiceProvider();

            // Set the RSACryptoServiceProvider object to use the specified public key
            Rsa.FromXmlString(PublicKey);

            // Convert the input data string to a byte array
            var DataToEncrypt = Encoder.GetBytes(Data);

            // Encrypt the byte array using the RSACryptoServiceProvider object
            var EncryptedByteArray = Rsa.Encrypt(DataToEncrypt, false);

            // Convert the encrypted byte array to a comma-separated string of bytes
            var Length = EncryptedByteArray.Length;
            var Item = 0;
            var StringBuilder = new StringBuilder();
            foreach (var EncryptedByte in EncryptedByteArray)
            {
                Item++;
                StringBuilder.Append(EncryptedByte);

                if (Item < Length)
                    StringBuilder.Append(",");
            }

            // Return the encrypted data as a string
            return StringBuilder.ToString();
        }

        #endregion
    }
}
