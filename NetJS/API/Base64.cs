using NetJS.Core.Javascript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetJS.API {
    public class Base64 {

        /// <summary>Decodes a base64 string.</summary>
        /// <param name="base64">The string to decode</param>
        /// <returns>Returns the decoded string.</returns>
        /// <example><code lang="javascript">var decoded = Base64.decode("TWFuIGlzIGRpc3Rpbmd1aXNoZWQ=");</code></example>
        public static Constant decode(Constant _this, Constant[] arguments, Scope scope) {
            var value = ((Core.Javascript.String)arguments[0]).Value;
            return new Core.Javascript.String(Util.Base64.Decode(value));
        }

        /// <summary>Encodes a string as a base64 string.</summary>
        /// <param name="s">The string to encode</param>
        /// <returns>Returns the encoded string.</returns>
        /// <example><code lang="javascript">var encoded = Base64.encode("Man is distinguished");</code></example>
        public static Constant encode(Constant _this, Constant[] arguments, Scope scope) {
            var value = ((Core.Javascript.String)arguments[0]).Value;
            return new Core.Javascript.String(Util.Base64.Encode(value));
        }
    }
}