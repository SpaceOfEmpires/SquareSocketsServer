using System;
using System.Net.Sockets;
using System.Text;

namespace SquareSocketsServer {
    internal class ReceivePacket {
        private byte[] _buffer;
        private Client _client;

        public ReceivePacket(Client client) {
            _client = client;
        }

        public void StartReceiving() {
            try {
                _buffer = new byte[4];
                _client._socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, null);
            } catch { }
        }

        private void ReceiveCallback(IAsyncResult AR) {
            try {
                // if bytes are less than 1 takes place when a client disconnect from the server.
                // So we run the Disconnect function on the current client
                if (_client._socket.EndReceive(AR) > 1) {
                    // Convert the first 4 bytes (int 32) that we received and convert it to an Int32 (this is the size for the coming data).
                    _buffer = new byte[BitConverter.ToInt32(_buffer, 0)];
                    // Next receive this data into the buffer with size that we did receive before
                    _client._socket.Receive(_buffer, _buffer.Length, SocketFlags.None);
                    // When we received everything its onto you to convert it into the data that you've send.
                    // For example string, int etc... in this example I only use the implementation for sending and receiving a string.

                    // Convert the bytes to string and output it in a message box
                    string data = Encoding.Default.GetString(_buffer);
                    Console.WriteLine(data);
                    // Now we have to start all over again with waiting for a data to come from the socket.
                    StartReceiving();
                } else {
                    Disconnect();
                }
            } catch {
                // if exeption is throw check if socket is connected because than you can startreive again else Dissconect
                if (!_client._socket.Connected) {
                    Disconnect();
                } else {
                    StartReceiving();
                }
            }
        }

        private void Disconnect() {
            _client._socket.Disconnect(true);
            ClientController.RemoveClient(_client._id);
        }
    }
}
