using NetJS.Javascript;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using System.Web.WebSockets;

namespace NetJS {
    internal class WebHandler : IHttpHandler, IRequiresSessionState {

        public bool IsReusable => false;

        private static JSService _service;
        private static bool _initialized;

        public void ProcessRequest(HttpContext context) {
            if (!_initialized) {
                _service = new JSService();
                _initialized = true;
            }

            context.Session["init"] = 0;

            if (context.IsWebSocketRequest) {
                context.AcceptWebSocketRequest(WebSocketRequestHandler);
                return;
            }
            
            _service.Handle(context);
        }

        public async Task WebSocketRequestHandler(AspNetWebSocketContext context) {
            var socket = context.WebSocket;
            Debug.AddSocket(socket);

            const int maxMessageSize = 1024;

            if (socket.State == System.Net.WebSockets.WebSocketState.Open) {
                var receiveBuffer = new ArraySegment<Byte>(new Byte[maxMessageSize]);
                var cancellationToken = new CancellationToken();

                while (true) {
                    var content = await socket.ReceiveAsync(receiveBuffer, cancellationToken);
                    if (content.MessageType == System.Net.WebSockets.WebSocketMessageType.Close) {
                        Debug.RemoveSocket(socket);
                        break;
                    } else {
                        var text = Encoding.UTF8.GetString(receiveBuffer.Array, 0, content.Count);
                        Debug.HandleMessage(text);
                    }
                }
            }
        }
    }
}
