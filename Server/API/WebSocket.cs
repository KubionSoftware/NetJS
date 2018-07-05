using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.WebSockets;

namespace NetJS.Server.API {
    public class WebSocket : SocketHandler {

        private static dynamic _onConnection;
        private static dynamic _onMessage;
        private static dynamic _onClose;
        private static dynamic _onError;

        private static ConcurrentDictionary<string, AspNetWebSocketContext> _sockets = new ConcurrentDictionary<string, AspNetWebSocketContext>();
        
        private JSApplication _application;
        private JSSession _session;

        private string _id;

        public WebSocket(JSServer server) {
            _application = server.Application;
            _session = new JSSession();
            _id = Guid.NewGuid().ToString();
        }

        /// <summary>Creates an event listener</summary>
        /// <param name="event">The name of the event (connection|message|close|error)</param>
        /// <param name="func">The function to call</param>
        /// <returns>undefined</returns>
        /// <example><code lang="javascript">WebSocket.on("connection", function(id){
        ///     Log.write(id);
        /// });</code></example>
        public static void on(string e, dynamic f) {
            if (e == "connection") {
                _onConnection = f;
            } else if (e == "message") {
                _onMessage = f;
            } else if (e == "close") {
                _onClose = f;
            } else if (e == "error") {
                _onError = f;
            }
        }

        /// <summary>Send a message to a websocket</summary>
        /// <param name="id">The id of the socket (string)</param>
        /// <param name="message">The message (string)</param>
        /// <returns>undefined</returns>
        /// <example><code lang="javascript">WebSocket.send(id, "{}");</code></example>
        public static void send(string id, string message) {
            if (_sockets.TryGetValue(id, out AspNetWebSocketContext context)) {
                try {
                    var responseBytes = Encoding.UTF8.GetBytes(message);
                    context.WebSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                } catch (Exception) {
                    State.Application.Error(new Error($"Can't send message to socket with id '{id}'"));
                }
            } else {
                State.Application.Error(new Error("Trying to send to a non-existing websocket id"));
            }
        }

        /// <summary>Closes a websocket connection</summary>
        /// <param name="id">The id of the socket (string)</param>
        /// <returns>undefined</returns>
        /// <example><code lang="javascript">WebSocket.close(id);</code></example>
        public static void close(string id) {
            if (_sockets.TryGetValue(id, out AspNetWebSocketContext context)) {
                context.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by user", CancellationToken.None);
                _sockets.TryRemove(id, out AspNetWebSocketContext removed);
            }
        }

        /// <summary>Checks if there is an open connection to a websocket</summary>
        /// <param name="id">The id of the socket (string)</param>
        /// <returns>If there is an open connection (boolean)</returns>
        /// <example><code lang="javascript">var open = WebSocket.isOpen(id);</code></example>
        public static bool isOpen(string id) {
            if (_sockets.TryGetValue(id, out AspNetWebSocketContext context)) {
                return context.WebSocket.State == WebSocketState.Open;
            }

            return false;
        }

        public override void OnConnection(AspNetWebSocketContext context) {
            _sockets.TryAdd(_id, context);

            if (_onConnection == null) return;
            
            Action<object> callback = result => { };
            var request = new NetJS.FunctionRequest(_onConnection, _application, callback, new JSSession(), _id);
            _application.AddRequest(request);
        }

        public override void OnClose(AspNetWebSocketContext context) {
            _sockets.TryRemove(_id, out AspNetWebSocketContext removed);

            if (_onClose == null) return;

            Action<object> callback = result => { };
            var request = new NetJS.FunctionRequest(_onClose, _application, callback, new JSSession(), _id);
            _application.AddRequest(request);
        }

        public override void OnMessage(AspNetWebSocketContext context, string message) {
            if (_onMessage == null) return;

            Action<object> callback = result => { };
            var request = new NetJS.FunctionRequest(_onMessage, _application, callback, new JSSession(), _id, message);
            _application.AddRequest(request);
        }

        public override void OnError(AspNetWebSocketContext context, Exception e) {
            // TODO: should websocket be removed on error?
            _sockets.TryRemove(_id, out AspNetWebSocketContext removed);

            if (_onError == null) return;

            Action<object> callback = result => { };
            var request = new NetJS.FunctionRequest(_onError, _application, callback, new JSSession(), _id, e.ToString());
            _application.AddRequest(request);
        }
    }
}