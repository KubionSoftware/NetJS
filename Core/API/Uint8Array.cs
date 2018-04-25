using NetJS.Core.Javascript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.API {
    class Uint8Array {

        public static Constant constructor(Constant _this, Constant[] arguments, Scope scope) {
            var length = Tool.GetArgument<Javascript.Number>(arguments, 0, "Uint8Array constructor");

            return new Javascript.Uint8Array((int)length.Value);
        }
    }
}
