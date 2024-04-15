using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChatServer.UserDetails;

namespace YouChatServer.JsonClasses
{
    internal class PastFriendRequests
    {
        private List<PastFriendRequest> _friendRequest;

        public PastFriendRequests(List<PastFriendRequest> friendRequest)
        {
            _friendRequest = friendRequest;
        }

        public List<PastFriendRequest> FriendRequests
        {
            get
            {
                return _friendRequest;
            }
            set
            {
                _friendRequest = value;
            }
        }
    }
}
