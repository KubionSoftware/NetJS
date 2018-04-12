using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Access : BinaryOperator {
        // TODO: better name
        public bool IsDot;

        public Access(bool isDot) : base(16) {
            IsDot = isDot;
        }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.Access(right, scope);
        }

        public override Constant Execute(Scope scope, bool getValue = true) {
            var left = Left.Execute(scope, false);
            var right = Right.Execute(scope, !IsDot);

            var result = Execute(left, right, scope);
            return getValue ? result.GetValue(scope) : result;
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);

            if (IsDot) {
                builder.Append(Tokens.Access);
                Right.Uneval(builder, depth);
            } else {
                builder.Append(Tokens.ArrayOpen);
                Right.Uneval(builder, depth);
                builder.Append(Tokens.ArrayClose);
            }
        }

        public override string ToDebugString() {
            return "access";
        }
    }
}
