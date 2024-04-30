using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.Encryption
{
    /// <summary>
    /// The "MD5" class provides methods for creating MD5 hashes used for hashing passwords and verifications answers in the database.
    /// </summary>
    internal class MD5
    {
        #region Public Static Methods

        /// <summary>
        /// The "CreateMD5Hash" method calculates the MD5 hash from the input string.
        /// </summary>
        /// <param name="input">The input string to be hashed.</param>
        /// <returns>The MD5 hash of the input string.</returns>
        /// <remarks>
        /// This method calculates the MD5 hash from the input string and returns it as a hexadecimal string.
        /// MD5 is a widely used cryptographic hash function that produces a 128-bit hash value.
        /// </remarks>
        public static string CreateMD5Hash(string input)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                stringBuilder.Append(hashBytes[i].ToString("X2"));
            }
            return stringBuilder.ToString();
        }
        
        #endregion
    }
}
