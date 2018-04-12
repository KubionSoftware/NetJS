using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Declaration : Statement {
        public class DeclarationVariable {
            public Variable Variable;
            public Expression Expression;

            public DeclarationVariable(Variable variable, Expression expression) {
                Variable = variable;
                Expression = expression;
            }
        }

        public List<DeclarationVariable> Declarations = new List<DeclarationVariable>();

        public override Result Execute(Scope scope) {
            foreach (var declaration in Declarations) {
                scope.DeclareVariable(declaration.Variable.Name, Static.Undefined);
                if (declaration.Expression != null) {
                    declaration.Variable.Assignment(declaration.Expression.Execute(scope), scope);
                }
            }

            return new Result(ResultType.None);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.Variable + " ");

            for (var i = 0; i < Declarations.Count; i++) {
                if (i > 0) builder.Append(", ");

                var declaration = Declarations[i];
                declaration.Variable.Uneval(builder, depth);
                builder.Append(" " + Tokens.Assign + " ");
                declaration.Expression.Uneval(builder, depth);
            }
        }
    }
}
