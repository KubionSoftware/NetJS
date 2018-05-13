using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Call : BinaryOperator {
        public Call() : base(16) { }

        public override Constant Execute(Scope scope, bool getValue = true) {
            Constant _this = Static.Undefined;

            var left = Left.Execute(scope, false);

            if (left is Path path) {
                _this = path.GetThis(scope);
            }

            left = left.Execute(scope);
            if (left is Constructor constructor) {
                _this = new Object(constructor.Function.Get<Object>("prototype"));
            }

            if (Right is ArgumentList arguments) {
                return left.Call(arguments, _this, scope);
            } else {
                // TODO: better error message
                throw new InternalError("Call without arguments... Contact developer, this should not happen");
            }
        }

        public override string ToDebugString() {
            return "call";
        }
    }
}
