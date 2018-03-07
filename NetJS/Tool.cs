using NetJS.Core.Javascript;

namespace NetJS {
    public class Tool {

        public static T GetFromScope<T>(Scope scope, string name) {
            var variable = scope.GetVariable(name);
            if(variable is Foreign foreign) {
                if(foreign.Value is T t) {
                    return t;
                }
            }
            return default(T);
        }
    }
}