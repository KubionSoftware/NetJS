using NetJS.Core;
using NetJS.Core.API;

namespace NetJS.API {
    public class Config {

        public Config(Agent agent, Settings settings) {
            var file = settings.Root + settings.Config;

            Load(file, agent);
        }

        public void Load(string file, Agent agent) {
            if (System.IO.File.Exists(file)) {
                var content = System.IO.File.ReadAllText(file);

                var config = JSONAPI.parse(Static.Undefined, new[] { new String(content) }, agent);
                agent.Running.GetGlobalObject().Set("Config", config, agent);
            } else {
                agent.Running.GetGlobalObject().Set("Config", Core.Tool.Construct("Object", agent), agent);
            }
        }
    }
}