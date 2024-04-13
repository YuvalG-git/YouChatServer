using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    internal class JsonObject
    {
        private Enum messageType;
        private object messageBody;

        public JsonObject(Enum messageType, object messageBody)
        {
            this.messageType = messageType;
            this.messageBody = messageBody;
        }
        public Enum MessageType
        {
            get
            {
                return messageType;
            }
            set
            {
                messageType = value;
            }
        }
        public object MessageBody
        {
            get
            {
                return messageBody;
            }
            set
            {
                messageBody = value;
            }
        }
    }
}
