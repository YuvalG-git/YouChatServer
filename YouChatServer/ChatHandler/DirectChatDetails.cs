using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.ChatHandler
{
    /// <summary>
    /// The "DirectChatDetails" class represents the details of a direct chat between two participants.
    /// </summary>
    /// <remarks>
    /// This class inherits from the base ChatDetails class and adds functionality specific to direct chats.
    /// </remarks>
    public class DirectChatDetails : ChatDetails
    {
        #region Constructors

        /// <summary>
        /// The "DirectChatDetails" constructor initializes a new instance of the <see cref="DirectChatDetails"/> class with the specified chat tag line ID, last message time, last message content, last message sender name, and chat participants.
        /// </summary>
        /// <param name="chatTagLineId">The ID of the chat tag line.</param>
        /// <param name="lastMessageTime">The time of the last message in the chat.</param>
        /// <param name="lastMessageContent">The content of the last message in the chat.</param>
        /// <param name="lastMessageSenderName">The name of the sender of the last message in the chat.</param>
        /// <param name="chatParticipants">The list of participants in the chat.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the DirectChatDetails class, which represents the details of a direct chat between two participants.
        /// It inherits the base class constructor to initialize the chat details.
        /// </remarks>
        public DirectChatDetails(string chatTagLineId, DateTime? lastMessageTime, string lastMessageContent, string lastMessageSenderName, List<ChatParticipant> chatParticipants) : base(chatTagLineId, lastMessageTime, lastMessageContent, lastMessageSenderName, chatParticipants)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The "GetOtherUserName" method retrieves the username of the other participant in a chat, given the username of one participant.
        /// </summary>
        /// <param name="username">The username of one participant in the chat.</param>
        /// <returns>The username of the other participant in the chat, or an empty string if no other participant is found.</returns>
        /// <remarks>
        /// This method iterates through the list of chat participants and returns the username of the first participant found
        /// that is not equal to the specified username. If no other participant is found, it returns an empty string.
        /// </remarks>
        public string GetOtherUserName(string username)
        {
            string chatParticipantName;
            foreach (ChatParticipant chatParticipant in ChatParticipants)
            {
                chatParticipantName = chatParticipant.Username;
                if (chatParticipantName != username)
                {
                    return chatParticipantName;
                }
            }
            return "";
        }

        #endregion
    }
}
