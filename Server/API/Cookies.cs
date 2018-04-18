using NetJS.Core.Javascript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NetJS.Server.API {
    /// <summary>Provides methods to read, write or remove cookies</summary>
    class Cookies {

        /// <summary>Reads a cookie.</summary>
        /// <param name="name">The name of the cookie (string)</param>
        /// <returns>The cookie value (string)</returns>
        /// <example><code lang="javascript">var ssid = Cookies.get("SSID");</code></example>
        /// <exception cref="Error">Thrown if there is no HTTP context</exception>
        public static Constant get(Constant _this, Constant[] arguments, Scope scope) {
            var key = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "Cookies.get").Value;

            var cookie = HttpContext.Current.Request.Cookies.Get(key);
            return new Core.Javascript.String(cookie.Value);
        }

        /// <summary>Writes a cookie.</summary>
        /// <param name="name">The name of the cookie (string)</param>
        /// <param name="value">The value to set (string)</param>
        /// <example><code lang="javascript">Headers.set("SSID", "AE3oaD8COGojttJue");</code></example>
        /// <exception cref="Error">Thrown if there is no HTTP context</exception>
        public static Constant set(Constant _this, Constant[] arguments, Scope scope) {
            var key = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "Cookies.set").Value;
            var value = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 1, "Cookies.set").Value;
            var expires = Core.Tool.GetArgument<Core.Javascript.Date>(arguments, 2, "Cookies.set", false);

            var cookie = new HttpCookie(key, value);
            if (expires != null) cookie.Expires = expires.Value;
            HttpContext.Current.Response.Cookies.Add(cookie);
            return Static.Undefined;
        }

        /// <summary>Removes a cookie.</summary>
        /// <param name="name">The name of the cookie (string)</param>
        /// <example><code lang="javascript">Headers.remove("SSID")</code></example>
        /// <exception cref="Error">Thrown if there is no HTTP context</exception>
        public static Constant remove(Constant _this, Constant[] arguments, Scope scope) {
            var key = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "Cookies.remove").Value;

            var cookie = new HttpCookie(key);
            cookie.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Add(cookie);
            return Static.Undefined;
        }
    }
}
