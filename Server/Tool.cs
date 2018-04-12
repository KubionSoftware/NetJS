using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetJS.Server {
    public class Tool {
        public static string[] GetPath(HttpRequest request) {
            return request.Url.PathAndQuery.Split('?')[0].Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static NetJS.Core.Javascript.Object CreateRequest(HttpContext context, string[] path, NetJS.Core.Javascript.Scope scope) {
            var request = NetJS.Core.Tool.Construct("Object", scope);
            request.Set("path", NetJS.Core.Tool.ToArray(path, scope));

            var form = NetJS.Core.Tool.Construct("Object", scope);
            for (var i = 0; i < context.Request.Form.Count; i++) {
                var key = context.Request.Form.GetKey(i);
                var value = context.Request.Form.Get(i);
                form.Set(key, new NetJS.Core.Javascript.String(value));
            }
            request.Set("form", form);

            var parameters = NetJS.Core.Tool.Construct("Object", scope);
            var query = HttpUtility.ParseQueryString(context.Request.Url.Query);
            for (var i = 0; i < query.Count; i++) {
                var key = query.GetKey(i);
                if (key == null) continue;

                var value = query.Get(i);
                parameters.Set(key, new NetJS.Core.Javascript.String(value));
            }
            request.Set("params", parameters);

            var content = new System.IO.StreamReader(context.Request.InputStream, context.Request.ContentEncoding).ReadToEnd();
            request.Set("content", new NetJS.Core.Javascript.String(content));

            request.Set("method", new NetJS.Core.Javascript.String(context.Request.HttpMethod));
            request.Set("url", new NetJS.Core.Javascript.String(context.Request.RawUrl));
            request.Set("scheme", new NetJS.Core.Javascript.String(context.Request.Url.Scheme));

            return request;
        }
    }
}