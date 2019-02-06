using System;
using System.Threading;

namespace SquareSocketsServer {
    class Program {
        private static ManualResetEvent endMainThread = new ManualResetEvent(false);

        static void Main(string[] args) {
            Console.WriteLine("Server");
            Server server = new Server();
            server.Start();
            endMainThread.WaitOne();
        }
    }
}
