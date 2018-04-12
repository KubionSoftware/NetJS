using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Return : Statement {
        public Expression Expression;

        public override Result Execute(Scope scope) {
            return new Result(ResultType.Return, Expression == null ? Static.Undefined : Expression.Execute(scope));
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.Return);

            if (Expression != null) {
                builder.Append(" ");
                Expression.Uneval(builder, depth);
            }
        }
    }

    public class Break : Statement {

        public override Result Execute(Scope scope) {
            return new Result(ResultType.Break);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.Break);
        }
    }

    public class Continue : Statement {

        public override Result Execute(Scope scope) {
            return new Result(ResultType.Continue);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.Continue);
        }
    }

    public class Throw : Statement {
        public Expression Expression;

        public override Result Execute(Scope scope) {
            return new Result(ResultType.Throw, Expression.Execute(scope));
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.Throw);
        }
    }
}
