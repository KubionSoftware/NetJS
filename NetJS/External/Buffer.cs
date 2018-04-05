using NetJS.Core.Javascript;

namespace NetJS.External {
    public class Buffer {

        public static Constant set(Constant _this, Constant[] arguments, Scope scope) {
            var value = ((Core.Javascript.String)arguments[0]).Value;
            scope.Buffer.Clear();
            scope.Buffer.Append(value);
            return Static.Undefined;
        }

        public static Constant get(Constant _this, Constant[] arguments, Scope scope) {
            return new Core.Javascript.String(scope.Buffer.ToString());
        }

        public static Constant clear(Constant _this, Constant[] arguments, Scope scope) {
            scope.Buffer.Clear();
            return Static.Undefined;
        }
    }
}