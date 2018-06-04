using NetJS.Core.Javascript;
using System.Web;

namespace NetJS {
    public class Tool {

        public static T GetFromScope<T>(LexicalEnvironment lex, string name) {
            var variable = lex.GetStackVariable(name);
            if(variable is Foreign foreign) {
                if(foreign.Value is T t) {
                    return t;
                }
            }
            return default(T);
        }

        public static JSApplication GetApplication(LexicalEnvironment lex) {
            var application = GetFromScope<JSApplication>(lex, "__application__");
            if (application == null) {
                throw new InternalError("No application");
            }
            return application;
        }

        public static JSSession GetSession(LexicalEnvironment lex) {
            var session = GetFromScope<JSSession>(lex, "__session__");
            if (session == null) {
                throw new InternalError("No session");
            }
            return session;
        }
    }
}