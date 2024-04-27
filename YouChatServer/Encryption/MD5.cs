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
        /// <summary>
        /// The "CreateMD5Hash" method calculates the MD5 hash from the input string.
        /// </summary>
        /// <param name="input">The input string to be hashed.</param>
        /// <returns>The MD5 hash of the input string.</returns>
        public static string CreateMD5Hash(string input)//crypts passwords
        {
            // Step 1, calculate MD5 hash from input
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            
            // Step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
