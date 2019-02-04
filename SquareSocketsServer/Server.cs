using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SquareSocketsServer {
    internal class Server {
        public Socket listenerSocket; // The socket which will listen to incoming connections
        public short port = 11000; // Port to listen on

        public void Start() {
            try {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = null;
                // Find and use the first ipv4 address of this pc
                for (int i = 0; i < ipHostInfo.AddressList.Length; i++) {
                    if (ipHostInfo.AddressList[i].AddressFamily == AddressFamily.InterNetwork) {
                        ipAddress = ipHostInfo.AddressList[i];
                    }
                }
                // Stop in case no ip have been found
                if (ipAddress == null) {
                    Debug.WriteLine("Server ip address not found");
                    return;
                } else {
                    Debug.WriteLine($"Server listening on: {ipAddress}:{port}");
                }

                listenerSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listenerSocket.Bind(new IPEndPoint(ipAddress, port));
                listenerSocket.Listen(100);
                listenerSocket.BeginAccept(AcceptCallback, listenerSocket);
            } catch (Exception ex) {
                throw new Exception("Server error" + ex);
            }
        }

        public void AcceptCallback(IAsyncResult ar) {
            try {
                DataController.ToConsole($"Accept CallBack port:{port} protocol type: {ProtocolType.Tcp}");
                Socket acceptedSocket = listenerSocket.EndAccept(ar);
                ClientController.AddClient(acceptedSocket);

                listenerSocket.BeginAccept(AcceptCallback, listenerSocket);
            } catch (Exception ex) {
                throw new Exception("Base Accept error" + ex);
            }
        }













        /*const int port = 11000; // Port to listen on

        public ManualResetEvent allDone = new ManualResetEvent(false); // Thread signal. Docs "Notifies one or more waiting threads that an event has occurred"

        private DataManager dataManager = new DataManager();

        public void StartServer() {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = null;

            // Find and use the first ipv4 address
            for (int i = 0; i < ipHostInfo.AddressList.Length; i++) {
                if (ipHostInfo.AddressList[i].AddressFamily == AddressFamily.InterNetwork) {
                    ipAddress = ipHostInfo.AddressList[i];
                }
            }

            // Stop in case no ip is set
            if (ipAddress == null) {
                Debug.WriteLine("ipAddress null");
                return;
            } else {
                Debug.WriteLine("Listening on ip: " + ipAddress);
            }

            IPEndPoint localEP = new IPEndPoint(ipAddress, port);

            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp); // Create a TCP/IP socket

            // Bind the socket to the local endpoint and listen for incoming connections
            try {
                listener.Bind(localEP);
                listener.Listen(100);

                while (true) {
                    allDone.Reset(); // Set the event to nonsignaled state
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener); // Start an asynchronous socket to listen for connections
                    allDone.WaitOne(); // Wait until a connection is made before continuing
                }

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        private void AcceptCallback(IAsyncResult ar) {
            allDone.Set(); // Signal the main thread to continue

            // Get the socket that handles the client request
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        private void ReadCallback(IAsyncResult ar) {
            String content = String.Empty;

            // Retrieve the state object and the handler socket from the asynchronous state object
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0) {
                // There  might be more data, so store the data received so far
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read
                // more data
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1) {
                    //Send(handler, content); // Echo the data back to the client
                    content = content.Remove(content.IndexOf("<EOF>")); // remove the end of file tag
                    dataManager.ToConsole(content);
                } else {
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state); // Not all data received. Get more
                }
            }
        }

        private void Send(Socket handler, String data) {
            byte[] byteData = Encoding.ASCII.GetBytes(data); // Convert the string data to byte data using ASCII encoding
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler); // Begin sending the data to the remote device
        }

        private void SendCallback(IAsyncResult ar) {
            try {
                Socket handler = (Socket)ar.AsyncState; // Retrieve the socket from the state object

                // Complete sending the data to the remote device
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }*/
    }
}
