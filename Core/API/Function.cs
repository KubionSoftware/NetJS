using NetJS.Core;

namespace NetJS.Core.API {
    class FunctionAPI {

        public static Constant constructor(Constant _this, Constant[] arguments, Agent agent) {
            var thisObject = (Object)_this;

            return Static.Undefined;
        }

        public static Constant call(Constant _this, Constant[] arguments, Agent agent) {
            var f = _this as Function;

            var thisArg = Tool.GetArgument(arguments, 0, "Function.call");
            var args = new Constant[arguments.Length - 1];
            for (var i = 1; i < arguments.Length; i++) {
                args[i - 1] = arguments[i];
            }

            return f.Call(thisArg, agent, args);
        }
    }
}
