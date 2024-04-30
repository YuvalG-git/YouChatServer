using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "PasswordUpdateDetails" class represents the details for updating a password.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing the past password, new password, and username for a password update operation.
    /// </remarks>
    internal class PasswordUpdateDetails
    {
        #region Private Fields

        /// <summary>
        /// The string "_pastPassword" stores the user's previous password.
        /// </summary>
        private string _pastPassword;

        /// <summary>
        /// The string "_newPassword" stores the user's new password.
        /// </summary>
        private string _newPassword;

        /// <summary>
        /// The string "_username" stores the username of the user.
        /// </summary>
        private string _username;

        #endregion

        #region Constructors

        /// <summary>
        /// The "PasswordUpdateDetails" constructor initializes a new instance of the <see cref="PasswordUpdateDetails"/> class with the specified past password, new password, and username.
        /// </summary>
        /// <param name="pastPassword">The past password of the user.</param>
        /// <param name="newPassword">The new password of the user.</param>
        /// <param name="username">The username of the user.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the PasswordUpdateDetails class, which represents the details for updating a password.
        /// It initializes the past password, new password, and username for the password update operation.
        /// </remarks>
        public PasswordUpdateDetails(string pastPassword, string newPassword, string username)
        {
            _pastPassword = pastPassword;
            _newPassword = newPassword;
            _username = username;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "PastPassword" property represents a past password of a user.
        /// It gets or sets a past password of the user.
        /// </summary>
        /// <value>
        /// A past password of the user.
        /// </value>
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

        /// <summary>
        /// The "NewPassword" property represents a new password for a user.
        /// It gets or sets the new password for the user.
        /// </summary>
        /// <value>
        /// The new password for the user.
        /// </value>
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

        #endregion
    }
}
