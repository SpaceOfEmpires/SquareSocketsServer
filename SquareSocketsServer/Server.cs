using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SquareSocketsServer {
    internal class Server {
        private Socket socket; // The socket which will listen to incoming connections

        private IPAddress ip; // Ip address to listen on
        private readonly short port = 11000; // Port to listen on

        private List<Connection> Connections { get; set; }

        public Server() {
            ip = GetIpAddress();
            Connections = new List<Connection>();
        }

        public void Start() {
            try {
                socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(new IPEndPoint(ip, port));
                socket.Listen(10);
                socket.BeginAccept(AcceptCallback, socket);
            } catch (Exception ex) {
                Console.WriteLine("Error" + ex);
            }
        }

        private void AcceptCallback(IAsyncResult ar) {
            try {
                Socket newConnection = socket.EndAccept(ar);

                Connection connection = new Connection(newConnection, Connections);
                Connections.Add(connection);
                connection.StartReceiving();

                socket.BeginAccept(AcceptCallback, socket);
            } catch (Exception ex) {
                Console.WriteLine("Error " + ex);
            }
        }

        private IPAddress GetIpAddress() {
            IPAddress ip = null;
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            // Find and use the first ipv4 address of this pc
            for (int i = 0; i < ipHostInfo.AddressList.Length; i++) {
                if (ipHostInfo.AddressList[i].AddressFamily == AddressFamily.InterNetwork) {
                    ip = ipHostInfo.AddressList[i];
                }
            }
            if (ip == null) { Console.WriteLine("Server ip address not found"); }
            else {
                Console.WriteLine("Server ip address found: " + ip);
                Console.WriteLine("Server port to listen on: " + port);
            }

            return ip;
        }
    }
}
