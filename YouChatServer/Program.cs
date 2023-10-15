using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            Application.ApplicationExit += new EventHandler(ApplicationExitHandler);

            System.Net.IPAddress localAdd = System.Net.IPAddress.Parse(ipAddress);
            TcpListener listener = new TcpListener(localAdd, portNo);
            Console.WriteLine("Simple TCP Server");
            Console.WriteLine("Listening to ip {0} port: {1}", ipAddress, portNo);
            Console.WriteLine("Server is ready.");
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
                    Thread t = new Thread(() => StartClient(tcp));
                    t.Start();
                }
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
        private static void ApplicationExitHandler(object sender, EventArgs e)
        {
            // Add a "server down" message to the log before closing
            Logger.LogInfo("Server down. Application is shutting down...");

            // Perform any necessary cleanup before the application exits
            Logger.CloseLogFiles();
        }
    }
}
