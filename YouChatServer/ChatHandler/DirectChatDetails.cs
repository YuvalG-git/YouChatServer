using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.ChatHandler
{
    public class DirectChatDetails : ChatDetails
    {
        public DirectChatDetails(string chatTagLineId, string messageHistory, DateTime? lastMessageTime, string lastMessageContent, string lastMessageSenderName, List<ChatParticipant> chatParticipants) : base(chatTagLineId, messageHistory, lastMessageTime, lastMessageContent, lastMessageSenderName, chatParticipants)
        {
        }
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
    }
}
