using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses.MessageClasses
{
    public class MessageHistory
    {
        private List<Message> _messages;

        public MessageHistory(List<Message> messages)
        {
            _messages = messages;
        }
        public List<Message> Messages
        {
            get
            {
                return _messages;
            }
            set
            {
                _messages = value;
            }
        }
    }
}
