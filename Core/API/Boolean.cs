using NetJS.Core.Javascript;

namespace NetJS.Core.API {
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