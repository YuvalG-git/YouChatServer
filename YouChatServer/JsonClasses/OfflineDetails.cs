using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    public class OfflineDetails
    {
        private string _username;
        private DateTime _lastSeenTime;

        public OfflineDetails(string username, DateTime lastSeenTime)
        {
            _username = username;
            _lastSeenTime = lastSeenTime;
        }
        public string Username
        {
            get 
            { 
                return _username;
            }
            set 
            { 
                _username = value;
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
    }
}
