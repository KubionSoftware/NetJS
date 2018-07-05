using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using System.Web.WebSockets;
using System.Net.WebSockets;

namespace NetJS.Server {
    internal class WebHandler : IHttpAsyncHandler, IRequiresSessionState {

        public bool IsReusable => false;

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData) {
            var handler = new AsyncHandler(cb, context, extraData);
            handler.Start();
            return handler;
        }

        public void EndProcessRequest(IAsyncResult result) { }

        public void ProcessRequest(HttpContext context) {
            throw new NotImplementedException();
        }
    }

    public class AsyncHandler : IAsyncResult {

        private static JSServer _server;
        private static bool _initialized;

        private HttpContext _context;
        private AsyncCallback _callback;

        private bool _completed;
        private object _state;

        public bool IsCompleted => _completed;
        public WaitHandle AsyncWaitHandle => null;
        public object AsyncState => _state;
        public bool CompletedSynchronously => false;

        public AsyncHandler(AsyncCallback callback, HttpContext context, object state) {
            _callback = callback;
            _context = context;
            _state = state;
        }

        public void Start() {
            if (!_initialized) {
                _server = new JSServer();
                _initialized = true;
            }

            _context.Session["init"] = 0;

            if (_context.IsWebSocketRequest) {
                _context.AcceptWebSocketRequest(WebSocketRequestHandler);
                _callback(this);
                return;
            }

            var segments = _context.Request.Url.Segments;
            var lastSegment = segments.Length == 0 ? "" : segments[segments.Length - 1];
            if (lastSegment.ToLower() == "restart") {
                _server = new JSServer();
            }

            _server.Handle(_context, () => _callback(this));
        }

        public async Task WebSocketRequestHandler(AspNetWebSocketContext context) {
            var socket = context.WebSocket;

            var segments = context.RequestUri.Segments;
            var lastSegment = segments.Length == 0 ? "" : segments[segments.Length - 1];

            SocketHandler handler = new API.WebSocket(_server);

            await handler.Handle(context);
        }
    }
}
