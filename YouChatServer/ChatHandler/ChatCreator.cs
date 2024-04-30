using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace YouChatServer.ChatHandler
{
    /// <summary>
    /// The "ChatCreator" class represents a chat and its properties, including the chat name, participants, and profile picture.
    /// It is used in the process of creating a new group chat.
    /// </summary>
    public class ChatCreator
    {
        #region Private Fields

        /// <summary>
        /// The string object "_chatName" represents the name of the chat.
        /// </summary>
        private string _chatName;

        /// <summary>
        /// The List of string objects "_chatParticipants" stores the participants of the chat.
        /// </summary>
        private List<string> _chatParticipants;

        /// <summary>
        /// The byte array "_chatProfilePictureBytes" stores the profile picture of the chat in binary format.
        /// </summary>
        private byte[] _chatProfilePictureBytes;

        #endregion

        #region Constructors

        /// <summary>
        /// The "ChatCreator" constructor initializes a new instance of the <see cref="ChatCreator"/> class with the specified name, chat participants, and chat profile picture bytes.
        /// </summary>
        /// <param name="name">The name of the chat.</param>
        /// <param name="chatParticipants">The list of chat participants.</param>
        /// <param name="chatProfilePictureBytes">The bytes representing the chat's profile picture.</param>
        /// <remarks>
        /// This constructor is used to send new group chat information from a client to the server, including the chat's name, participants, and profile picture.
        /// </remarks>
        public ChatCreator(string name, List<string> chatParticipants, byte[] chatProfilePictureBytes)
        {
            this._chatName = name;
            this._chatParticipants = chatParticipants;
            this._chatProfilePictureBytes = chatProfilePictureBytes;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "ChatName" property represents the name of the chat.
        /// It gets or sets the name of the chat.
        /// </summary>
        /// <value>
        /// The name of the chat.
        /// </value>
        public string ChatName
        {
            get
            {
                return _chatName;
            }
            set
            {
                _chatName = value;
            }
        }

        /// <summary>
        /// The "ChatParticipants" property represents the list of participants in the chat.
        /// It gets or sets the list of participants in the chat.
        /// </summary>
        /// <value>
        /// The list of participants in the chat.
        /// </value>
        public List<string> ChatParticipants
        {
            get
            {
                return _chatParticipants;
            }
            set
            {
                _chatParticipants = value;
            }
        }

        /// <summary>
        /// The "ChatProfilePictureBytes" property represents the profile picture of the chat as a byte array.
        /// It gets or sets the profile picture of the chat as a byte array.
        /// </summary>
        /// <value>
        /// The profile picture of the chat as a byte array.
        /// </value>
        public byte[] ChatProfilePictureBytes
        {
            get
            {
                return _chatProfilePictureBytes;
            }
            set
            {
                _chatProfilePictureBytes = value;
            }
        }

        #endregion
    }
}
