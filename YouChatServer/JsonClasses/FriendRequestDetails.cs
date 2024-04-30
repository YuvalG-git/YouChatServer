using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "FriendRequestDetails" class represents the details of a friend request sender.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing the username and tagline of a friend request sender.
    /// </remarks>
    internal class FriendRequestDetails
    {
        #region Private Fields

        /// <summary>
        /// The string "_username" represents the user's username.
        /// </summary>
        private string _username;

        /// <summary>
        /// The string "_tagLine" represents the user's tagline or description.
        /// </summary>
        private string _tagLine;

        #endregion

        #region Constructors

        /// <summary>
        /// The "FriendRequestDetails" constructor initializes a new instance of the <see cref="FriendRequestDetails"/> class with the specified username and tagline.
        /// </summary>
        /// <param name="username">The username of the friend request sender.</param>
        /// <param name="tagLine">The tagline of the friend request sender.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the FriendRequestDetails class, which represents the details of a friend request sender.
        /// It initializes the username and tagline of the friend request sender.
        /// </remarks>
        public FriendRequestDetails(string username, string tagLine)
        {
            _username = username;
            _tagLine = tagLine;
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
        /// The "TagLine" property represents the tag line of a user.
        /// It gets or sets the tag line of the user.
        /// </summary>
        /// <value>
        /// The tag line of the user.
        /// </value>
        public string TagLine
        {
            get
            { 
                return _tagLine;
            }
            set
            { 
                _tagLine = value;
            }
        }

        #endregion
    }
}
