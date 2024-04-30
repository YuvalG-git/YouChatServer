using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer
{
    /// <summary>
    /// The "RandomStringCreator" class provide a method for generating random strings.
    /// </summary>
    internal class RandomStringCreator
    {
        #region Private Static Fields

        /// <summary>
        /// The static Random object "_random" is used for generating random numbers.
        /// </summary>
        private static Random _random = new Random();

        #endregion

        #region Private Const Fields

        /// <summary>
        /// The string object "uppdercaseLettersAndNumbers" contains uppercase letters and numbers.
        /// </summary>
        private const string uppdercaseLettersAndNumbers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        
        /// <summary>
        /// The string object "allChars" contains all ASCII characters, including lowercase letters, uppercase letters, and numbers.
        /// </summary>
        private const string allChars = "abcdefghijklmnopqrstuvwABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        #endregion

        #region Public Methods

        /// <summary>
        /// The "RandomString" method generates a random string of the specified length, optionally including lowercase letters.
        /// </summary>
        /// <param name="Length">The length of the random string to generate.</param>
        /// <param name="containLowercaseLetters">Indicates whether the random string should include lowercase letters (default is false).</param>
        /// <returns>A randomly generated string.</returns>
        /// <remarks>
        /// This method creates a string of uppercase letters and numbers (if containLowercaseLetters is false) or uppercase and lowercase letters along with numbers (if containLowercaseLetters is true).
        /// It then selects random characters from this string to construct a string of the specified length.
        /// The generated string can be used for various purposes, such as generating passwords or codes.
        /// </remarks>
        public static string RandomString(int Length, bool containLowercaseLetters = false)
        {
            string chars = containLowercaseLetters ? allChars : uppdercaseLettersAndNumbers;
            return new string(Enumerable.Repeat(chars, Length).Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        #endregion
    }
}
