using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses.MessageClasses
{
    public class MessageData
    {
        private string _sender;
        private string _type;
        private string _content;
        private string _date;

        public MessageData(string sender, string type, string content, string date)
        {
            _sender = sender;
            _type = type;
            _content = content;
            _date = date;
        }
        public string Sender
        {
            get
            {
                return _sender;
            }
            set
            {
                _sender = value;
            }
        }
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
            }
        }
        public string Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
            }
        }
    }
}
