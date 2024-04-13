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
        private bool _lastSeenProperty;
        private bool _onlineProperty;
        private bool _profilePictureProperty;
        private bool _statusProperty;
        private int _textSizeProperty;
        private int _messageGapProperty;
        private bool _enterKeyPressedProperty;
        private string _tagLineId;

        public UserDetails(string username, string profilePicture, string profileStatus, bool lastSeenProperty, bool onlineProperty, bool profilePictureProperty, bool statusProperty, int textSizeProperty, int messageGapProperty, bool enterKeyPressedProperty, string tagLineId)
        {
            _username = username;
            _profilePicture = profilePicture;
            _profileStatus = profileStatus;
            _lastSeenProperty = lastSeenProperty;
            _onlineProperty = onlineProperty;
            _profilePictureProperty = profilePictureProperty;
            _statusProperty = statusProperty;
            _textSizeProperty = textSizeProperty;
            _messageGapProperty = messageGapProperty;
            _enterKeyPressedProperty = enterKeyPressedProperty;
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

        public bool LastSeenProperty
        {
            get
            {
                return _lastSeenProperty;
            }
            set
            {
                _lastSeenProperty = value;
            }
        }

        public bool OnlineProperty
        {
            get
            {
                return _onlineProperty;
            }
            set
            {
                _onlineProperty = value;
            }
        }

        public bool ProfilePictureProperty
        {
            get
            {
                return _profilePictureProperty;
            }
            set
            {
                _profilePictureProperty = value;
            }
        }

        public bool StatusProperty
        {
            get
            {
                return _statusProperty;
            }
            set
            {
                _statusProperty = value;
            }
        }

        public int TextSizeProperty
        {
            get
            {
                return _textSizeProperty;
            }
            set
            {
                _textSizeProperty = value;
            }
        }

        public int MessageGapProperty
        {
            get
            {
                return _messageGapProperty;
            }
            set
            {
                _messageGapProperty = value;
            }
        }

        public bool EnterKeyPressedProperty
        {
            get
            {
                return _enterKeyPressedProperty;
            }
            set
            {
                _enterKeyPressedProperty = value;
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
