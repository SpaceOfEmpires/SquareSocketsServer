using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SquareSocketsServer {
    internal class Server {
        private Socket socket; // The socket which will listen to incoming connections

        private IPAddress ip; // Ip address to listen on
        private readonly short port = 11000; // Port to listen on

        public Server() {
            ip = GetIpAddress();
        }

        /// <summary>
        /// Setup the connection and start accepting incomming connections
        /// </summary>
        public void Start() {
            try {
                socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp); // Initialize a new socket that will use an ipv4 address and tcp. Socket types: https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.sockettype?view=netframework-4.7.2
                socket.Bind(new IPEndPoint(ip, port)); // Bind the socket to an endpoint of the ip and port
                socket.Listen(10); // "The maximum length of the pending connections queue": https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.listen?view=netcore-2.2
                socket.BeginAccept(AcceptCallback, socket); // Accept an incomming socket connection. Then run the AsyncCallback to handle the new connection
            } catch (Exception ex) {
                Console.WriteLine("Error" + ex);
            }
        }

        /// <summary>
        /// AsyncCallback that is run when an incomming connection is accepted
        /// </summary>
        private void AcceptCallback(IAsyncResult ar) {
            try {
                Socket newConnection = socket.EndAccept(ar); // Get the socket from the new socket connection
                Connection connection = new Connection(newConnection); // Initialize a new connection object
                ConnectionManager.Connections.Add(connection); // Add the connection to the list of active connections
                socket.BeginAccept(AcceptCallback, socket); // Accept new incomming socket connections again
            } catch (Exception ex) {
                Console.WriteLine("Error " + ex);
            }
        }

        /// <summary>
        /// Get the first ipv4 address of this machine
        /// </summary>
        private IPAddress GetIpAddress() {
            IPAddress ip = null; 
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName()); // Perform a dns lookup on this machine to get all the ip addresses
            // Find and use the first ipv4 address from the dns lookup
            for (int i = 0; i < ipHostInfo.AddressList.Length; i++) {
                if (ipHostInfo.AddressList[i].AddressFamily == AddressFamily.InterNetwork) {
                    ip = ipHostInfo.AddressList[i];
                }
            }
            if (ip == null) {
                Console.WriteLine("Server ip address not found");
            } else {
                Console.WriteLine("Server ip address found: " + ip);
                Console.WriteLine("Server port to listen on: " + port);
            }
            return ip;
        }
    }
}
