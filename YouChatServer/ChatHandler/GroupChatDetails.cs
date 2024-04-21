﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.ChatHandler
{
    public class GroupChatDetails : ChatDetails
    {
        private string _chatName;
        public byte[] _chatProfilePicture;
        public GroupChatDetails(string chatTagLineId, DateTime? lastMessageTime, string lastMessageContent, string lastMessageSenderName, List<ChatParticipant> chatParticipants, string chatName, byte[] chatProfilePicture) : base(chatTagLineId, lastMessageTime, lastMessageContent, lastMessageSenderName, chatParticipants)
        {
            _chatName = chatName;
            _chatProfilePicture = chatProfilePicture;
        }

        public string ChatName
        {
            get
            {
                return _chatName;
            }
            set
            {
                _chatName= value;
            }
        }
        public byte[] ChatProfilePicture
        {
            get
            {
                return _chatProfilePicture;
            }
            set
            {
                _chatProfilePicture = value;
            }
        }
    }
}