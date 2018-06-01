using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Constructor : Constant {

        public Function Function;

        public Constructor(Function function) {
            Function = function;
        }

        public Constant Call(Constant[] arguments, Constant thisValue, Scope scope) {
            var result = Function.Call(arguments, thisValue, scope);
            if (!(result is Undefined)) {
                return result;
            } else {
                return thisValue;
            }
        }

        public override string ToDebugString() {
            return "[[constructor]]";
        }
    }
}
