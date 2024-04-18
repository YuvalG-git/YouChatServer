using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.ChatHandler
{
    internal class Chats
    {
        private List<ChatDetails> _chats;

        public Chats(List<ChatDetails> chats)
        {
            _chats = chats;
        }
        public List<ChatDetails> ChatList
        {
            get
            {
                return _chats;
            }
            set
            {
                _chats = value;
            }
        }
    }
}
