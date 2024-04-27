using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    internal class UserDetails
    {
        private string _username;
        private string _profilePicture;
        private string _profileStatus;
        private string _tagLineId;

        public UserDetails(string username, string profilePicture, string profileStatus, string tagLineId)
        {
            _username = username;
            _profilePicture = profilePicture;
            _profileStatus = profileStatus;
            _tagLineId = tagLineId;
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

        public string ProfileStatus
        {
            get
            {
                return _profileStatus;
            }
            set
            {
                _profileStatus = value;
            }
        }

        public string TagLineId
        {
            get
            {
                return _tagLineId;
            }
            set
            {
                _tagLineId = value;
            }
        }
    }
}
