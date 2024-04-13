using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    internal class PasswordUpdateDetails
    {
        private string _pastPassword;
        private string _newPassword;
        private string _username;

        public PasswordUpdateDetails(string pastPassword, string newPassword, string username)
        {
            _pastPassword = pastPassword;
            _newPassword = newPassword;
            _username = username;
        }
        public string PastPassword
        {
            get
            { 
                return _pastPassword;
            }
            set 
            { 
                _pastPassword = value;
            }
        }
        public string NewPassword
        {
            get
            {
                return _newPassword;
            }
            set
            {
                _newPassword = value;
            }
        }
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
            }
        }
    }
}
