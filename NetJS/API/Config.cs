using NetJS.Core.Javascript;
using NetJS.Core.API;

namespace NetJS.API {
    public class Config {

        public Config(Scope scope, Settings settings) {
            var file = settings.Root + settings.Config;

            Load(file, scope);
        }

        public void Load(string file, Scope scope) {
            if (System.IO.File.Exists(file)) {
                var content = System.IO.File.ReadAllText(file);

                var config = JSON.parse(Static.Undefined, new[] { new String(content) }, scope);
                scope.SetVariable("Config", config);
            } else {
                scope.SetVariable("Config", Core.Tool.Construct("Object", scope));
            }
        }
    }
}