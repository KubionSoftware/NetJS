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
    }

    public class Break : Statement {

        public override Result Execute(Scope scope) {
            return new Result(ResultType.Break);
        }
    }

    public class Continue : Statement {

        public override Result Execute(Scope scope) {
            return new Result(ResultType.Continue);
        }
    }

    public class Throw : Statement {
        public Expression Expression;

        public override Result Execute(Scope scope){
            var value = Expression.Execute(scope);
            var message = Tool.ToString(value, scope);
            throw new Error(message);
        }
    }
}
