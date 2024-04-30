using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.ChatHandler
{
    /// <summary>
    /// The "ChatDetails" class represents the details of a chat, including the tag line ID, last message time, last message content, last message sender name, and chat participants.
    /// It is used in managing and displaying chat details.
    /// </summary>
    public class ChatDetails
    {
        #region Private Fields

        /// <summary>
        /// The string object "_chatTagLineId" represents the tag line ID of the chat.
        /// </summary>
        private string _chatTagLineId;

        /// <summary>
        /// The nullable DateTime object "_lastMessageTime" represents the time of the last message in the chat.
        /// </summary>
        private DateTime? _lastMessageTime;

        /// <summary>
        /// The string object "_lastMessageContent" represents the content of the last message in the chat.
        /// </summary>
        private string _lastMessageContent;

        /// <summary>
        /// The string object "_lastMessageSenderName" represents the name of the sender of the last message in the chat.
        /// </summary>
        private string _lastMessageSenderName;

        /// <summary>
        /// The List of ChatParticipant objects "_chatParticipants" stores the participants of the chat.
        /// </summary>
        private List<ChatParticipant> _chatParticipants;

        #endregion

        #region Constructors

        /// <summary>
        /// The "ChatDetails" constructor initializes a new instance of the <see cref="ChatDetails"/> class with the specified chat tag line ID, last message time, last message content, last message sender name, and chat participants.
        /// </summary>
        /// <param name="chatTagLineId">The ID of the chat tag line.</param>
        /// <param name="lastMessageTime">The time of the last message in the chat.</param>
        /// <param name="lastMessageContent">The content of the last message in the chat.</param>
        /// <param name="lastMessageSenderName">The name of the sender of the last message in the chat.</param>
        /// <param name="chatParticipants">The list of participants in the chat.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the ChatDetails class, which represents the details of a chat.
        /// It initializes the chat's tag line ID, last message time, last message content, last message sender name, and chat participants.
        /// </remarks>
        public ChatDetails(string chatTagLineId, DateTime? lastMessageTime, string lastMessageContent, string lastMessageSenderName, List<ChatParticipant> chatParticipants)
        {
            _chatTagLineId = chatTagLineId;
            _lastMessageTime = lastMessageTime;
            _lastMessageContent = lastMessageContent;
            _chatParticipants = chatParticipants;
            _lastMessageSenderName = lastMessageSenderName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "ChatTagLineId" property represents the tag line ID of the chat.
        /// It gets or sets the tag line ID of the chat.
        /// </summary>
        /// <value>
        /// The tag line ID of the chat.
        /// </value>
        public string ChatTagLineId
        {
            get
            {
                return _chatTagLineId;
            }
            set
            {
                _chatTagLineId = value;
            }
        }

        /// <summary>
        /// The "LastMessageTime" property represents the date and time of the last message in the chat.
        /// It gets or sets the date and time of the last message in the chat.
        /// </summary>
        /// <value>
        /// The date and time of the last message in the chat.
        /// </value>
        public DateTime? LastMessageTime
        {
            get
            {
                return _lastMessageTime;
            }
            set
            {
                _lastMessageTime = value;
            }
        }

        /// <summary>
        /// The "LastMessageContent" property represents the content of the last message in the chat.
        /// It gets or sets the content of the last message in the chat.
        /// </summary>
        /// <value>
        /// The content of the last message in the chat.
        /// </value>
        public string LastMessageContent
        {
            get
            {
                return _lastMessageContent;
            }
            set
            {
                _lastMessageContent = value;
            }
        }

        /// <summary>
        /// The "LastMessageSenderName" property represents the name of the sender of the last message in the chat.
        /// It gets or sets the name of the sender of the last message in the chat.
        /// </summary>
        /// <value>
        /// The name of the sender of the last message in the chat.
        /// </value>
        public string LastMessageSenderName
        {
            get
            {
                return _lastMessageSenderName;
            }
            set
            {
                _lastMessageSenderName = value;
            }
        }

        /// <summary>
        /// The "ChatParticipants" property represents the list of participants in the chat.
        /// It gets or sets the list of participants in the chat.
        /// </summary>
        /// <value>
        /// The list of participants in the chat.
        /// </value>
        public List<ChatParticipant> ChatParticipants
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

        #endregion

        #region Public Methods

        /// <summary>
        /// The "UserExist" method checks if a user with the specified username exists among the chat participants.
        /// </summary>
        /// <param name="username">The username to check for.</param>
        /// <returns>True if the user exists, otherwise false.</returns>
        /// <remarks>
        /// This method iterates through the chat participants to check if any of their usernames match the specified username.
        /// If a match is found, the method returns true; otherwise, it returns false.
        /// </remarks>
        public bool UserExist(string username)
        {
            string chatParticipantName;
            foreach (ChatParticipant chatParticipant in _chatParticipants)
            {
                chatParticipantName = chatParticipant.Username;
                if (chatParticipantName == username)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
