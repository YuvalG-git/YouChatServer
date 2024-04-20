using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    public class Message
    {
        private string _messageSenderName;
        private string _chatId;
        private object _messageContent; 
        private DateTime _messageDateAndTime;

        public Message(string messageSenderName, string chatId, object messageContent, DateTime messageDateAndTime)
        {
            _messageSenderName = messageSenderName;
            _chatId = chatId;
            _messageContent = messageContent;
            _messageDateAndTime = messageDateAndTime;
        }

        public object MessageContent
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
        public string MessageSenderName
        {
            get
            {
                return _messageSenderName;
            }
            set
            {
                _messageSenderName = value;
            }
        }
    }
}
