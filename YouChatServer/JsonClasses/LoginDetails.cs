using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "LoginDetails" class represents the details for a login operation.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing the username and password for a login.
    /// </remarks>
    internal class LoginDetails
    {
        #region Private Fields

        /// <summary>
        /// The string "_username" stores the username of the user.
        /// </summary>
        private string _username;

        /// <summary>
        /// The string "_password" stores the password of the user.
        /// </summary>
        private string _password;

        #endregion

        #region Constructors

        /// <summary>
        /// The "LoginDetails" constructor initializes a new instance of the <see cref="LoginDetails"/> class with the specified username and password.
        /// </summary>
        /// <param name="username">The username for the login.</param>
        /// <param name="password">The password for the login.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the LoginDetails class, which represents the details for a login operation.
        /// It initializes the username and password for the login.
        /// </remarks>
        public LoginDetails(string username, string password)
        {
            _username = username;
            _password = password;
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
        /// The "Password" property represents the password of a user.
        /// It gets or sets the password of the user.
        /// </summary>
        /// <value>
        /// The password of the user.
        /// </value>
        public string Password
        {
            get 
            { 
                return _password;
            }
            set 
            { 
                _password = value;
            }
        }

        #endregion
    }
}
