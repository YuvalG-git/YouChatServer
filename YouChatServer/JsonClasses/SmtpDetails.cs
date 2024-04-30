using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "SmtpDetails" class represents SMTP details for an email address.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing the email address and SMTP verification details for the SMTP configuration.
    /// </remarks>
    internal class SmtpDetails
    {
        #region Private Fields

        /// <summary>
        /// The string "_emailAddress" stores the email address of the user.
        /// </summary>
        private string _emailAddress;

        /// <summary>
        /// The SmtpVerification object "_smtpVerification" handles the SMTP verification status.
        /// </summary>
        private SmtpVerification _smtpVerification;

        #endregion

        #region Constructors

        /// <summary>
        /// The "SmtpDetails" constructor initializes a new instance of the <see cref="SmtpDetails"/> class with the specified email address and SMTP verification details.
        /// </summary>
        /// <param name="emailAddress">The email address associated with the SMTP details.</param>
        /// <param name="smtpVerification">The SMTP verification details.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the SmtpDetails class, which represents SMTP details for an email address.
        /// It initializes the email address and SMTP verification details for the SMTP configuration.
        /// </remarks>
        public SmtpDetails(string emailAddress, SmtpVerification smtpVerification)
        {
            _emailAddress = emailAddress;
            _smtpVerification = smtpVerification;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "EmailAddress" property represents the email address of a user.
        /// It gets or sets the email address of the user.
        /// </summary>
        /// <value>
        /// The email address of the user.
        /// </value>
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

        /// <summary>
        /// The "SmtpVerification" property represents the SMTP verification status.
        /// It gets or sets the SMTP verification status.
        /// </summary>
        /// <value>
        /// The SMTP verification status.
        /// </value>
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

        #endregion
    }
}
