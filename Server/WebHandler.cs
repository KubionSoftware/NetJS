using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using System.Web.WebSockets;

namespace NetJS.Server {
    internal class WebHandler : IHttpAsyncHandler, IReadOnlySessionState {

        public bool IsReusable => true;

        // Start of application, all requests come in via this method
        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData) {
            // Create a new handler object to handle the request asynchronously
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
            var called = false;
            Action after = () => {
                _completed = true;

                if (!called) {
                    // Try because request could already have been ended
                    try {
                        _callback(this);
                        called = true;
                    } catch { }
                }
            };

            // If there is no server, create a new one
            if (!_initialized) {
                _initialized = true;
                _server = new JSServer(_context, after);
            }

            // TODO: Handle this better
            if (_server == null) return;

            // If it is a websocket request, handle it asynchronously and complete immediately
            if (_context.IsWebSocketRequest) {
                _context.AcceptWebSocketRequest(WebSocketRequestHandler);
                after();
                return;
            }

            // If the url ends with /restart, force restart the server
            var segments = _context.Request.Url.Segments;
            var lastSegment = segments.Length == 0 ? "" : segments[segments.Length - 1];
            if (lastSegment.ToLower() == "restart") {
                _server = new JSServer(_context, after);
            }

            // Process a http request
            _server.ProcessRequest(_context, after);
        }

        // Handle a websocket request
        public async Task WebSocketRequestHandler(AspNetWebSocketContext context) {
            var socket = context.WebSocket;

            var segments = context.RequestUri.Segments;
            var lastSegment = segments.Length == 0 ? "" : segments[segments.Length - 1];

            SocketHandler handler = new API.WebSocket(_server);

            await handler.Handle(context);
        }
    }
}
