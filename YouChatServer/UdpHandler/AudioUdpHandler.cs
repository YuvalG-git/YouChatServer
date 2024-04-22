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
        public static Dictionary<IPEndPoint, IPEndPoint> EndPoints = new Dictionary<IPEndPoint, IPEndPoint>();
        public static Dictionary<IPEndPoint, string> clientKeys = new Dictionary<IPEndPoint, string>();

        public static void StartAudioUdpClient()
        {
            AudioUdpClient = new UdpClient(11000);//udp listens to port 11000
            AudioUdpClient.BeginReceive(new AsyncCallback(ReceiveAudioUdpMessage), null);
        }

        static private void ReceiveAudioUdpMessage(IAsyncResult ar)
        {
            while (true)
            {
                try
                {
                    IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

                    byte[] encryptedData = AudioUdpClient.Receive(ref clientEndPoint);
                    string key = clientKeys[clientEndPoint];
                    // Decrypt the received data using the client's key
                    byte[] decryptedData = Encryption.Encryption.DecryptDataToBytes(key, encryptedData);
                    IPEndPoint friendIpEndPoint = EndPoints[clientEndPoint];
                    string friendKey = clientKeys[friendIpEndPoint];
                    byte[] serverEncryptedData = Encryption.Encryption.EncryptDataToBytes(friendKey, decryptedData);
                    AudioUdpClient.Send(serverEncryptedData, serverEncryptedData.Length, friendIpEndPoint);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }
        }
    }
}
