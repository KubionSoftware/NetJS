using NetJS.Core.Javascript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NetJS.Server.API {
    /// <summary>Provides methods to read, write or remove headers</summary>
    class Headers {

        /// <summary>Reads a header.</summary>
        /// <param name="name">The name of the header (string)</param>
        /// <returns>The header value (string)</returns>
        /// <example><code lang="javascript">var acceptedTypes = Headers.get("Accept");</code></example>
        /// <exception cref="Error">Thrown if there is no HTTP context</exception>
        public static Constant get(Constant _this, Constant[] arguments, Scope scope) {
            var key = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "Headers.get").Value;

            var context = Tool.GetContext(scope, "Headers.get");
            var header = context.Request.Headers.Get(key);
            return new Core.Javascript.String(header);
        }

        /// <summary>Writes a header.</summary>
        /// <param name="name">The name of the header (string)</param>
        /// <param name="value">The value to set (string)</param>
        /// <example><code lang="javascript">Headers.set("Content-Type", "application/json");</code></example>
        /// <exception cref="Error">Thrown if there is no HTTP context</exception>
        public static Constant set(Constant _this, Constant[] arguments, Scope scope) {
            var key = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "Headers.set").Value;
            var value = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 1, "Headers.set").Value;

            var context = Tool.GetContext(scope, "Headers.get");
            context.Response.AppendHeader(key, value);
            return Static.Undefined;
        }

        /// <summary>Removes a header.</summary>
        /// <param name="name">The name of the header (string)</param>
        /// <example><code lang="javascript">Headers.remove("ApplicationID")</code></example>
        /// <exception cref="Error">Thrown if there is no HTTP context</exception>
        public static Constant remove(Constant _this, Constant[] arguments, Scope scope) {
            var key = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "Headers.remove").Value;

            var context = Tool.GetContext(scope, "Headers.get");
            context.Response.Headers.Remove(key);
            return Static.Undefined;
        }
    }
}
