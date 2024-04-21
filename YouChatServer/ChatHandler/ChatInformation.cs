using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.ChatHandler
{
    internal class ChatInformation
    {
        private ChatDetails _chatDetails;
        private string _messageHistoryPath;

        public ChatInformation(ChatDetails chatDetails, string messageHistoryPath)
        {
            _chatDetails = chatDetails;
            _messageHistoryPath = messageHistoryPath;
        }
        public ChatDetails ChatDetails
        {
            get 
            { 
                return _chatDetails;
            }
            set 
            { 
                _chatDetails = value;
            }
        }
        public string MessageHistoryPath
        {
            get
            {
                return _messageHistoryPath;
            }  
            set
            {
                _messageHistoryPath = value;
            }
        }
    }
}
