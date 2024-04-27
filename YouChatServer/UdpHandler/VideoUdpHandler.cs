using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.UdpHandler
{
    /// <summary>
    /// The "VideoUdpHandler" class manages the UDP video communication between clients.
    /// </summary>
    internal class VideoUdpHandler
    {
        /// <summary>
        /// The UDP client for video communication.
        /// </summary>
        private static UdpClient VideoUdpClient;

        /// <summary>
        /// The dictionary of client endpoints and their corresponding friend endpoints.
        /// </summary>
        public static Dictionary<IPEndPoint, IPEndPoint> EndPoints = new Dictionary<IPEndPoint, IPEndPoint>();

        /// <summary>
        /// The dictionary of client endpoints and their corresponding encryption keys.
        /// </summary>
        public static Dictionary<IPEndPoint, string> clientKeys = new Dictionary<IPEndPoint, string>();

        /// <summary>
        /// The "StartVideoUdpClient" method starts the UDP client for video communication on port 12000.
        /// </summary>
        public static void StartVideoUdpClient()
        {
            VideoUdpClient = new UdpClient(12000);
            VideoUdpClient.BeginReceive(new AsyncCallback(ReceiveVideoUdpMessage), null);
        }

        /// <summary>
        /// The "ReceiveVideoUdpMessage" Callback method for receiving video UDP messages.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        static private void ReceiveVideoUdpMessage(IAsyncResult ar)
        {
            while (true)
            {
                try
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
