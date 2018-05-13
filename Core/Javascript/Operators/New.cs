using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class New : UnaryRightOperator {
        public New() : base(16) { }

        public override Constant Execute(Constant right, Scope scope) {
            return right.New(scope);
        }

        public override string ToDebugString() {
            return "new";
        }
    }
}
