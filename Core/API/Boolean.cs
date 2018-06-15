using NetJS.Core;

namespace NetJS.Core.API {
    class BooleanAPI {

        public static Constant constructor(Constant _this, Constant[] arguments, Agent agent) {
            var thisObject = (Object)_this;

            return Static.Undefined;
        }

        public static Constant toString(Constant _this, Constant[] arguments, Agent agent) {
            return new String(((Boolean)_this).Value ? "true" : "false");
        }
    }
}