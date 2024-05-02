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
    /// The "VideoUdpHandler" class manages video communication over UDP, including handling encryption and decryption of video data.
    /// </summary>
    /// <remarks>
    /// This class provides static methods and fields for initializing and managing a UDP client for video communication.
    /// It includes methods for starting the UDP client, receiving encrypted video data, decrypting it, and forwarding it to the appropriate client.
    /// </remarks>
    internal class VideoUdpHandler
    {
        #region Private Static Fields

        /// <summary>
        /// The static UdpClient "VideoUdpClient" is used for video communication over UDP.
        /// </summary>
        private static UdpClient VideoUdpClient;

        #endregion

        #region Private Const Fields

        /// <summary>
        /// The constant "VideoUdpClientPort" specifies the port number used by the VideoUdpClient for communication.
        /// </summary>
        private const int VideoUdpClientPort = 12000;

        #endregion

        #region Public Static Fields

        /// <summary>
        /// The static Dictionary "EndPoints" maps client IPEndPoints to server IPEndPoints for communication.
        /// </summary>
        public static Dictionary<IPEndPoint, IPEndPoint> EndPoints = new Dictionary<IPEndPoint, IPEndPoint>();

        /// <summary>
        /// The static Dictionary "clientKeys" maps client IPEndPoints to their corresponding keys for authentication.
        /// </summary>
        public static Dictionary<IPEndPoint, string> clientKeys = new Dictionary<IPEndPoint, string>();

        #endregion

        #region Public Static Methods

        /// <summary>
        /// The "StartVideoUdpClient" method initializes and starts a UDP client for video communication.
        /// </summary>
        /// <remarks>
        /// This method creates a new UdpClient instance listening on port 12000 for incoming video data.
        /// It then begins asynchronously receiving video data using the BeginReceive method,
        /// with the callback method ReceiveVideoUdpMessage to handle received data.
        /// </remarks>
        public static void StartVideoUdpClient()
        {
            // Initialize a new UdpClient listening on port 12000
            VideoUdpClient = new UdpClient(VideoUdpClientPort);

            // Begin asynchronously receiving video data
            VideoUdpClient.BeginReceive(new AsyncCallback(ReceiveVideoUdpMessage), null);
        }

        /// <summary>
        /// The "ReceiveVideoUdpMessage" method asynchronously receives encrypted video data from the UDP client, decrypts it, and forwards it to the appropriate client.
        /// </summary>
        /// <param name="ar">The result of the asynchronous operation.</param>
        /// <remarks>
        /// This method continuously receives encrypted video data from the UDP client using a while loop.
        /// It decrypts the received data using the encryption key associated with the client, obtained from the clientKeys dictionary.
        /// The decrypted data is then re-encrypted using the encryption key of the recipient client, obtained from the EndPoints and clientKeys dictionaries.
        /// Finally, the re-encrypted data is sent to the recipient client using the UDP client.
        /// </remarks>
        private static void ReceiveVideoUdpMessage(IAsyncResult ar)
        {
            while (true)
            {
                try
                {
                    // Get the client's endpoint and receive encrypted data
                    IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] encryptedData = VideoUdpClient.Receive(ref clientEndPoint);

                    // Decrypt the received data using the client's encryption key
                    string key = clientKeys[clientEndPoint];
                    byte[] decryptedData = Encryption.AESServiceProvider.DecryptDataToBytes(key, encryptedData);

                    // Get the recipient client's endpoint and encryption key
                    IPEndPoint friendIpEndPoint = EndPoints[clientEndPoint];
                    string friendKey = clientKeys[friendIpEndPoint];

                    // Re-encrypt the decrypted data using the recipient client's encryption key
                    byte[] serverEncryptedData = Encryption.AESServiceProvider.EncryptDataToBytes(friendKey, decryptedData);

                    // Send the re-encrypted data to the recipient client
                    VideoUdpClient.Send(serverEncryptedData, serverEncryptedData.Length, friendIpEndPoint);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        #endregion
    }
}
