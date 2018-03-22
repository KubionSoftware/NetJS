using NetJS.Core.Javascript;
using System.Web;

namespace NetJS.External {
    /// <summary>Sessions are implemented using ASP.NET. You can use the session to store and retrieve values.</summary>
    /// <remarks>This class can set, get and delete key-value pairs.
    /// Unlike SessionStorage in browsers, this session allows you to store all types of variables instread of only strings.</remarks>
    /// <example name="Functions implementation">Here you can see the functions of this class in action:
    /// <code lang="javascript">Sessions.set("key", "value");
    /// console.log(Sessions.get("key"); //prints: value
    /// Sessions.delete("key");</code></example>
    public class Session {

        
        /// <summary>Sessions.get takes a key, gets the value linked in the session and returns the value.</summary>
        /// <param name="key">The key to get a value from</param>
        /// <returns>Value linked to key.</returns>
        /// <example><code lang="javascript">var value = Sessions.get("userID");</code></example>
        /// <exception cref="InternalError">Thrown when no application has been found in application scope.</exception>
        public static Constant get(Constant _this, Constant[] arguments, Scope scope) {
            var key = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "Session.get").Value;

            var session = Tool.GetFromScope<JSSession>(scope, "__session__");
            if (session == null) throw new InternalError("No session");
            var value = session == null ? null : session.Get(key);

            if(value == null) return Static.Undefined;
            return value;
        }

        /// <summary>Sessions.set takes a key and a value and sets the link in the session.</summary>
        /// <param name="key">The key to set a value with</param>
        /// <param name="value">The value to link to the key</param>
        /// <example><code lang="javascript">Session.set("userId", user.id);</code></example>
        /// <exception cref="InternalError">Thrown when no application has been found in application scope.</exception>
        public static Constant set(Constant _this, Constant[] arguments, Scope scope) {
            var key = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "Session.set").Value;
            var value = Core.Tool.GetArgument(arguments, 1, "Session.set");

            var session = Tool.GetFromScope<JSSession>(scope, "__session__");
            if (session == null) throw new InternalError("No session");
            if (session != null) session.Set(key, value);

            return Static.Undefined;
        }

        /// <summary>Sessions.delete takes a key and removes it from the session.</summary>
        /// <param name="key">The key to get a value from</param>
        /// <example><code lang="javascript">Sessions.delete("userId");</code></example>
        /// <exception cref="InternalError">Thrown when no application has been found in application scope.</exception>
        public static Constant delete(Constant _this, Constant[] arguments, Scope scope) {
            var key = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "Session.delete").Value;

            var session = Tool.GetFromScope<JSSession>(scope, "__session__");
            if (session == null) throw new InternalError("No session");
            if (session != null) session.Remove(key);

            return Static.Undefined;
        }
    }
}