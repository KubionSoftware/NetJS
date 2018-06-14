using NetJS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.API {
    class Uint8ArrayAPI {

        public static Constant constructor(Constant _this, Constant[] arguments, Agent agent) {
            var length = Tool.GetArgument<Number>(arguments, 0, "Uint8Array constructor");

            return new Uint8Array((int)length.Value, agent);
        }
    }
}
