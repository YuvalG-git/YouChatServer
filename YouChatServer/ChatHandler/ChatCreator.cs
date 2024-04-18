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
    public class ChatCreator
    {
        private string _chatName;
        private List<string> _chatParticipants;
        private byte[] _chatProfilePictureBytes;
        public ChatCreator(string name, List<string> chatParticipants, byte[] chatProfilePictureBytes)
        {
            this._chatName = name;
            this._chatParticipants = chatParticipants;
            this._chatProfilePictureBytes = chatProfilePictureBytes;
        }
        public string ChatName
        {
            get
            {
                return _chatName;
            }
            set
            {
                _chatName = value;
            }
        }
        public List<string> ChatParticipants
        {
            get
            {
                return _chatParticipants;
            }
            set
            {
                _chatParticipants = value;
            }
        }
        public byte[] ChatProfilePictureBytes
        {
            get
            {
                return _chatProfilePictureBytes;
            }
            set
            {
                _chatProfilePictureBytes = value;
            }
        }
    }
}
