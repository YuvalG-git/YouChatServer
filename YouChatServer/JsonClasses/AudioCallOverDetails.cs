using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    internal class AudioCallOverDetails
    {
        private string _chatId;
        private int _socketPort;

        public AudioCallOverDetails(string chatId, int socketPort)
        {
            _chatId = chatId;
            _socketPort = socketPort;
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
        public int SocketPort
        {
            get
            {
                return _socketPort;
            }
            set
            {
                _socketPort = value;
            }
        }
    }
}
