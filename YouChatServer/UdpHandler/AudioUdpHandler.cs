using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.UdpHandler
{
    internal class AudioUdpHandler
    {
        private static UdpClient AudioUdpClient;//udp audio listener object

        public static void StartAudioUdpClient()
        {
            AudioUdpClient = new UdpClient(11000);//udp listens to port 11000
            AudioUdpClient.BeginReceive(new AsyncCallback(ReceiveAudioUdpMessage), null);
        }

        static private void ReceiveAudioUdpMessage(IAsyncResult ar)
        {
            while (true)
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

                byte[] imageData = AudioUdpClient.Receive(ref clientEndPoint);
                AudioUdpClient.Send(imageData, imageData.Length, clientEndPoint);
            }
        }

    }
}
