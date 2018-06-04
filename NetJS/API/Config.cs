using NetJS.Core.Javascript;
using NetJS.Core.API;

namespace NetJS.API {
    public class Config {

        public Config(LexicalEnvironment lex, Settings settings) {
            var file = settings.Root + settings.Config;

            Load(file, lex);
        }

        public void Load(string file, LexicalEnvironment lex) {
            if (System.IO.File.Exists(file)) {
                var content = System.IO.File.ReadAllText(file);

                var config = JSON.parse(Static.Undefined, new[] { new String(content) }, lex);
                lex.GetGlobalObject().Set("Config", config);
            } else {
                lex.GetGlobalObject().Set("Config", Core.Tool.Construct("Object", lex));
            }
        }
    }
}