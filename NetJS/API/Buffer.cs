using NetJS.Core;

namespace NetJS.API {
    public class Buffer {

        public static Constant set(Constant _this, Constant[] arguments, Agent agent) {
            var value = ((Core.String)arguments[0]).Value;
            agent.Running.Buffer.Clear();
            agent.Running.Buffer.Append(value);
            return Static.Undefined;
        }

        public static Constant get(Constant _this, Constant[] arguments, Agent agent) {
            return new Core.String(agent.Running.Buffer.ToString());
        }

        public static Constant clear(Constant _this, Constant[] arguments, Agent agent) {
            agent.Running.Buffer.Clear();
            return Static.Undefined;
        }
    }
}