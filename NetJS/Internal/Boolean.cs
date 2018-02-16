using NetJS.Javascript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetJS.Internal {
    class Boolean {

        public static Constant constructor(Constant _this, Constant[] arguments, Scope scope) {
            var thisObject = (Javascript.Object)_this;

            return Static.Undefined;
        }

        public static Constant toString(Constant _this, Constant[] arguments, Scope scope) {
            return new Javascript.String(((Javascript.Boolean)_this).Value ? "true" : "false");
        }
    }
}