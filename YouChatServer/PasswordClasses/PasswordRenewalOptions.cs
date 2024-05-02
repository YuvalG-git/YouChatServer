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
        #region Private Fields

        /// <summary>
        /// The <see cref="EnumHandler.CommunicationMessageID_Enum"/> object '_successfulPasswordRenewal' represents the ID for a successful password renewal communication message.
        /// </summary>
        private EnumHandler.CommunicationMessageID_Enum _successfulPasswordRenewal;

        /// <summary>
        /// The <see cref="EnumHandler.CommunicationMessageID_Enum"/> object '_failedPasswordRenewal' represents the ID for a failed password renewal communication message.
        /// </summary>
        private EnumHandler.CommunicationMessageID_Enum _failedPasswordRenewal;

        /// <summary>
        /// The <see cref="EnumHandler.CommunicationMessageID_Enum"/> object '_errorPasswordRenewal' represents the ID for an error in password renewal communication message.
        /// </summary>
        private EnumHandler.CommunicationMessageID_Enum _errorPasswordRenewal;

        #endregion

        #region Constructors

        /// <summary>
        /// The "PasswordRenewalOptions" constructor initializes a new instance of the <see cref="PasswordRenewalOptions"/> class with the specified communication message IDs for successful, failed, and error password renewals.
        /// </summary>
        /// <param name="successfulPasswordRenewal">The communication message ID for successful password renewal.</param>
        /// <param name="failedPasswordRenewal">The communication message ID for failed password renewal.</param>
        /// <param name="errorPasswordRenewal">The communication message ID for error password renewal.</param>
        /// <remarks>
        /// This constructor is used to set up the options for password renewal messages.
        /// </remarks>
        public PasswordRenewalOptions(EnumHandler.CommunicationMessageID_Enum successfulPasswordRenewal, EnumHandler.CommunicationMessageID_Enum failedPasswordRenewal, EnumHandler.CommunicationMessageID_Enum errorPasswordRenewal)
        {
            _successfulPasswordRenewal = successfulPasswordRenewal;
            _failedPasswordRenewal = failedPasswordRenewal;
            _errorPasswordRenewal = errorPasswordRenewal;
        }

        #endregion

        #region Public Get Methods

        /// <summary>
        /// The "GetSuccessfulPasswordRenewal" method retrieves the ID of a successful password renewal message.
        /// </summary>
        /// <returns>The ID of the successful password renewal message.</returns>
        /// <remarks>
        /// This method returns the ID of a successful password renewal message from the EnumHandler.
        /// </remarks>
        public EnumHandler.CommunicationMessageID_Enum GetSuccessfulPasswordRenewal()
        {
            return _successfulPasswordRenewal;
        }

        /// <summary>
        /// The "GetFailedPasswordRenewal" method retrieves the ID of a failed password renewal message.
        /// </summary>
        /// <returns>The ID of the failed password renewal message.</returns>
        /// <remarks>
        /// This method returns the ID of a failed password renewal message from the EnumHandler.
        /// </remarks>
        public EnumHandler.CommunicationMessageID_Enum GetFailedPasswordRenewal()
        {
            return _failedPasswordRenewal;
        }

        /// <summary>
        /// The "GetErrorPasswordRenewal" method retrieves the ID of an error password renewal message.
        /// </summary>
        /// <returns>The ID of the error password renewal message.</returns>
        /// <remarks>
        /// This method returns the ID of an error password renewal message from the EnumHandler.
        /// </remarks>
        public EnumHandler.CommunicationMessageID_Enum GetErrorPasswordRenewal()
        {
            return _errorPasswordRenewal;
        }

        #endregion
    }
}
