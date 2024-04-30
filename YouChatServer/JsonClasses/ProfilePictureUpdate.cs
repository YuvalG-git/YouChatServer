using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "ProfilePictureUpdate" class represents a request to update the user's profile picture.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing the username and profile picture ID for the update.
    /// </remarks>
    public class ProfilePictureUpdate
    {
        #region Private Fields

        /// <summary>
        /// The string "_username" stores the username of the user.
        /// </summary>
        private string _username;

        /// <summary>
        /// The string "_profilePictureId" stores the profile picture ID of the user.
        /// </summary>
        private string _profilePictureId;

        #endregion

        #region Constructors

        /// <summary>
        /// The "ProfilePictureUpdate" constructor initializes a new instance of the <see cref="ProfilePictureUpdate"/> class with the specified username and profile picture ID.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="profilePictureId">The ID of the new profile picture.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the ProfilePictureUpdate class, which represents a request to update the user's profile picture.
        /// It initializes the username and profile picture ID for the update.
        /// </remarks>
        public ProfilePictureUpdate(string username, string profilePictureId)
        {
            _username = username;
            _profilePictureId = profilePictureId;
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
        /// The "ProfilePictureId" property represents the profile picture ID of a user.
        /// It gets or sets the profile picture ID of the user.
        /// </summary>
        /// <value>
        /// The profile picture ID of the user.
        /// </value>
        public string ProfilePictureId
        {
            get
            {
                return _profilePictureId;
            }
            set
            {
                _profilePictureId = value;
            }
        }

        #endregion
    }
}
