using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class File : Statement {

        private List<Expression> _expressions;

        public File(List<Expression> expressions) {
            _expressions = expressions;
        }

        public override Result Execute(Scope scope) {
            foreach (var expression in _expressions) {
                scope.Buffer.Append(expression.Execute(scope).ToString());
            }

            return new Result(ResultType.None);
        }
    }
}
