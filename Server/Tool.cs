using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NetJS.Server {
    public class Tool {
        public static string[] GetPath(HttpRequest request) {
            return request.Url.PathAndQuery.Split('?')[0].Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        }
        
        public static HttpContext GetContext() {
            return (State.Request as ServerRequest).Context;
        }

        public static void End(HttpContext context, string s) {
            try {
                if (context.Response.IsClientConnected) {
                    var buffer = Encoding.UTF8.GetBytes(s);
                    context.Response.BinaryWrite(buffer);
                    //context.Response.SuppressContent = true;
                    //context.ApplicationInstance.CompleteRequest();
                }
            } catch { }
        }
    }
}