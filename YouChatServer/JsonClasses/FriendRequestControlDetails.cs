using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "FriendRequestControlDetails" class represents the details of a friend request sender.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing the username and profile picture of a friend request sender.
    /// </remarks>
    internal class FriendRequestControlDetails
    {
        #region Private Fields

        /// <summary>
        /// The string "_username" represents the user's username.
        /// </summary>
        private string _username;

        /// <summary>
        /// The string "_profilePicture" represents the URL or path to the user's profile picture.
        /// </summary>
        private string _profilePicture;

        #endregion

        #region Constructors

        /// <summary>
        /// The "FriendRequestControlDetails" constructor initializes a new instance of the <see cref="FriendRequestControlDetails"/> class with the specified username and profile picture.
        /// </summary>
        /// <param name="username">The username of the friend request sender.</param>
        /// <param name="profilePicture">The profile picture of the friend request sender.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the FriendRequestControlDetails class, which represents the details of a friend request sender.
        /// It initializes the username and profile picture of the friend request sender.
        /// </remarks>
        public FriendRequestControlDetails(string username, string profilePicture)
        {
            _username = username;
            _profilePicture = profilePicture;
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
        /// The "ProfilePicture" property represents the profile picture of a user.
        /// It gets or sets the profile picture of the user.
        /// </summary>
        /// <value>
        /// The profile picture of the user.
        /// </value>
        public string ProfilePicture
        {
            get 
            { 
                return _profilePicture;
            }
            set 
            { 
                _profilePicture = value;
            }
        }

        #endregion
    }
}
