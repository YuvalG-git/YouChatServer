using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.ChatHandler
{
    public class GroupChat : Chat
    {
        private string _chatName;
        public byte[] _chatProfilePicture;
        public GroupChat(string chatTagLineId, string messageHistory, DateTime? lastMessageTime, string lastMessageContent, List<ChatParticipant> chatParticipants, string chatName, byte[] chatProfilePicture) : base(chatTagLineId, messageHistory, lastMessageTime, lastMessageContent, chatParticipants)
        {
            _chatName = chatName;
            _chatProfilePicture = chatProfilePicture;
        }

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
    }
}
