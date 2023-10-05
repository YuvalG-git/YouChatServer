using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer
{
    internal class RandomStringCreator
    {
        public static Random _random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        public static string RandomString(int Length)
        {
            return new string(Enumerable.Repeat(chars, Length).Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
