using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer
{
    /// <summary>
    /// The "PasswordRenewalOptions" class provides options for password renewal messages.
    /// </summary>
    internal class PasswordRenewalOptions
    {
        /// <summary>
        /// The "_successfulPasswordRenewal" object represents the message ID for successful password renewal.
        /// </summary>
        private EnumHandler.CommunicationMessageID_Enum _successfulPasswordRenewal;

        /// <summary>
        /// The "_failedPasswordRenewal" object represents the message ID for failed password renewal.
        /// </summary>
        private EnumHandler.CommunicationMessageID_Enum _failedPasswordRenewal;

        /// <summary>
        /// The "_errorPasswordRenewal" object represents the message ID for error in password renewal.
        /// </summary>
        private EnumHandler.CommunicationMessageID_Enum _errorPasswordRenewal;

        /// <summary>
        /// The "PasswordRenewalOptions" constructor initializes a new instance of the <see cref="PasswordRenewalOptions"/> class.
        /// </summary>
        /// <param name="successfulPasswordRenewal">The message ID for successful password renewal.</param>
        /// <param name="failedPasswordRenewal">The message ID for failed password renewal.</param>
        /// <param name="errorPasswordRenewal">The message ID for error in password renewal.</param>
        public PasswordRenewalOptions(EnumHandler.CommunicationMessageID_Enum successfulPasswordRenewal, EnumHandler.CommunicationMessageID_Enum failedPasswordRenewal, EnumHandler.CommunicationMessageID_Enum errorPasswordRenewal)
        {
            _successfulPasswordRenewal = successfulPasswordRenewal;
            _failedPasswordRenewal = failedPasswordRenewal;
            _errorPasswordRenewal = errorPasswordRenewal;
        }

        /// <summary>
        /// The "GetSuccessfulPasswordRenewal" method returns the message ID for successful password renewal.
        /// </summary>
        /// <returns>The message ID for successful password renewal.</returns>
        public EnumHandler.CommunicationMessageID_Enum GetSuccessfulPasswordRenewal()
        {
            return _successfulPasswordRenewal;
        }

        /// <summary>
        /// The "GetFailedPasswordRenewal" method returns the message ID for failed password renewal.
        /// </summary>
        /// <returns>The message ID for failed password renewal.</returns>
        public EnumHandler.CommunicationMessageID_Enum GetFailedPasswordRenewal()
        {
            return _failedPasswordRenewal;
        }

        /// <summary>
        /// The "GetErrorPasswordRenewal" method returns the message ID for error in password renewal.
        /// </summary>
        /// <returns>The message ID for error in password renewal.</returns>
        public EnumHandler.CommunicationMessageID_Enum GetErrorPasswordRenewal()
        {
            return _errorPasswordRenewal;
        }
    }
}
