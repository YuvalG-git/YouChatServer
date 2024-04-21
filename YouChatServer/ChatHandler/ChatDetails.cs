using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.ChatHandler
{
    public class ChatDetails
    {
        private string _chatTagLineId;
        private DateTime? _lastMessageTime;
        private string _lastMessageContent;
        private string _lastMessageSenderName;

        private List<ChatParticipant> _chatParticipants;

        public ChatDetails(string chatTagLineId, DateTime? lastMessageTime, string lastMessageContent, string lastMessageSenderName, List<ChatParticipant> chatParticipants)
        {
            _chatTagLineId = chatTagLineId;
            _lastMessageTime = lastMessageTime;
            _lastMessageContent = lastMessageContent;
            _chatParticipants = chatParticipants;
            _lastMessageSenderName = lastMessageSenderName;
        }

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
    }
}
