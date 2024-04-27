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

        const int portNo = 1500;

        private const string ipAddress = "10.100.102.3";


        private const int maxConnectionsPerIP = 5; // Maximum connections allowed per IP
        public static Dictionary<string, int> connectionCounts = new Dictionary<string, int>();
        public static List<IPEndPoint[]> videoCallMembersDetails = new List<IPEndPoint[]>(); //another soloution might be a dictonary storing as key an object contain name and ip and as value the object of the person you are in a call with...
                                                                                             //public static Dictionary<Guid, VideoCallMembers> VideoCalls = new Dictionary<Guid, VideoCallMembers>();
        private static Dictionary<string, ServerConnectAttemptCounter> ClientsHistory;

        /// <summary>
        /// The "Main" method sets up a TCP listener on the specified IP address and port number
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
            VideoUdpHandler.StartVideoUdpClient();

            ClientsHistory = new Dictionary<string, ServerConnectAttemptCounter>();
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

                if (tcp != null)
                {
                    string clientIpAndPort = tcp.Client.RemoteEndPoint.ToString();
                    string clientIP = clientIpAndPort.Split(':')[0];

                    if (!ClientsHistory.ContainsKey(clientIP))
                    {
                        ClientsHistory.Add(clientIP, new ServerConnectAttemptCounter(maxConnectionsPerIP, TimeSpan.FromMinutes(10)));
                    }
                    else if (!ClientsHistory[clientIP].NewAttempt())
                    {
                        tcp.Close();
                        Console.WriteLine($"(DOS protection) Connection rejected from: {clientIP}");
                        continue;
                    }
                    Logger.LogInfo("Application started.");

                    Console.WriteLine("new socket: " + clientIP);

                    Thread t = new Thread(() => StartClient(tcp));
                    t.Start();
                }
            }
        }



        /// <summary>
        /// The "StartClient" method is responsible for starting a new client connection by creating an instance of the Client class and initializing it with the provided TcpClient object
        /// </summary>
        /// <param name="tcp">Represents the TCP client that connects to the server</param>
        public static void StartClient(TcpClient tcp)
        {
            Client user = new Client(tcp);
        }

    }
}
