using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.ChatHandler
{
    /// <summary>
    /// The "GroupChatDetails" class represents the details of a group chat.
    /// </summary>
    /// <remarks>
    /// This class inherits from the base ChatDetails class and adds functionality specific to group chats.
    /// </remarks>
    public class GroupChatDetails : ChatDetails
    {
        #region Private Fields

        /// <summary>
        /// The string object "_chatName" represents the name of the chat.
        /// </summary>
        private string _chatName;

        /// <summary>
        /// The byte array "_chatProfilePicture" represents the profile picture of the chat.
        /// </summary>
        public byte[] _chatProfilePicture;

        #endregion

        #region Constructors

        /// <summary>
        /// The "GroupChatDetails" constructor initializes a new instance of the <see cref="GroupChatDetails"/> class with the specified chat tag line ID, last message time, last message content, last message sender name, chat participants, chat name, and chat profile picture.
        /// </summary>
        /// <param name="chatTagLineId">The ID of the chat tag line.</param>
        /// <param name="lastMessageTime">The time of the last message in the chat.</param>
        /// <param name="lastMessageContent">The content of the last message in the chat.</param>
        /// <param name="lastMessageSenderName">The name of the sender of the last message in the chat.</param>
        /// <param name="chatParticipants">The list of participants in the chat.</param>
        /// <param name="chatName">The name of the group chat.</param>
        /// <param name="chatProfilePicture">The profile picture of the group chat.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the GroupChatDetails class, which represents the details of a group chat.
        /// It inherits the base class constructor to initialize the chat details and adds the group chat's name and profile picture.
        /// </remarks>
        public GroupChatDetails(string chatTagLineId, DateTime? lastMessageTime, string lastMessageContent, string lastMessageSenderName, List<ChatParticipant> chatParticipants, string chatName, byte[] chatProfilePicture) : base(chatTagLineId, lastMessageTime, lastMessageContent, lastMessageSenderName, chatParticipants)
        {
            _chatName = chatName;
            _chatProfilePicture = chatProfilePicture;
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
                _chatName= value;
            }
        }

        /// <summary>
        /// The "ChatProfilePicture" property represents the profile picture of the chat as a byte array.
        /// It gets or sets the profile picture of the chat as a byte array.
        /// </summary>
        /// <value>
        /// The profile picture of the chat as a byte array.
        /// </value>
        public byte[] ChatProfilePicture
        {
            get
            {
                return _chatProfilePicture;
            }
            set
            {
                _chatProfilePicture = value;
            }
        }

        #endregion
    }
}
