using NetJS.Core.Javascript;
using NetJS.Core.API;

namespace NetJS.API {
    public class Config {

        public Config(Settings settings) {
            var file = settings.Root + settings.Config;

            Load(file);
        }

        public void Load(string file) {
            if (System.IO.File.Exists(file)) {
                var content = System.IO.File.ReadAllText(file);

                //var config = JSON.parse(Static.Undefined, new[] { new String(content) }, scope);
                //scope.DeclareVariable("Config", Core.Javascript.DeclarationScope.Global, true, config);
            } else {
                //scope.DeclareVariable("Config", Core.Javascript.DeclarationScope.Global, true, Core.Tool.Construct("Object", scope));
            }
        }
    }
}