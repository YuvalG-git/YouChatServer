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

    internal static class Program
    {
        #region Private Const Fields

        /// <summary>
        /// The integer constant "portNumber" represents the port number used for the server connection.
        /// </summary>
        private const int portNumber = 1500;

        /// <summary>
        /// The string constant "ipAddress" represents the IP address used for the server connection.
        /// </summary>
        private const string ipAddress = "10.100.102.3";

        /// <summary>
        /// The integer constant "maxConnectionsPerIP" represents the maximum number of connections allowed per IP address.
        /// </summary>
        private const int maxConnectionsPerIP = 5;
        
        #endregion

        #region Static Fields

        /// <summary>
        /// The static dictionary "ClientsHistory" maps IP addresses to their connection attempt counters.
        /// </summary>
        private static Dictionary<string, ServerConnectAttemptCounter> ClientsHistory;

        #endregion

        #region Static Methods

        /// <summary>
        /// The "Main" method sets up a TCP listener on the specified IP address and port number
        /// It then enters an infinite loop to accept incoming client connections
        /// For each accepted connection, a new thread is created to handle the client
        /// This allows the server to handle multiple clients concurrently
        /// </summary>
        /// <param name="args">Represents an array of command-line arguments passed to the program when it is executed</param>
        /// 
        /// <summary>
        /// The entry point for the application.
        /// </summary>
        /// <param name="args">The command-line arguments passed to the application.</param>
        /// <remarks>
        /// This method performs the following tasks:
        /// 1. Logs the start of the application using the Logger.LogInfo method.
        /// 2. Sets up the chat handler by calling ChatHandler.ChatHandler.SetChats().
        /// 3. Creates a TCP listener on the specified IP address and port number.
        /// 4. Displays server information including the IP address and port number.
        /// 5. Starts UDP audio and video handlers by calling AudioUdpHandler.StartAudioUdpClient() and VideoUdpHandler.StartVideoUdpClient().
        /// 6. Initializes a dictionary for tracking client connections.
        /// 7. Starts listening for incoming TCP connections in an infinite loop.
        /// 8. Accepts incoming TCP clients and handles them in separate threads using the StartClient method.
        /// If a client exceeds the maximum connection attempts, the connection is rejected, and a message is logged and displayed.
        /// </remarks>
        static void Main(string[] args)
        {
            // Log application start
            Logger.LogInfo("Application started.");

            // Set up chat handler
            ChatHandler.ChatHandler.SetChats();

            // Create TCP listener
            System.Net.IPAddress localAdd = System.Net.IPAddress.Parse(ipAddress);
            TcpListener listener = new TcpListener(localAdd, portNumber);

            // Display server information
            Console.WriteLine("Simple TCP Server");
            Console.WriteLine("Listening to ip {0} port: {1}", ipAddress, portNumber);
            Console.WriteLine("Server is ready.");

            // Start UDP audio and video handlers
            AudioUdpHandler.StartAudioUdpClient();
            VideoUdpHandler.StartVideoUdpClient();

            // Initialize dictionary for tracking client connections
            ClientsHistory = new Dictionary<string, ServerConnectAttemptCounter>();

            // Start listening for incoming connections
            listener.Start();

            // Infinitely loop to accept new connections
            while (true)
            {
                // Accept new TCP client
                TcpClient tcp = listener.AcceptTcpClient();

                if (tcp != null)
                {
                    // Get client IP address
                    string clientIpAndPort = tcp.Client.RemoteEndPoint.ToString();
                    string clientIP = clientIpAndPort.Split(':')[0];

                    // Check if client IP is in dictionary, add if not
                    if (!ClientsHistory.ContainsKey(clientIP))
                    {
                        ClientsHistory.Add(clientIP, new ServerConnectAttemptCounter(maxConnectionsPerIP, TimeSpan.FromMinutes(10)));
                    }
                    // Reject connection if client has exceeded maximum connection attempts
                    else if (!ClientsHistory[clientIP].HandleNewAttempt())
                    {
                        tcp.Close();
                        string DOSMessage = $"(DOS protection) Connection rejected from: {clientIP}";
                        Logger.LogUserDOSProtection(DOSMessage);
                        Console.WriteLine(DOSMessage);
                        continue;
                    }

                    string SocketConnection = "new socket: " + clientIP;
                    Console.WriteLine(SocketConnection);
                    Logger.LogUserSocketConnection(SocketConnection);

                    // Start new thread for client communication
                    Thread t = new Thread(() => StartClient(tcp));
                    t.Start();
                }
            }
        }

        /// <summary>
        /// The "StartClient" method starts handling a new TCP client connection.
        /// </summary>
        /// <param name="tcp">The TcpClient object representing the new client connection.</param>
        /// <remarks>
        /// This method creates a new instance of the Client class to handle the TCP client connection.
        /// The Client class manages the communication between the server and the client.
        /// </remarks>
        public static void StartClient(TcpClient tcp)
        {
            Client user = new Client(tcp);
        }

        #endregion
    }
}
