using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public abstract class Expression : Node {

        public virtual Constant Execute(Scope scope, bool getValue = true) {
            return Static.Undefined;
        }

        public bool IsTrue(Scope scope) {
            var result = Execute(scope);

            if (result is Boolean b) {
                return b.Value;
            } else if (result is Number n) {
                return n.Value != 0;
            } else if (result is String s) {
                return s.Value.Length > 0;
            }

            return (!(result is Undefined || result is Null || result is NaN));
        }

        public virtual Constant GetValue(Scope scope) {
            return Static.Undefined;
        }

        public abstract string ToDebugString();
    }
}
