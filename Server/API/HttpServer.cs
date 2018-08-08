using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NetJS.Server.API {
    public class HTTPServer {

        public static Action<object> Callback(Action after, HttpContext context) {
            return result => {
                Tool.End(context, result.ToString());
                after();
            };
        }

        private static dynamic _onConnection;

        /// <summary>Creates an event listener</summary>
        /// <param name="event">The name of the event (connection)</param>
        /// <param name="func">The function to call</param>
        /// <returns>undefined</returns>
        /// <example><code lang="javascript">HttpServer.on("connection", function(){
        ///     Log.write(id);
        /// });</code></example>
        public static void on(string e, dynamic f) {
            if (e == "connection") {
                _onConnection = f;
            }
        }

        public static void OnConnection(JSApplication application, JSSession session, Action after) {
            if (_onConnection == null) return;

            Action<object> callback = Callback(after, HttpContext.Current);
            var request = new ServerRequest(_onConnection, application, callback, session, HttpContext.Current);
            application.AddRequest(request);
        }
    }
}