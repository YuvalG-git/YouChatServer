using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    internal class SmtpDetails
    {
        private string _username;
        private string _emailAddress;

        public SmtpDetails(string username, string emailAddress)
        {
            _username = username;
            _emailAddress = emailAddress;
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }
        public string EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; }
        }

    }
}
