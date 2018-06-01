using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {

    public enum DeclarationScope {
        Engine,
        Global,
        Function,
        Block
    }

    public class Declaration : Statement {

        public class DeclarationVariable {
            public string Name;
            public Type Type;
            public Expression Expression;

            public DeclarationVariable(string name, Type type, Expression expression) {
                Name = name;
                Type = type;
                Expression = expression;
            }
        }

        public List<DeclarationVariable> Declarations = new List<DeclarationVariable>();

        public DeclarationScope Scope;
        public bool IsConstant;

        public Declaration(DeclarationScope scope, bool isConstant) {
            Scope = scope;
            IsConstant = isConstant;
        }

        public override Result Execute(Scope scope) {
            foreach (var declaration in Declarations) {
                Constant value = Static.Undefined;

                if (declaration.Expression != null) {
                    value = declaration.Expression.Execute(scope);
                }

                scope.DeclareVariable(declaration.Name, Scope, IsConstant, value, declaration.Type);
            }

            return new Result(ResultType.None);
        }
    }
}
