using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NetJS.Server.API {
    public class HTTPServer {

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

            Action<object> callback = result => {
                var responseString = result.ToString();
                var buffer = Encoding.UTF8.GetBytes(responseString);
                var output = Tool.GetContext().Response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
                NetJS.API.Log.write("Http request took " + State.Request.ElapsedMilliseconds() + " ms");
                after();
            };
            var request = new ServerRequest(_onConnection, application, callback, session, HttpContext.Current);
            application.AddRequest(request);
        }
    }
}