using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer
{
    internal class PasswordRenewalOptions
    {
        private EnumHandler.CommunicationMessageID_Enum _successfulPasswordRenewal;
        private EnumHandler.CommunicationMessageID_Enum _failedPasswordRenewal;
        private EnumHandler.CommunicationMessageID_Enum _errorPasswordRenewal;

        public PasswordRenewalOptions(EnumHandler.CommunicationMessageID_Enum successfulPasswordRenewal, EnumHandler.CommunicationMessageID_Enum failedPasswordRenewal, EnumHandler.CommunicationMessageID_Enum errorPasswordRenewal)
        {
            _successfulPasswordRenewal = successfulPasswordRenewal;
            _failedPasswordRenewal = failedPasswordRenewal;
            _errorPasswordRenewal = errorPasswordRenewal;
        }
        public EnumHandler.CommunicationMessageID_Enum GetSuccessfulPasswordRenewal()
        {
            return _successfulPasswordRenewal;
        }
        public EnumHandler.CommunicationMessageID_Enum GetFailedPasswordRenewal()
        {
            return _failedPasswordRenewal;
        }
        public EnumHandler.CommunicationMessageID_Enum GetErrorPasswordRenewal()
        {
            return _errorPasswordRenewal;
        }
    }
}
