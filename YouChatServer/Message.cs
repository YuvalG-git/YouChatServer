using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer
{
    internal class Message
    {
        private string UserSenderName { get; set; }
        private string UserMessage { get; set; }
        //private string MessageType { get; set; }
        private string MessageTime { get; set; }

        private string MessageType; //text message, image, file, contact (text message that contains different useful details)...

        public Message(string userSenderName, string userMessage, string messageTime)
        {
            this.UserSenderName = userSenderName;
            this.UserMessage = userMessage;
            this.MessageTime = messageTime;
        }
    }
}
