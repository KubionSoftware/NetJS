using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace NetJS {
    class Proxy {

        private IPEndPoint _listenEndPoint;
        private Func<int> _onConnection;

        public Proxy(int listenPort, Func<int> onConnection) {
            _listenEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), listenPort);
            _onConnection = onConnection;
        }

        public void Start() {
            var thread = new Thread(Loop);
            thread.Start();
        }

        private void Loop() {
            var listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            listenerSocket.Bind(_listenEndPoint);
            listenerSocket.Listen(10);

            while (true) {
                var source = listenerSocket.Accept();
                var destination = new ProxyForwarder();
                var state = new ProxyState(source, destination.Socket);
                var port = _onConnection();
                var remote = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
                destination.Connect(remote, source);
                source.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, ProxyForwarder.OnDataReceive, state);
            }
        }

        public static bool IsPortAvailable(int port) {
            // See: https://stackoverflow.com/a/570461/4304127

            bool isAvailable = true;

            // Evaluate current system tcp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our TcpClient is occupied, we will set isAvailable to false.
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray) {
                if (tcpi.LocalEndPoint.Port == port) {
                    isAvailable = false;
                    break;
                }
            }

            return isAvailable;
        }
    }

    class ProxyState {
        public Socket SourceSocket { get; private set; }
        public Socket DestinationSocket { get; private set; }
        public byte[] Buffer { get; private set; }

        public ProxyState(Socket source, Socket destination) {
            SourceSocket = source;
            DestinationSocket = destination;
            Buffer = new byte[8192];
        }
    }

    class ProxyForwarder {
        private readonly Socket _mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public Socket Socket { get { return _mainSocket; } }

        public void Connect(EndPoint remoteEndpoint, Socket destination) {
            var state = new ProxyState(_mainSocket, destination);
            _mainSocket.Connect(remoteEndpoint);
            _mainSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, OnDataReceive, state);
        }

        public static void OnDataReceive(IAsyncResult result) {
            var state = (ProxyState)result.AsyncState;
            try {
                var bytesRead = state.SourceSocket.EndReceive(result);
                if (bytesRead > 0) {
                    state.DestinationSocket.Send(state.Buffer, bytesRead, SocketFlags.None);
                    state.SourceSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, OnDataReceive, state);
                }
            } catch {
                state.DestinationSocket.Close();
                state.SourceSocket.Close();
            }
        }
    }
}
