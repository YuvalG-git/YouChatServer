using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatApp
{
    public static class EnumHandler
    {
        public enum UserAuthentication_Enum
        {
            Login,
            Registration,
            PasswordUpdate,
            PasswordRestart
        }
        /// <summary>
        /// Enum used to ...
        /// </summary>
        public enum SmtpMessageType_Enum
        {
            RegistrationMessage,
            LoginMessage,
            PasswordRenewalMessage
        }
        public enum CommunicationMessageID_Enum
        {
            loginRequest,
            loginResponse
        }
    }
}
