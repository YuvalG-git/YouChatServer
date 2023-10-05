using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.Encryption
{
    internal class Encryption
    {
        public static string EncryptData(string SymmetricKey, string Message)
        {
            byte[] Key = Encoding.UTF8.GetBytes(SymmetricKey);
            byte[] IV = new byte[16];
            string EncryptedMessage = AESServiceProvider.Encrypt(Message, Key, IV);
            return EncryptedMessage;
        }

        public static string DecryptData(string SymmetricKey, string Message)
        {
            byte[] Key = Encoding.UTF8.GetBytes(SymmetricKey);
            byte[] IV = new byte[16];

            string DecryptedMessage = AESServiceProvider.Decrypt(Message, Key, IV);
            return DecryptedMessage;
        }
    }
}
