using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YouChatServer.ClientAttemptsStateHandler;
using YouChatServer.UdpHandler;
using YouChatServer.UserDetails;

namespace YouChatServer
{
    /// <summary>
    /// The Program class represents the class that starts running when "Start" is pressed
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Represents the port number of the server
        /// </summary>
        const int portNo = 1500;

        /// <summary>
        /// Represents the server's ip number
        /// </summary>
        private const string ipAddress = "10.100.102.3";

        static UdpClient AudioUdpClient;//udp audio listener object
        static UdpClient VideoUdpClient; //udp video/ screen share listener obje
        public static int maxConnectionsPerIP = 5; // Maximum connections allowed per IP
        public static Dictionary<string, int> connectionCounts = new Dictionary<string, int>();
        public static List<IPEndPoint[]> videoCallMembersDetails = new List<IPEndPoint[]>(); //another soloution might be a dictonary storing as key an object contain name and ip and as value the object of the person you are in a call with...
                                                                                             //public static Dictionary<Guid, VideoCallMembers> VideoCalls = new Dictionary<Guid, VideoCallMembers>();
        private static Dictionary<string, ServerConnectAttemptCounter> ClientsHistory;

        /// <summary>
        /// The Main method sets up a TCP listener on the specified IP address and port number
        /// It then enters an infinite loop to accept incoming client connections
        /// For each accepted connection, a new thread is created to handle the client
        /// This allows the server to handle multiple clients concurrently
        /// </summary>
        /// <param name="args">Represents an array of command-line arguments passed to the program when it is executed</param>
        static void Main(string[] args)
        {
            Logger.LogInfo("Application started.");
            ChatHandler.ChatHandler.SetChats();

            //UserDetails.DataCreator.CreateDataBase();
            System.Net.IPAddress localAdd = System.Net.IPAddress.Parse(ipAddress);
            TcpListener listener = new TcpListener(localAdd, portNo);
            Console.WriteLine("Simple TCP Server");
            Console.WriteLine("Listening to ip {0} port: {1}", ipAddress, portNo);
            Console.WriteLine("Server is ready.");
            AudioUdpHandler.StartAudioUdpClient();
            VideoUdpHandler.StartAudioUdpClient();

            ClientsHistory = new Dictionary<string, ServerConnectAttemptCounter>();
            //ReceiveAndEchoImageUDP();
            // Start listen to incoming connection requests
            listener.Start();
            // infinit loop.
            while (true)
            {
                // AcceptTcpClient - Blocking call
                // Execute will not continue until a connection is established
                // We create an instance of ChatClient so the server will be able to 
                // server multiple client at the same time.
                TcpClient tcp = listener.AcceptTcpClient();
                //if (HandleWithDDOS(tcp))
                //{
                //    if (tcp != null)
                //    {
                //        Thread t = new Thread(() => StartClient(tcp));
                //        t.Start();
                //    }
                //}
                if (tcp != null)
                {
                    string clientIpAndPort = tcp.Client.RemoteEndPoint.ToString();
                    string clientIP = clientIpAndPort.Split(':')[0];

                    if (!ClientsHistory.ContainsKey(clientIP))
                    {
                        ClientsHistory.Add(clientIP, new ServerConnectAttemptCounter(5, TimeSpan.FromMinutes(10)));
                    }
                    else if (!ClientsHistory[clientIP].NewAttempt())
                    {
                        tcp.Close();
                        Console.WriteLine($"(DDOS protection) Connection rejected from: {clientIP}");
                        continue;
                    }
                    Console.WriteLine("new socket: " + clientIP);

                    Thread t = new Thread(() => StartClient(tcp));
                    t.Start();
                }
            }
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
        /// <summary>
        /// UDP listen to screen/camera share from client
        /// </summary>
        public static void ReceiveVideoUdpMessage(IAsyncResult ar)
        {
            while (true)
            {
                //IPEndPoint clientEndPoint;
                //string destinationClientId = DetermineDestinationClient(receivedFrame);

                //if (clientEndpoints.TryGetValue(destinationClientId, out IPEndPoint destinationEndPoint))

                //string destinationClientId = DetermineDestinationClient(receivedFrame);

                //if (clientEndpoints.TryGetValue(destinationClientId, out IPEndPoint destinationEndPoint))
                //{
                //    // Send the processed frame to the destination client
                //    udpListener.Send(processedFrameData, processedFrameData.Length, destinationEndPoint);
                //}
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

                byte[] imageData = VideoUdpClient.Receive(ref clientEndPoint);

                // Convert bytes to image
                using (MemoryStream ms = new MemoryStream(imageData))
                {
                    Image receivedImage = Image.FromStream(ms);

                    // Display the received image (optional)
                    Console.WriteLine("Received an image from a client.");
                }
                //this is the code to use:
                /*
                IPEndPoint senderEndpoint = clientEndPoint;
                int senderIndex;
                int recieverIndex;
                IPEndPoint recieveEndpoint;
                int temp;
                foreach (IPEndPoint[] iPEndPoints in videoCallMembersDetails)
                {
                    temp = Array.IndexOf(iPEndPoints, senderEndpoint);
                    if (temp != -1)
                    {
                        senderIndex = temp;
                        recieverIndex = 1 - senderIndex;
                        recieveEndpoint = iPEndPoints[recieverIndex];
                        VideoUdpClient.Send(imageData, imageData.Length, recieveEndpoint);

                        break;
                    }
                }
                */
                //if (senderIndex >= 0)
                //{
                //    // Forward the frame to the other client(s)
                //    for (int i = 0; i < clientEndpoints.Count; i++)
                //    {
                //        if (i != senderIndex)
                //        {
                //            // Forward the frame to the other clients
                //            udpListener.Send(receivedFrame, receivedFrame.Length, clientEndpoints[i]);
                //        }
                //    }
                //}
                //IPEndPoint iPEndPoint;

                //foreach (IPEndPoint[] iPEndPoints in videoCallMembersDetails)
                //{
                //    if (iPEndPoints[0] == (clientEndPoint))
                //    {
                //        iPEndPoint = iPEndPoints[1];
                //    }
                //    else if (iPEndPoints[1] == (clientEndPoint))
                //    {
                //        iPEndPoint = iPEndPoints[0];
                //    }
                //}
                //VideoUdpClient.Send(imageData, imageData.Length, iPEndPoint);

                // Send the image back to the same client
                VideoUdpClient.Send(imageData, imageData.Length, clientEndPoint);
            }
        }
        public static void ReceiveAndEchoImageUDP()
        {
            UdpClient udpListener = new UdpClient(12345); // Listen on the specified port

            while (true)
            {
                //string destinationClientId = DetermineDestinationClient(receivedFrame);

                //if (clientEndpoints.TryGetValue(destinationClientId, out IPEndPoint destinationEndPoint))
                //{
                //    // Send the processed frame to the destination client
                //    udpListener.Send(processedFrameData, processedFrameData.Length, destinationEndPoint);
                //}
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] imageData = udpListener.Receive(ref clientEndPoint);

                // Convert bytes to image
                using (MemoryStream ms = new MemoryStream(imageData))
                {
                    Image receivedImage = Image.FromStream(ms);

                    // Display the received image (optional)
                    Console.WriteLine("Received an image from a client.");
                }

                // Send the image back to the same client
                udpListener.Send(imageData, imageData.Length, clientEndPoint);
            }
        }

        /// <summary>
        /// The StartClient method is responsible for starting a new client connection by creating an instance of the Client class and initializing it with the provided TcpClient object
        /// </summary>
        /// <param name="tcp">Represents the TCP client that connects to the server</param>
        public static void StartClient(TcpClient tcp)
        {
            Client user = new Client(tcp);
        }
        private static bool HandleWithDDOS(TcpClient client)
        {
            string clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

            if (IsConnectionAllowed(clientIP))
            {
                Console.WriteLine($"Accepted connection from {clientIP}");
                client.Close();
                return true;
            }
            else
            {
                Console.WriteLine($"Connection from {clientIP} blocked (rate limit exceeded)");
                client.Close();
                return false;
            }
        }
        static bool IsConnectionAllowed(string clientIP)
        {
            lock (connectionCounts)
            {
                if (connectionCounts.ContainsKey(clientIP))
                {
                    connectionCounts[clientIP]++;
                    if (connectionCounts[clientIP] > maxConnectionsPerIP)
                    {
                        return false; // Rate limit exceeded
                    }
                }
                else
                {
                    connectionCounts[clientIP] = 1;
                }
            }

            // Remove the IP from the count after a delay to reset the rate limit
            ThreadPool.QueueUserWorkItem(state =>
            {
                Thread.Sleep(60000); // 60 seconds (adjust as needed)
                lock (connectionCounts)
                {
                    connectionCounts[clientIP]--;
                    if (connectionCounts[clientIP] == 0)
                    {
                        connectionCounts.Remove(clientIP);
                    }
                }
            });

            return true;
        }

    }
}
