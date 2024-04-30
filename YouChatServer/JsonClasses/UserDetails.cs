using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "UserDetails" class represents details about a user, including the username, profile picture, profile status, and tagline ID.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing user details such as username, profile picture, profile status, and tagline ID.
    /// </remarks>
    internal class UserDetails
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
        /// The string "_profileStatus" stores the profile status of the user.
        /// </summary>
        private string _profileStatus;

        /// <summary>
        /// The string "_tagLineId" stores the tagline ID of the user.
        /// </summary>
        private string _tagLineId;

        #endregion

        #region Constructors

        /// <summary>
        /// The "UserDetails" constructor initializes a new instance of the <see cref="UserDetails"/> class with the specified user details.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="profilePicture">The profile picture of the user.</param>
        /// <param name="profileStatus">The profile status of the user.</param>
        /// <param name="tagLineId">The tag line ID of the user.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the UserDetails class, which represents details about a user.
        /// It initializes the username, profile picture, profile status, and tag line ID for the user.
        /// </remarks>
        public UserDetails(string username, string profilePicture, string profileStatus, string tagLineId)
        {
            _username = username;
            _profilePicture = profilePicture;
            _profileStatus = profileStatus;
            _tagLineId = tagLineId;
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
        /// The "ProfileStatus" property represents the status of a user's profile.
        /// It gets or sets the status of the user's profile.
        /// </summary>
        /// <value>
        /// The status of the user's profile.
        /// </value>
        public string ProfileStatus
        {
            get
            {
                return _profileStatus;
            }
            set
            {
                _profileStatus = value;
            }
        }

        /// <summary>
        /// The "TagLineId" property represents the ID of a user.
        /// It gets or sets the ID of the user.
        /// </summary>
        /// <value>
        /// The ID of the user.
        /// </value>
        public string TagLineId
        {
            get
            {
                return _tagLineId;
            }
            set
            {
                _tagLineId = value;
            }
        }

        #endregion
    }
}
