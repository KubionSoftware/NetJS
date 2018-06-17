using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebSockets;

namespace NetJS.Server {
    public abstract class SocketHandler {

        private const int BufferSize = 1024;

        public async Task Handle(AspNetWebSocketContext context) {
            var socket = context.WebSocket;

            if (socket.State == WebSocketState.Open) {
                OnConnection(context);

                var receiveBuffer = new ArraySegment<Byte>(new Byte[BufferSize]);
                var cancellationToken = new CancellationToken();

                while (true) {
                    try {
                        var content = await socket.ReceiveAsync(receiveBuffer, cancellationToken);

                        if (content.MessageType == WebSocketMessageType.Close) {
                            OnClose(context);
                            break;
                        } else {
                            var text = Encoding.UTF8.GetString(receiveBuffer.Array, 0, content.Count);
                            OnMessage(context, text);
                        }
                    } catch(Exception e) {
                        OnError(context, e);
                    }
                }
            }
        }

        public abstract void OnConnection(AspNetWebSocketContext context);
        public abstract void OnClose(AspNetWebSocketContext context);
        public abstract void OnMessage(AspNetWebSocketContext context, string message);
        public abstract void OnError(AspNetWebSocketContext context, Exception e);
    }
}