﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.ChatHandler
{
    public class ChatParticipant
    {
        private string _username;
        private string _profilePicture;

        public ChatParticipant(string username, string profilePicture)
        {
            _username = username;
            _profilePicture = profilePicture;
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }
        public string ProfilePicture
        {
            get { return _profilePicture; }
            set { _profilePicture = value; }
        }
    }
}
