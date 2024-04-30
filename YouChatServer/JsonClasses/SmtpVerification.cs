using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "SmtpVerification" class represents SMTP verification details.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing the username and verification flag for the SMTP verification process.
    /// </remarks>
    public class SmtpVerification
    {
        #region Private Fields

        /// <summary>
        /// The string "_username" stores the username of the user.
        /// </summary>
        private string _username;

        /// <summary>
        /// The bool "_afterFail" indicates whether the information sent is after a failed attempt or a reset request.
        /// </summary>
        private bool _afterFail;

        #endregion

        #region Constructors

        /// <summary>
        /// The "SmtpVerification" constructor initializes a new instance of the <see cref="SmtpVerification"/> class with the specified username and verification flag.
        /// </summary>
        /// <param name="username">The username associated with the SMTP verification.</param>
        /// <param name="afterFail">A flag indicating whether the verification is performed after a failed attempt.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the SmtpVerification class, which represents SMTP verification details.
        /// It initializes the username and verification flag for the SMTP verification process.
        /// </remarks>
        public SmtpVerification(string username, bool afterFail)
        {
            _username = username;
            _afterFail = afterFail;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "Username" property represents the username of a user.
        /// It gets or sets the username of the user.
        /// </summary>
        /// <value>
        /// The username of the user.
        /// </value>
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

        /// <summary>
        /// The "AfterFail" property is used to determine if the information sent is after a failed attempt or a reset request.
        /// It gets or sets a value indicating whether the information is after a failed attempt or a reset request.
        /// </summary>
        /// <value>
        /// true if the information is after a failed attempt or reset request; otherwise, false.
        /// </value>
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

        #endregion
    }
}
