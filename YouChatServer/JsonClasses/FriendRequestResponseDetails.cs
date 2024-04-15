using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    internal class FriendRequestResponseDetails
    {
        private string _username;
        private string _status;

        public FriendRequestResponseDetails(string username, string status)
        {
            _username = username;
            _status = status;
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }
        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }
    }
}
