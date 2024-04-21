using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    internal class SmtpDetails
    {
        private string _emailAddress;
        private SmtpVerification _smtpVerification;
        public SmtpDetails(string emailAddress, SmtpVerification smtpVerification)
        {
            _emailAddress = emailAddress;
            _smtpVerification = smtpVerification;
        }

        public string EmailAddress
        {
            get
            {
                return _emailAddress;
            }
            set
            {
                _emailAddress = value;
            }
        }
        public SmtpVerification SmtpVerification
        {
            get
            {
                return _smtpVerification;
            }
            set
            {
                _smtpVerification = value;
            }
        }

    }
}
