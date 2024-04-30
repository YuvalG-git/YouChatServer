using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.UdpHandler
{
    /// <summary>
    /// The "AudioUdpHandler" class manages audio communication over UDP, including handling encryption and decryption of audio data.
    /// </summary>
    /// <remarks>
    /// This class provides static methods and fields for initializing and managing a UDP client for audio communication.
    /// It includes methods for starting the UDP client, receiving encrypted audio data, decrypting it, and forwarding it to the appropriate client.
    /// </remarks>
    internal class AudioUdpHandler
    {
        #region Private Static Fields

        /// <summary>
        /// The static UdpClient "AudioUdpClient" is used for audio communication over UDP.
        /// </summary>
        private static UdpClient AudioUdpClient;

        #endregion

        #region Private Const Fields

        /// <summary>
        /// The constant "AudioUdpClientPort" specifies the port number used by the AudioUdpClient for communication.
        /// </summary>
        private const int AudioUdpClientPort = 11000;

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
        /// The "StartAudioUdpClient" method initializes and starts a UDP client for audio communication.
        /// </summary>
        /// <remarks>
        /// This method creates a new UdpClient instance listening on port 11000 for incoming audio data.
        /// It then begins asynchronously receiving audio data using the BeginReceive method,
        /// with the callback method ReceiveAudioUdpMessage to handle received data.
        /// </remarks>
        public static void StartAudioUdpClient()
        {
            // Initialize a new UdpClient listening on port 11000
            AudioUdpClient = new UdpClient(AudioUdpClientPort);

            // Begin asynchronously receiving audio data
            AudioUdpClient.BeginReceive(new AsyncCallback(ReceiveAudioUdpMessage), null);
        }

        /// <summary>
        /// The "ReceiveAudioUdpMessage" method asynchronously receives encrypted audio data from the UDP client, decrypts it, and forwards it to the appropriate client.
        /// </summary>
        /// <param name="ar">The result of the asynchronous operation.</param>
        /// <remarks>
        /// This method continuously receives encrypted audio data from the UDP client using a while loop.
        /// It decrypts the received data using the encryption key associated with the client, obtained from the clientKeys dictionary.
        /// The decrypted data is then re-encrypted using the encryption key of the recipient client, obtained from the EndPoints and clientKeys dictionaries.
        /// Finally, the re-encrypted data is sent to the recipient client using the UDP client.
        /// </remarks>
        private static void ReceiveAudioUdpMessage(IAsyncResult ar)
        {
            while (true)
            {
                try
                {
                    // Get the client's endpoint and receive encrypted data
                    IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] encryptedData = AudioUdpClient.Receive(ref clientEndPoint);

                    // Decrypt the received data using the client's encryption key
                    string key = clientKeys[clientEndPoint];
                    byte[] decryptedData = Encryption.Encryption.DecryptDataToBytes(key, encryptedData);

                    // Get the recipient client's endpoint and encryption key
                    IPEndPoint friendIpEndPoint = EndPoints[clientEndPoint];
                    string friendKey = clientKeys[friendIpEndPoint];

                    // Re-encrypt the decrypted data using the recipient client's encryption key
                    byte[] serverEncryptedData = Encryption.Encryption.EncryptDataToBytes(friendKey, decryptedData);

                    // Send the re-encrypted data to the recipient client
                    AudioUdpClient.Send(serverEncryptedData, serverEncryptedData.Length, friendIpEndPoint);
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
