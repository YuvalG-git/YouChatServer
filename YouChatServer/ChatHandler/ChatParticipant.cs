using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.ChatHandler
{
    /// <summary>
    /// The "ChatParticipant" class represents a participant in a chat.
    /// </summary>
    /// <remarks>
    /// This class is used to store information about a chat participant, including their username and profile picture.
    /// </remarks>
    public class ChatParticipant
    {
        #region Private Fields

        /// <summary>
        /// The string object "_username" represents the username of the user.
        /// </summary>
        private string _username;

        /// <summary>
        /// The string object "_profilePicture" represents the path or URL to the user's profile picture.
        /// </summary>
        private string _profilePicture;

        #endregion

        #region Constructors

        /// <summary>
        /// The "ChatParticipant" constructor initializes a new instance of the <see cref="ChatParticipant"/> class with the specified username and profile picture.
        /// </summary>
        /// <param name="username">The username of the chat participant.</param>
        /// <param name="profilePicture">The profile picture of the chat participant.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the ChatParticipant class, which represents a participant in a chat.
        /// It initializes the participant's username and profile picture.
        /// </remarks>
        public ChatParticipant(string username, string profilePicture)
        {
            _username = username;
            _profilePicture = profilePicture;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "Username" property represents the username associated with the chat.
        /// It gets or sets the username associated with the chat.
        /// </summary>
        /// <value>
        /// The username associated with the chat.
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
        /// The "ProfilePicture" property represents the profile picture associated with the chat.
        /// It gets or sets the profile picture associated with the chat.
        /// </summary>
        /// <value>
        /// The profile picture associated with the chat.
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
