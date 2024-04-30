using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "PastFriendRequest" class represents a past friend request.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing the username, profile picture, and date of a past friend request.
    /// </remarks>
    internal class PastFriendRequest
    {
        #region Private Fields

        /// <summary>
        /// The string "_username" stores the username of the user.
        /// </summary>
        private string _username;

        /// <summary>
        /// The string "_profilePicture" stores the profile picture of the user.
        /// </summary>
        private string _profilePicture;

        /// <summary>
        /// The DateTime "_friendRequestDate" stores the date and time when the friend request was sent.
        /// </summary>
        private DateTime _friendRequestDate;

        #endregion

        #region Constructors

        /// <summary>
        /// The "PastFriendRequest" constructor initializes a new instance of the <see cref="PastFriendRequest"/> class with the specified username, profile picture, and friend request date.
        /// </summary>
        /// <param name="username">The username of the friend request sender.</param>
        /// <param name="profilePicture">The profile picture of the friend request sender.</param>
        /// <param name="friendRequestDate">The date of the friend request.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the PastFriendRequest class, which represents a past friend request.
        /// It initializes the username, profile picture, and friend request date of the past friend request.
        /// </remarks>
        public PastFriendRequest(string username, string profilePicture, DateTime friendRequestDate)
        {
            _username = username;
            _profilePicture = profilePicture;
            _friendRequestDate = friendRequestDate;
        }

        /// <summary>
        /// The "PastFriendRequest" constructor initializes a new instance of the <see cref="PastFriendRequest"/> class with the specified username and friend request time.
        /// </summary>
        /// <param name="username">The username of the friend request sender.</param>
        /// <param name="friendRequestTime">The time of the friend request.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the PastFriendRequest class, which represents a past friend request without a profile picture.
        /// It initializes the username and friend request time of the past friend request.
        /// </remarks>
        public PastFriendRequest(string username, DateTime friendRequestTime)
        {
            _username = username;
            _profilePicture = null;
            _friendRequestDate = friendRequestTime;
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

        /// <summary>
        /// The "FriendRequestDate" property represents the date of a friend request.
        /// It gets or sets the date of the friend request.
        /// </summary>
        /// <value>
        /// The date of the friend request.
        /// </value>
        public DateTime FriendRequestDate
        {
            get 
            { 
                return _friendRequestDate;
            }
            set 
            { 
                _friendRequestDate = value;
            }
        }

        #endregion
    }
}
