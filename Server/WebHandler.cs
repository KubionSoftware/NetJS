using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using System.Web.WebSockets;
using NetJS.Core;
using System.Net.WebSockets;

namespace NetJS.Server {
    internal class WebHandler : IHttpHandler, IRequiresSessionState {

        public bool IsReusable => false;

        private static JSServer _server;
        private static bool _initialized;

        public void ProcessRequest(HttpContext context) {
            if (!_initialized) {
                _server = new JSServer();
                _initialized = true;
            }

            context.Session["init"] = 0;

            if (context.IsWebSocketRequest) {
                context.AcceptWebSocketRequest(WebSocketRequestHandler);
                return;
            }

            var segments = context.Request.Url.Segments;
            var lastSegment = segments.Length == 0 ? "" : segments[segments.Length - 1];
            if (lastSegment.ToLower() == "restart") {
                _server = new JSServer();
            }

            _server.Handle(context);
        }

        public async Task WebSocketRequestHandler(AspNetWebSocketContext context) {
            var socket = context.WebSocket;

            var segments = context.RequestUri.Segments;
            var lastSegment = segments.Length == 0 ? "" : segments[segments.Length - 1];

            SocketHandler handler;

            if (lastSegment.ToLower() == "debug") {
                handler = new DebugSocketHandler();
            } else {
                handler = new API.WebSocket(_server);
            }

            await handler.Handle(context);
        }
    }
}
