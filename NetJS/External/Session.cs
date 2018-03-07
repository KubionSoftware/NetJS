using NetJS.Core.Javascript;
using System.Web;

namespace NetJS.External {
    public class Session {

        public static Constant get(Constant _this, Constant[] arguments, Scope scope) {
            var key = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "Session.get").Value;

            var session = Tool.GetFromScope<JSSession>(scope, "__session__");
            if (session == null) throw new InternalError("No session");
            var value = session == null ? null : session.Get(key);

            if(value == null) return Static.Undefined;
            return value;
        }

        public static Constant set(Constant _this, Constant[] arguments, Scope scope) {
            var key = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "Session.set").Value;
            var value = Core.Tool.GetArgument(arguments, 1, "Session.set");

            var session = Tool.GetFromScope<JSSession>(scope, "__session__");
            if (session == null) throw new InternalError("No session");
            if (session != null) session.Set(key, value);

            return Static.Undefined;
        }

        public static Constant delete(Constant _this, Constant[] arguments, Scope scope) {
            var key = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "Session.delete").Value;

            var session = Tool.GetFromScope<JSSession>(scope, "__session__");
            if (session == null) throw new InternalError("No session");
            if (session != null) session.Remove(key);

            return Static.Undefined;
        }
    }
}