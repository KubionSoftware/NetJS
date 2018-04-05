using NetJS.Core.Javascript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetJS.External {
    public class Base64 {

        public static Constant decode(Constant _this, Constant[] arguments, Scope scope) {
            var value = ((Core.Javascript.String)arguments[0]).Value;
            return new Core.Javascript.String(Util.Base64.Decode(value));
        }

        public static Constant encode(Constant _this, Constant[] arguments, Scope scope) {
            var value = ((Core.Javascript.String)arguments[0]).Value;
            return new Core.Javascript.String(Util.Base64.Encode(value));
        }
    }
}