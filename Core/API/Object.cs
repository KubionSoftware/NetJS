using NetJS.Core;
using System.Linq;

namespace NetJS.Core.API {
    class ObjectAPI {

        public static Constant constructor(Constant _this, Constant[] arguments, Agent agent) {
            var thisObject = (Object)_this;

            return Static.Undefined;
        }

        [StaticFunction]
        public static Constant keys(Constant _this, Constant[] arguments, Agent agent) {
            var o = Tool.GetArgument<Object>(arguments, 0, "Object.keys");
            return Tool.ToArray(o.OwnPropertyKeys().Select(key => key.ToString()), agent);
        }

        [StaticFunction]
        public static Constant values(Constant _this, Constant[] arguments, Agent agent) {
            var o = Tool.GetArgument<Object>(arguments, 0, "Object.values");
            var keys = o.OwnPropertyKeys();
            var array = new Array(0, agent);
            foreach(var key in keys) {
                array.Add(o.Get(key));
            }
            return array;
        }

        public static Constant toString(Constant _this, Constant[] arguments, Agent agent) {
            // Because this is actually useful
            return JSONAPI.stringify(null, new Constant[] { _this }, agent);

            // According to javascript specification
            // return new String("[object Object]");
        }
    }
}
