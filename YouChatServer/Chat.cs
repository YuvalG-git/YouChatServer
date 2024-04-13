using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer
{
    internal class Chat
    {
        private XmlFileManager _FileManager;
        private Hashtable _chatClients;

        public Chat(XmlFileManager fileManager, Hashtable chatClients)
        {
            _FileManager = fileManager;
            _chatClients = chatClients;
        }
        public XmlFileManager FileManager
        {
            get
            {
                return _FileManager;
            }
            set
            {
                _FileManager = value;
            }
        }
        public Hashtable ChatClients
        {
            get
            {
                return _chatClients;
            }
            set
            {
                _chatClients = value;
            }
        }

    }
}
