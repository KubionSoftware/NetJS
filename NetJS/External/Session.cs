using NetJS.Javascript;
using System.Web;

namespace NetJS.External {
    public class Session {

        public static Constant get(Constant _this, Constant[] arguments, Scope scope) {
            var key = Tool.GetArgument<Javascript.String>(arguments, 0, "Session.get").Value;

            var value = scope.Session == null ? null : scope.Session.Get(key);

            if(value == null) return Javascript.Static.Undefined;
            return value;
        }

        public static Constant set(Constant _this, Constant[] arguments, Scope scope) {
            var key = Tool.GetArgument<Javascript.String>(arguments, 0, "Session.set").Value;
            var value = Tool.GetArgument(arguments, 1, "Session.set");

            if (scope.Session != null) scope.Session.Set(key, value);

            return Static.Undefined;
        }

        public static Constant delete(Constant _this, Constant[] arguments, Scope scope) {
            var key = Tool.GetArgument<Javascript.String>(arguments, 0, "Session.delete").Value;

            if (scope.Session != null) scope.Session.Remove(key);

            return Static.Undefined;
        }
    }
}