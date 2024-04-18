using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChatServer.ChatHandler;
using YouChatServer.ContactHandler;

namespace YouChatServer.JsonClasses
{
    public class ContactAndChat
    {
        private ChatHandler.ChatDetails _chat;
        private ContactDetails _contactDetails;

        public ContactAndChat(ChatHandler.ChatDetails chat, ContactDetails contactDetails)
        {
            _chat = chat;
            _contactDetails = contactDetails;
        }

        public ChatHandler.ChatDetails Chat
        {
            get
            {
                return _chat;
            }
            set
            {
                _chat = value;
            }
        }
        public ContactDetails ContactDetails
        {
            get
            {
                return _contactDetails;
            }
            set
            {
                _contactDetails = value;
            }
        }
    }
}
