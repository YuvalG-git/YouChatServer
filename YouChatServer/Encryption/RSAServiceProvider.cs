using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.Encryption
{
    internal class RSAServiceProvider
    {
        private string PrivateKey;
        private string PublicKey;
        private UnicodeEncoding Encoder;
        private RSACryptoServiceProvider RSA;

        public RSAServiceProvider()
        {
            Encoder = new UnicodeEncoding();
            RSA = new RSACryptoServiceProvider();

            PrivateKey = RSA.ToXmlString(true);
            PublicKey = RSA.ToXmlString(false);
        }
        /// <summary>
        /// return PrivateKey
        /// </summary>
        /// <returns>PrivateKey</returns>
        public string GetPrivateKey()
        {
            return this.PrivateKey;
        }
        /// <summary>
        /// return PublicKey
        /// </summary>
        /// <returns>PublicKey</returns>
        public string GetPublicKey()
        {
            return this.PublicKey;
        }
        /// <summary>
        /// decript data by privateKey
        /// </summary>
        /// <param name="Data">data to decript</param>
        /// /// <param name="PrivateKey">privateKey</param>
        /// <returns>decripted data</returns>
        public string Decrypt(string Data, string PrivateKey)
        {

            var DataArray = Data.Split(new char[] { ',' });
            byte[] DataByte = new byte[DataArray.Length];
            for (int i = 0; i < DataArray.Length; i++)
            {
                DataByte[i] = Convert.ToByte(DataArray[i]);
            }

            RSA.FromXmlString(PrivateKey);
            var DecryptedByte = RSA.Decrypt(DataByte, false);
            return Encoder.GetString(DecryptedByte);
        }
        /// <summary>
        /// Encrypt the data by public key
        /// </summary>
        /// <param name="Data">data to encrypt</param>
        /// <param name="PublicKey"></param>
        /// <returns>encripted data</returns>
        public string Encrypt(string Data, string PublicKey)
        {
            var Rsa = new RSACryptoServiceProvider();
            Rsa.FromXmlString(PublicKey);
            var DataToEncrypt = Encoder.GetBytes(Data);
            var EncryptedByteArray = Rsa.Encrypt(DataToEncrypt, false);
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

            return StringBuilder.ToString();
        }
    }
}
