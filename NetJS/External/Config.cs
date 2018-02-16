using NetJS.Javascript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetJS.External {
    public class Config {

        public Config(Watch watch, Scope scope, Settings settings) {
            var file = settings.Root + settings.Config;

            Load(file, scope);
            watch.Add(file, () => {
                Load(file, scope);
            });
        }

        public void Load(string file, Scope scope) {
            if (System.IO.File.Exists(file)) {
                var content = System.IO.File.ReadAllText(file);

                var config = Internal.JSON.parse(Static.Undefined, new[] { new Javascript.String(content) }, scope);
                scope.SetVariable("Config", config);
            } else {
                scope.SetVariable("Config", Tool.Construct("Object", scope));
            }
        }
    }
}