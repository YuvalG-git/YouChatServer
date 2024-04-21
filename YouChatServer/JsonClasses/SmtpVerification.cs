using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    public class SmtpVerification
    {
        private string _username;
        private bool _afterFail;

        public SmtpVerification(string username, bool afterFail)
        {
            _username = username;
            _afterFail = afterFail;
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
        public bool AfterFail
        {
            get
            {
                return _afterFail;
            }
            set
            {
                _afterFail = value;
            }
        }
    }
}
