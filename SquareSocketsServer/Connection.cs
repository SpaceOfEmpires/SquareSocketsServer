using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace SquareSocketsServer {
    internal class Connection {
        private Socket Socket { get; set; } // The socket this connection belongs to

        private byte[] buffer; // Byte array used in figuring out when the incomming package ends

        public Connection(Socket socket) {
            Console.WriteLine("New connection");
            Socket = socket;
            Receive();
        }

        /// <summary>
        /// Begin listening for incomming packages
        /// </summary>
        private void Receive() {
            try {
                buffer = new byte[4]; // Set the buffer to a size of 4 (int32)
                Socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null); // Listen for a package. Receive 4 bytes (buffer). Then run the AsyncCallback to handle the incomming package
            } catch {

            }
        }

        /// <summary>
        /// AsyncCallback that is run when a package has arrived
        /// </summary>
        private void ReceiveCallback(IAsyncResult AR) {
            try {
                if (Socket.EndReceive(AR) > 1) {
                    buffer = new byte[BitConverter.ToInt32(buffer, 0)]; // Get the 4 first bytes from the incomming package and turn the value into a new byte array. The new byte array is the length of the rest of the incomming package
                    Socket.Receive(buffer, buffer.Length, SocketFlags.None); // Receive the last of the package

                    string data = Encoding.UTF8.GetString(buffer); // Turn the package of bytes into a string using the encoding UTF8



                    Console.WriteLine("Received data: " + data); // The server should not write to the console but rather a window? or a file perhaps
                    ConnectionManager.SyncMsg(this, data); // Currently any incomming message is send to any client connected to the server



                    Receive(); // Start listening for new incomming packages
                } else {
                    Disconnect(); // If we didnt receive more than one byte then the client disconnected
                }
            } catch {
                if (!Socket.Connected) {
                    Disconnect(); // Disconnect this socket if the client disconnected
                } else {
                    Receive(); // If the client didnt disconnect then try listening for a new package
                }
            }
        }

        /// <summary>
        /// Disconnect the socket and remove it from the list of active connections
        /// </summary>
        private void Disconnect() {
            Socket.Shutdown(SocketShutdown.Both);
            Socket.Close();
            ConnectionManager.Connections.Remove(this);
            Console.WriteLine("Client disconnected");
        }

        /// <summary>
        /// Take a string and encode it to finally send it over the socket
        /// </summary>
        public void Send(string data) {
            try {
                List<byte> fullPacket = new List<byte>(); // Using a list to easier add onto the array
                byte[] package = Encoding.UTF8.GetBytes(data); // Get the string as a byte array
                fullPacket.AddRange(BitConverter.GetBytes(package.Length)); // Get the length of the package in bytes and add it to the list
                fullPacket.AddRange(package); // Now add the actual package to the list
                Socket.Send(fullPacket.ToArray()); // Send the list to the socket as an array
            } catch (Exception ex) {
                Console.WriteLine("Error " + ex);
            }
        }
    }
}