using NetJS.Core.Javascript;

namespace NetJS.Core.API {
    class Object {

        public static Constant constructor(Constant _this, Constant[] arguments, Scope scope) {
            var thisObject = (Javascript.Object)_this;

            return Static.Undefined;
        }

        // TODO: this should be a static method Object.keys(obj) instead of obj.keys()
        public static Constant keys(Constant _this, Constant[] arguments, Scope scope) {
            if(_this is Javascript.Object o) {
                return Tool.ToArray(o.GetKeys(), scope);
            }

            return Static.Undefined;
        }

        public static Constant toString(Constant _this, Constant[] arguments, Scope scope) {
            // Because this is actually useful
            return JSON.stringify(null, new Constant[] { _this }, scope);

            // According to javascript specification
            // return new Javascript.String("[object Object]");
        }
    }
}
