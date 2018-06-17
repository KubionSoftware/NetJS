using NetJS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Web;
using System.Web.WebSockets;

namespace NetJS.Server {
    public class DebugSocketHandler : SocketHandler {

        public override void OnConnection(AspNetWebSocketContext context) {
            Debug.AddSocket(context.WebSocket);
        }

        public override void OnClose(AspNetWebSocketContext context) {
            Debug.RemoveSocket(context.WebSocket);
        }

        public override void OnMessage(AspNetWebSocketContext context, string message) {
            Debug.HandleMessage(message);
        }

        public override void OnError(AspNetWebSocketContext context, Exception e) {

        }
    }
}