using System;
using System.Net.Sockets;

namespace SquareSocketsServer {
    internal class Client {
        public Socket _socket { get; set; }
        public int _id { get; set; }

        public ReceivePacket Receive { get; set; }

        public Client(Socket socket, int id) {
            _socket = socket;
            _id = id;

            Receive = new ReceivePacket(this);
            Receive.StartReceiving();
        }
    }
}
