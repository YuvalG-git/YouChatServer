using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    public class ProfilePictureUpdate
    {
        private string _username;
        private string _profilePictureId;

        public ProfilePictureUpdate(string username, string profilePictureId)
        {
            _username = username;
            _profilePictureId = profilePictureId;
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
        public string ProfilePictureId
        {
            get
            {
                return _profilePictureId;
            }
            set
            {
                _profilePictureId = value;
            }
        }
    }
}
