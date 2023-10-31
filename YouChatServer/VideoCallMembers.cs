using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer
{
    internal class VideoCallMembers
    {
        public IPEndPoint SenderIPEndPoint { get; set; }
        public IPEndPoint ReceiverIPEndPoint { get; set; }
        // Add other call-related information as needed
        public VideoCallMembers(IPEndPoint sender, IPEndPoint receiver)
        {
            SenderIPEndPoint = sender;
            ReceiverIPEndPoint = receiver;
        }
    }
}
