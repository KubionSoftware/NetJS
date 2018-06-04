using System.Collections.Generic;

namespace NetJS.Core.Javascript {
    public class Optimizer {

        public static Block Optimize(Block code) {
            if (code == null) return null;

            for(var i = 0; i < code.Nodes.Count; i++) {
                code.Nodes[i] = Optimize(code.Nodes[i]);
            }
            

            return code;
        }

        private static Expression Optimize(Expression expression) {
            if (expression is Operator op) {
                op.SetLeft(Optimize(op.GetLeft));
                op.SetRight(Optimize(op.GetRight));

                // TODO: Convert all static access to references
                
            } else if (expression is FunctionLiteral function) {
                function.Body = Optimize(function.Body);
            } else if (expression is ClassLiteral c) {
                c.Constructor = (FunctionLiteral)Optimize(c.Constructor);

                for (var i = 0; i < c.PrototypeMethods.Count; i++) {
                    c.PrototypeMethods[i] = (FunctionLiteral)Optimize(c.PrototypeMethods[i]);
                }

                for (var i = 0; i < c.StaticMethods.Count; i++) {
                    c.StaticMethods[i] = (FunctionLiteral)Optimize(c.StaticMethods[i]);
                }
            } else if (expression is ArgumentList a) {
                for (var i = 0; i < a.Arguments.Length; i++) {
                    a.Arguments[i] = Optimize(a.Arguments[i]);
                }
            }

            return expression;
        }

        private static Node Optimize(Node node) {
            if (node is Expression expression) {
                return Optimize(expression);
            } else if (node is Block block) {
                return Optimize(block);
            } else if (node is For forNode) {
                forNode.Stmt = Optimize(forNode.Stmt);
            } else if (node is While whileNode) {
                whileNode.Body = Optimize(whileNode.Body);
            } else if (node is ForOf forOfNode) {
                forOfNode.Body = Optimize(forOfNode.Body);
            } else if (node is ForIn forInNode) {
                forInNode.Body = Optimize(forInNode.Body);
            } else if (node is If ifNode) {
                foreach (var i in ifNode.Ifs) {
                    i.Body = Optimize(i.Body);
                }
                ifNode.Else = Optimize(ifNode.Else);
            } else if (node is Try tryNode) {
                tryNode.TryBody = Optimize(tryNode.TryBody);
                tryNode.CatchBody = Optimize(tryNode.CatchBody);
                tryNode.FinallyBody = Optimize(tryNode.FinallyBody);
            } else if (node is Node declaration) {
                foreach (var dec in declaration.Declarations) {
                    dec.Expression = Optimize(dec.Expression);
                }
            }

            return node;
        }
    }
}