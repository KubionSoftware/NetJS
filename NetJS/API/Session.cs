using Microsoft.ClearScript;
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
        public static object get(string key) {
            var value = State.Session.Get(key);
            return value;
        }

        /// <summary>Sessions.set takes a key and a value and sets the link in the session.</summary>
        /// <param name="key">The key to set a value with</param>
        /// <param name="value">The value to link to the key</param>
        /// <example><code lang="javascript">Session.set("userId", user.id);</code></example>
        public static bool set(string key, object value) {
            if (!(value is string || value is int || value is double || value is bool)) {
                State.Application.Error(new Error("Can only store string, number or boolean in Session"), ErrorStage.Runtime);
                return false;
            }

            State.Session.Set(key, value);
            return true;
        }

        /// <summary>Session.contains checks if the session contains a key.</summary>
        /// <param name="key">The key to check</param>
        /// <example><code lang="javascript">if (Session.contains("userId")) { ... }</code></example>
        public static bool contains(string key) {
            return State.Session.Contains(key);
        }

        /// <summary>Sessions.remove takes a key and removes it from the session.</summary>
        /// <param name="key">The key to get a value from</param>
        /// <example><code lang="javascript">Sessions.remove("userId");</code></example>
        public static bool remove(string key) {
            State.Session.Remove(key);
            return true;
        }

        /// <summary>Sessions.clear removes all values from the session.</summary>
        /// <example><code lang="javascript">Sessions.clear();</code></example>
        public static bool clear() {
            State.Session.Clear();
            return true;
        }

        /// <summary>Sessions.getAll returns the entire session object.</summary>
        /// <returns>Object containing all keys and values.</returns>
        /// <example><code lang="javascript">var session = Sessions.getAll();</code></example>
        public static dynamic getAll() {
            return State.Session.GetObject();
        }
    }
}