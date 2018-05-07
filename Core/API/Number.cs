using NetJS.Core.Javascript;
using System;
using System.Globalization;

namespace NetJS.Core.API {

    class Number {

        public static Constant constructor(Constant _this, Constant[] arguments, Scope scope) {
            var thisObject = (Javascript.Object)_this;

            return Static.Undefined;
        }

        public static Constant toString(Constant _this, Constant[] arguments, Scope scope) {
            return new Javascript.String(((Javascript.Number)_this).Value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
