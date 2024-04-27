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
        /// <summary>
        /// The static Random object "_random" is used for generating random numbers.
        /// </summary>
        public static Random _random = new Random();

        /// <summary>
        /// The string object "uppdercaseLettersAndNumbers" contains uppercase letters and numbers.
        /// </summary>
        const string uppdercaseLettersAndNumbers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        
        /// <summary>
        /// The string object "allChars" contains all ASCII characters, including lowercase letters, uppercase letters, and numbers.
        /// </summary>
        const string allChars = "abcdefghijklmnopqrstuvwABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        /// <summary>
        /// The "RandomString" method generates a random string of a specified length.
        /// </summary>
        /// <param name="Length">The length of the random string to generate.</param>
        /// <param name="containLowercaseLetters">Specifies whether the random string should contain lowercase letters.</param>
        /// <returns>A randomly generated string.</returns>
        public static string RandomString(int Length, bool containLowercaseLetters = false)
        {
            string chars = containLowercaseLetters ? allChars : uppdercaseLettersAndNumbers;
            return new string(Enumerable.Repeat(chars, Length).Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
