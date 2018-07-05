using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.API {
    /// <summary>Application can be used to store and retrieve global values</summary>
    /// <remarks>This class can set, get and remove key-value pairs.
    public class Application {

        /// <summary>Application.get takes a key and returns the value.</summary>
        /// <param name="key">The key to get a value from</param>
        /// <returns>Value linked to key.</returns>
        /// <example><code lang="javascript">var value = Application.get("userID");</code></example>
        public static object get(string key) {
            var value = State.Application.Get(key);
            return value;
        }

        /// <summary>Application.set stores a value under a key.</summary>
        /// <param name="key">The key to set a value with</param>
        /// <param name="value">The value to link to the key</param>
        /// <example><code lang="javascript">Application.set("userId", user.id);</code></example>
        public static bool set(string key, object value) {
            State.Application.Set(key, value);
            return true;
        }

        /// <summary>Application.remove takes a key and removes it.</summary>
        /// <param name="key">The key to get a value from</param>
        /// <example><code lang="javascript">Application.remove("userId");</code></example>
        public static bool remove(string key) {
            State.Application.Remove(key);
            return true;
        }

        /// <summary>Application.clear removes all values.</summary>
        /// <example><code lang="javascript">Application.clear();</code></example>
        public static bool clear() {
            State.Application.Clear();
            return true;
        }

        /// <summary>Application.getAll returns all global values.</summary>
        /// <returns>Object containing all keys and values.</returns>
        /// <example><code lang="javascript">var application = Application.getAll();</code></example>
        public static dynamic getAll() {
            return State.Application.GetObject();
        }
    }
}
