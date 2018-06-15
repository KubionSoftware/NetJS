using NetJS.Core;
using System.Web;

namespace NetJS.API {
    /// <summary>Sessions are implemented using ASP.NET. You can use the session to store and retrieve values.</summary>
    /// <remarks>This class can set, get and remove key-value pairs.
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
        public static Constant get(Constant _this, Constant[] arguments, Agent agent) {
            var key = Core.Tool.GetArgument<Core.String>(arguments, 0, "Session.get").Value;

            var session = (agent as NetJSAgent).Session;
            var value = session.Get(key);
            if(value == null) return Static.Undefined;
            return value;
        }

        /// <summary>Sessions.set takes a key and a value and sets the link in the session.</summary>
        /// <param name="key">The key to set a value with</param>
        /// <param name="value">The value to link to the key</param>
        /// <example><code lang="javascript">Session.set("userId", user.id);</code></example>
        public static Constant set(Constant _this, Constant[] arguments, Agent agent) {
            var key = Core.Tool.GetArgument<Core.String>(arguments, 0, "Session.set").Value;
            var value = Core.Tool.GetArgument(arguments, 1, "Session.set");

            var session = (agent as NetJSAgent).Session;
            session.Set(key, value);

            return Static.Undefined;
        }

        /// <summary>Sessions.remove takes a key and removes it from the session.</summary>
        /// <param name="key">The key to get a value from</param>
        /// <example><code lang="javascript">Sessions.remove("userId");</code></example>
        public static Constant remove(Constant _this, Constant[] arguments, Agent agent) {
            var key = Core.Tool.GetArgument<Core.String>(arguments, 0, "Session.delete").Value;

            var session = (agent as NetJSAgent).Session;
            session.Remove(key);

            return Static.Undefined;
        }

        /// <summary>Sessions.clear removes all values from the session.</summary>
        /// <example><code lang="javascript">Sessions.clear();</code></example>
        public static Constant clear(Constant _this, Constant[] arguments, Agent agent) {
            var session = (agent as NetJSAgent).Session;
            session.Clear();

            return Static.Undefined;
        }

        /// <summary>Sessions.getAll returns the entire session object.</summary>
        /// <returns>Object containing all keys and values.</returns>
        /// <example><code lang="javascript">var session = Sessions.getAll();</code></example>
        public static Constant getAll(Constant _this, Constant[] arguments, Agent agent) {
            var session = (agent as NetJSAgent).Session;
            return session.GetObject(agent);
        }
    }
}