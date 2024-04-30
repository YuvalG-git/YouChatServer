using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "FriendRequestResponseDetails" class represents the details of a friend request response.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing the username of the friend request recipient and the status of the friend request response.
    /// </remarks>
    internal class FriendRequestResponseDetails
    {
        #region Private Fields

        /// <summary>
        /// The string "_username" represents the user's username.
        /// </summary>
        private string _username;

        /// <summary>
        /// The string "_status" represents the user's status.
        /// </summary>
        private string _status;

        #endregion

        #region Constructors

        /// <summary>
        /// The "FriendRequestResponseDetails" constructor initializes a new instance of the <see cref="FriendRequestResponseDetails"/> class with the specified username and status.
        /// </summary>
        /// <param name="username">The username of the friend request recipient.</param>
        /// <param name="status">The status of the friend request response.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the FriendRequestResponseDetails class, which represents the details of a friend request response.
        /// It initializes the username of the friend request recipient and the status of the friend request response.
        /// </remarks>
        public FriendRequestResponseDetails(string username, string status)
        {
            _username = username;
            _status = status;
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
        /// The "Status" property represents the status of a user.
        /// It gets or sets the status of the user.
        /// </summary>
        /// <value>
        /// The status of the user.
        /// </value>
        public string Status
        {
            get 
            { 
                return _status;
            }
            set 
            { 
                _status = value;
            }
        }

        #endregion
    }
}
