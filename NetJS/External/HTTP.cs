using NetJS.Javascript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetJS.External {
    class HTTP {

        public static Constant get(Constant _this, Constant[] arguments, Scope scope) {
            var url = ((Javascript.String)arguments[0]).Value;
            return new Javascript.String(Util.HTTP.Get(url));
        }

        public static Constant post(Constant _this, Constant[] arguments, Scope scope) {
            var url = ((Javascript.String)arguments[0]).Value;
            var content = ((Javascript.String)arguments[1]).Value;
            return new Javascript.String(Util.HTTP.Post(url, content));
        }

        public static Constant execute(Constant _this, Constant[] arguments, Scope scope) {
            var connectionName = ((Javascript.String)arguments[0]).Value;
            var url = scope.Application.Connections.GetHttpUrl(connectionName);

            var query = ((Javascript.String)arguments[1]).Value;

            var result = new Javascript.String(Util.HTTP.Get(url + query));
            try {
                var json = Internal.JSON.parse(_this, new[] { result }, scope);
                return json;
            } catch {
                return result;
            }
        }
    }
}