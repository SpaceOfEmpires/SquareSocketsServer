using System;
using System.Collections.Generic;

namespace SquareSocketsServer {
    internal static class ConnectionManager {
        public static List<Connection> Connections { get; set; } // List of active connections

        static ConnectionManager() {
            Connections = new List<Connection>();
        }

        // Send a message to every connection but the connection the message came from
        public static void SyncMsg(Connection messenger, string message) {
            foreach(Connection connection in Connections) {
                if (connection != messenger) {
                    connection.Send(message);
                }
            }
        }
    }
}
