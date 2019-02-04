using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SquareSocketsServer {
    static class ClientController {
        public static List<Client> Clients = new List<Client>();
        private static int count = 0;

        public static void AddClient(Socket socket) {
            Clients.Add(new Client(socket, count));
        }

        public static void RemoveClient(int id) {
            Clients.RemoveAt(Clients.FindIndex(x => x._id == id));
        }
    }
}
