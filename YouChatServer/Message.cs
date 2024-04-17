using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer
{
    public enum MessageType
    {
        Text,      // For sending a text message
        Image,     // For sending an image
        Contact // For sending a contact
    }
    internal class Message
    {

        private string UserSenderName { get; set; }
        private string UserMessage { get; set; }
        //private string MessageType { get; set; }
        private string MessageTime { get; set; }

        private MessageType MessageType { get; set; }
        //private int MessageType; //text message, image, file, contact (text message that contains different useful details)...


        public Message(string userSenderName, string userMessage, string messageTime, MessageType messageType)
        {
            this.UserSenderName = userSenderName;
            this.UserMessage = userMessage;
            this.MessageTime = messageTime;
            this.MessageType = messageType;
        }
    }
}
