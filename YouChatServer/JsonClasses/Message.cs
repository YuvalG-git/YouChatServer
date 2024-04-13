using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    internal class Message
    {
        private string _messageContent;
        private DateTime _messageDateAndTime;
        private string _chatId;

        public Message(string messageContent, DateTime messageDateAndTime, string chatId)
        {
            _messageContent = messageContent;
            _messageDateAndTime = messageDateAndTime;
            _chatId = chatId;
        }
        public string MessageContent
        { 
            get 
            { 
                return _messageContent;
            } 
            set
            {
                _messageContent = value;
            }
        }
        public DateTime MessageDateAndTime
        {
            get
            {
                return _messageDateAndTime;
            }
            set
            {
                _messageDateAndTime = value;
            }
        }
        public string ChatId
        {
            get
            {
                return _chatId;
            }
            set
            {
                _chatId = value;
            }
        }
    }
}
