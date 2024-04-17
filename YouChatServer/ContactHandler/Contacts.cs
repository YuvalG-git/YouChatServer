using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.ContactHandler
{
    public class Contacts
    {
        private List<ContactDetails> _contacts;

        public Contacts(List<ContactDetails> contacts)
        {
            _contacts = contacts;
        }
        public List<ContactDetails> ContactList
        {
            get
            {
                return _contacts;
            }
            set
            {
                _contacts = value;
            }
        }
    }
}
