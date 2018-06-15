using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetJS.Server {
    public class Tool {
        public static string[] GetPath(HttpRequest request) {
            return request.Url.PathAndQuery.Split('?')[0].Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        }
        
        public static HttpContext GetContext(Core.Agent agent, string method) {
            var context = HttpContext.Current;
            if (context == null) {
                throw new Core.Error($"Can't use method '{method}' because NetJS is not being run as a server");
            }
            return context;
        }
    }
}