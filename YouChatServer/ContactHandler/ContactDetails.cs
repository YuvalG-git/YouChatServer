using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.ContactHandler
{
    public class ContactDetails
    {
        private string _name;
        private string _id;
        private string _profilePicture;
        private string _status;
        private DateTime _lastSeenTime;
        private bool _online;

        public ContactDetails(string name, string id, string profilePicture, string status, DateTime lastSeenTime, bool online)
        {
            _name = name;
            _id = id;
            _profilePicture = profilePicture;
            _status = status;
            _lastSeenTime = lastSeenTime;
            _online = online;
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }
        public string ProfilePicture
        {
            get
            {
                return _profilePicture;
            }
            set
            {
                _profilePicture = value;
            }
        }
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }
        public DateTime LastSeenTime
        {
            get
            {
                return _lastSeenTime;
            }
            set
            {
                _lastSeenTime = value;
            }
        }
        public bool Online
        {
            get
            {
                return _online;
            }
            set
            {
                _online = value;
            }
        }
    }
}
