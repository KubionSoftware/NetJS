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

        public override Constant Call(Constant other, Constant _this, Scope scope) {
            var result = Function.Call(other, _this, scope);
            if (!(result is Undefined)) {
                return result;
            } else {
                return _this;
            }
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append("[[constructor]](){}");
        }

        public override string ToDebugString() {
            return "[[constructor]]";
        }
    }
}
