using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {

    public enum DeclarationScope {
        Engine,
        Global,
        Function,
        Block
    }

    public class VariableDeclaration : Statement {

        public class DeclarationVariable {
            public Constant Name;
            public Type Type;
            public Expression Expression;

            public DeclarationVariable(Constant name, Type type, Expression expression) {
                Name = name;
                Type = type;
                Expression = expression;
            }

            public DeclarationVariable(string name, Type type, Expression expression) {
                Name = new String(name);
                Type = type;
                Expression = expression;
            }
        }

        public List<DeclarationVariable> Declarations = new List<DeclarationVariable>();

        public DeclarationScope Scope;
        public bool IsConstant;

        public VariableDeclaration(DeclarationScope lex, bool isConstant) {
            Scope = lex;
            IsConstant = isConstant;
        }

        public override Completion Evaluate(Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-let-and-const-declarations

            foreach (var declaration in Declarations) {
                var lhs = References.ResolveBinding(declaration.Name, null, agent);

                Constant value = Static.Undefined;
                if (declaration.Expression != null) {
                    var rhs = declaration.Expression.Evaluate(agent);
                    value = References.GetValue(rhs, agent);
                }

                // TODO: anonymous function name

                if (Scope == DeclarationScope.Block) {
                    References.InitializeReferencedBinding(lhs, value);
                } else {
                    References.PutValue(lhs, value, agent);
                }
            }

            return Static.NormalCompletion;
        }

        public Constant[] GetBoundNames() {
            var result = new Constant[Declarations.Count];
            for (var i = 0; i < Declarations.Count; i++) result[i] = Declarations[i].Name;
            return result;
        }
    }
}
