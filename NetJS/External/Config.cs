namespace NetJS.External {
    public class Config {

        public Config(Watch watch, Javascript.Scope scope, Settings settings) {
            var file = settings.Root + settings.Config;

            Load(file, scope);
            watch.Add(file, () => {
                Load(file, scope);
            });
        }

        public void Load(string file, Javascript.Scope scope) {
            if (System.IO.File.Exists(file)) {
                var content = System.IO.File.ReadAllText(file);

                var config = Internal.JSON.parse(Javascript.Static.Undefined, new[] { new Javascript.String(content) }, scope);
                scope.SetVariable("Config", config);
            } else {
                scope.SetVariable("Config", Tool.Construct("Object", scope));
            }
        }
    }
}