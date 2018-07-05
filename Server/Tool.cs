using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetJS.Server {
    public class Tool {
        public static string[] GetPath(HttpRequest request) {
            return request.Url.PathAndQuery.Split('?')[0].Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        }
        
        public static HttpContext GetContext() {
            return (State.Request as ServerRequest).Context;
        }
    }
}