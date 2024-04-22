using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.UdpHandler
{
    internal class VideoUdpHandler
    {
        private static UdpClient VideoUdpClient;
        public static Dictionary<IPEndPoint, IPEndPoint> EndPoints = new Dictionary<IPEndPoint, IPEndPoint>();
        public static Dictionary<IPEndPoint, string> clientKeys = new Dictionary<IPEndPoint, string>();

        public static void StartAudioUdpClient()
        {
            VideoUdpClient = new UdpClient(12000);
            VideoUdpClient.BeginReceive(new AsyncCallback(ReceiveAudioUdpMessage), null);
        }

        static private void ReceiveAudioUdpMessage(IAsyncResult ar)
        {
            while (true)
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

                byte[] encryptedData = VideoUdpClient.Receive(ref clientEndPoint);
                string key = clientKeys[clientEndPoint];
                // Decrypt the received data using the client's key
                byte[] decryptedData = Encryption.Encryption.DecryptDataToBytes(key, encryptedData);
                IPEndPoint friendIpEndPoint = EndPoints[clientEndPoint];
                string friendKey = clientKeys[friendIpEndPoint];
                byte[] serverEncryptedData = Encryption.Encryption.EncryptDataToBytes(friendKey, decryptedData);
                VideoUdpClient.Send(serverEncryptedData, serverEncryptedData.Length, friendIpEndPoint);
            }
        }
    }
}
