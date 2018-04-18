using NetJS.Core.Javascript;
using System.Web;

namespace NetJS {
    public class Tool {

        public static T GetFromScope<T>(Scope scope, string name) {
            var variable = scope.GetStackVariable(name);
            if(variable is Foreign foreign) {
                if(foreign.Value is T t) {
                    return t;
                }
            }
            return default(T);
        }

        public static JSApplication GetApplication(Scope scope) {
            var application = GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) {
                throw new InternalError("No application");
            }
            return application;
        }

        public static JSSession GetSession(Scope scope) {
            var session = GetFromScope<JSSession>(scope, "__session__");
            if (session == null) {
                throw new InternalError("No session");
            }
            return session;
        }
    }
}