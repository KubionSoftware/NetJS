using NetJS.Core;
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

        private static Function _onConnection;
        private static Function _onMessage;
        private static Function _onClose;
        private static Function _onError;

        private static ConcurrentDictionary<string, AspNetWebSocketContext> _sockets = new ConcurrentDictionary<string, AspNetWebSocketContext>();
        
        private JSApplication _application;
        private JSSession _session;

        private string _id;

        public WebSocket(JSServer server) {
            _application = server.Application;
            _session = new JSSession();
            _id = Guid.NewGuid().ToString();
        }

        private NetJSAgent CreateAgent() {
            var agent = new NetJSAgent(_application.Realm, _application, _session);
            return agent;
        }

        /// <summary>Creates an event listener</summary>
        /// <param name="event">The name of the event (connection|message|close|error)</param>
        /// <param name="func">The function to call</param>
        /// <returns>undefined</returns>
        /// <example><code lang="javascript">WebSocket.on("connection", function(id){
        ///     Log.write(id);
        /// });</code></example>
        public static Constant on(Constant _this, Constant[] arguments, Agent agent) {
            var e = Core.Tool.GetArgument<Core.String>(arguments, 0, "WebSocket.on").Value;
            var f = Core.Tool.GetArgument<Core.Function>(arguments, 1, "WebSocket.on");

            if (e == "connection") {
                _onConnection = f;
            } else if (e == "message") {
                _onMessage = f;
            } else if (e == "close") {
                _onClose = f;
            } else if (e == "error") {
                _onError = f;
            }

            return Static.Undefined;
        }

        /// <summary>Send a message to a websocket</summary>
        /// <param name="id">The id of the socket (string)</param>
        /// <param name="message">The message (string)</param>
        /// <returns>undefined</returns>
        /// <example><code lang="javascript">WebSocket.send(id, "{}");</code></example>
        public static Constant send(Constant _this, Constant[] arguments, Agent agent) {
            var id = Core.Tool.GetArgument<Core.String>(arguments, 0, "WebSocket.send").Value;
            var message = Core.Tool.GetArgument<Core.String>(arguments, 1, "WebSocket.send").Value;

            if (_sockets.TryGetValue(id, out AspNetWebSocketContext context)) {
                try {
                    var responseBytes = Encoding.UTF8.GetBytes(message);
                    context.WebSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                } catch (Exception) {
                    throw new Error($"Can't send message to socket with id '{id}'");
                }
            } else {
                throw new Error("Trying to send to a non-existing websocket id");
            }

            return Static.Undefined;
        }

        /// <summary>Closes a websocket connection</summary>
        /// <param name="id">The id of the socket (string)</param>
        /// <returns>undefined</returns>
        /// <example><code lang="javascript">WebSocket.close(id);</code></example>
        public static Constant close(Constant _this, Constant[] arguments, Agent agent) {
            var id = Core.Tool.GetArgument<Core.String>(arguments, 0, "WebSocket.send").Value;

            if (_sockets.TryGetValue(id, out AspNetWebSocketContext context)) {
                context.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by user", CancellationToken.None);
                _sockets.TryRemove(id, out AspNetWebSocketContext removed);
            }

            return Static.Undefined;
        }

        /// <summary>Checks if there is an open connection to a websocket</summary>
        /// <param name="id">The id of the socket (string)</param>
        /// <returns>If there is an open connection (boolean)</returns>
        /// <example><code lang="javascript">var open = WebSocket.isOpen(id);</code></example>
        public static Constant isOpen(Constant _this, Constant[] arguments, Agent agent) {
            var id = Core.Tool.GetArgument<Core.String>(arguments, 0, "WebSocket.send").Value;

            if (_sockets.TryGetValue(id, out AspNetWebSocketContext context)) {
                return Core.Boolean.Create(context.WebSocket.State == WebSocketState.Open);
            }

            return Core.Boolean.False;
        }

        public override void OnConnection(AspNetWebSocketContext context) {
            _sockets.TryAdd(_id, context);

            if (_onConnection == null) return;

            // TODO: error handling
            try {
                _onConnection.Call(Static.Undefined, CreateAgent(), new Constant[] {
                    new Core.String(_id)
                });
            } catch (Exception e) {

            }
        }

        public override void OnClose(AspNetWebSocketContext context) {
            _sockets.TryRemove(_id, out AspNetWebSocketContext removed);

            if (_onClose == null) return;

            // TODO: error handling
            try {
                _onClose.Call(Static.Undefined, CreateAgent(), new Constant[] {
                    new Core.String(_id)
                });
            } catch { }
        }

        public override void OnMessage(AspNetWebSocketContext context, string message) {
            if (_onMessage == null) return;

            // TODO: error handling
            try {
                _onMessage.Call(Static.Undefined, CreateAgent(), new Constant[] {
                    new Core.String(_id),
                    new Core.String(message)
                });
            } catch { }
        }

        public override void OnError(AspNetWebSocketContext context, Exception e) {
            // TODO: should websocket be removed on error?
            _sockets.TryRemove(_id, out AspNetWebSocketContext removed);

            if (_onError == null) return;

            // TODO: error handling
            try {
                _onError.Call(Static.Undefined, CreateAgent(), new Constant[] {
                    new Core.String(_id)
                });
            } catch { }
        }
    }
}