using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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
            System.Net.IPAddress localAdd = System.Net.IPAddress.Parse(ipAddress);
            TcpListener listener = new TcpListener(localAdd, portNo);
            Console.WriteLine("Simple TCP Server");
            Console.WriteLine("Listening to ip {0} port: {1}", ipAddress, portNo);
            Console.WriteLine("Server is ready.");
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
                if (tcp != null)
                {
                    Thread t = new Thread(() => StartClient(tcp));
                    t.Start();
                }
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

    }
}
