using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    internal class FriendRequestDetails
    {
        private string _username;
        private string _tagLine;

        public FriendRequestDetails(string username, string tagLine)
        {
            _username = username;
            _tagLine = tagLine;
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }
        public string TagLine
        {
            get { return _tagLine; }
            set { _tagLine = value; }
        }
    }
}
