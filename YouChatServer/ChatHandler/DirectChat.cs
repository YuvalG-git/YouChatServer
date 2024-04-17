using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.ChatHandler
{
    public class DirectChat : Chat
    {
        public DirectChat(string chatTagLineId, string messageHistory, DateTime? lastMessageTime, string lastMessageContent, List<ChatParticipant> chatParticipants) : base(chatTagLineId, messageHistory, lastMessageTime, lastMessageContent, chatParticipants)
        {
        }
    }
}
