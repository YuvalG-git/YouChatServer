using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace YouChatServer.ChatHandler
{
    internal class ChatCreator
    {
        public string _chatName { get; set; }
        //public List<ContactHandler.Contact> _chatParticipants{ get; set; } //could be nice to save this but the problem is that could be users that arent my friends.. so for now i will use string
        public List<string> _chatParticipants { get; set; }
        public byte[] _chatProfilePictureBytes { get; set; }
        public ChatCreator(string name, List<string> chatParticipants, byte[] chatProfilePictureBytes)
        {
            this._chatName = name;
            this._chatParticipants = new List<string>();
            _chatParticipants = chatParticipants;
            _chatProfilePictureBytes = chatProfilePictureBytes;
        }
    }
}
