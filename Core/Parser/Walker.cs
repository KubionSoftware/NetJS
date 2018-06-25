using System.Collections.Generic;
using System;

namespace NetJS.Core {
    public class Walker {

        private static void PerformAction(Func<Node, Node> action, Node node) {
            var result = action(node);

            if (result != null) {
                if (result is Expression e) {
                    Walk(e, action);
                } else if (result is Statement s) {
                    Walk(s, action);
                }
            }
        }

        public static void Walk(Expression expression, Func<Node, Node> action) {
            if (expression is Operator op) {
                PerformAction(action, op.GetLeft);
                PerformAction(action, op.GetRight);
            } else if (expression is FunctionLiteral function) {
                PerformAction(action, function.Body);
            } else if (expression is ClassLiteral c) {
                PerformAction(action, c.Constructor);

                for (var i = 0; i < c.PrototypeMethods.Count; i++) {
                    PerformAction(action, c.PrototypeMethods[i]);
                }

                for (var i = 0; i < c.StaticMethods.Count; i++) {
                    PerformAction(action, c.StaticMethods[i]);
                }
            } else if (expression is ArgumentList a) {
                for (var i = 0; i < a.Arguments.Length; i++) {
                    PerformAction(action, a.Arguments[i]);
                }
            }
        }

        public static void Walk(Statement statement, Func<Node, Node> action) {
            if (statement is ExpressionStatement expression) {
                PerformAction(action, expression.Expression);
            } else if (statement is Block block) {
                PerformAction(action, block.StatementList);
            } else if (statement is StatementList sl) {
                var statements = sl.List;
                for (var i = 0; i < statements.Count; i++) {
                    PerformAction(action, statements[i]);
                }
            } else if (statement is For forNode) {
                PerformAction(action, forNode.First);
                PerformAction(action, forNode.Second);
                PerformAction(action, forNode.Third);
                PerformAction(action, forNode.Stmt);
            } else if (statement is While whileNode) {
                PerformAction(action, whileNode.Check);
                PerformAction(action, whileNode.Body);
            } else if (statement is DoWhile doWhileNode) {
                PerformAction(action, doWhileNode.Check);
                PerformAction(action, doWhileNode.Body);
            } else if (statement is ForInOf forInOfNode) {
                PerformAction(action, forInOfNode.Declaration);
                PerformAction(action, forInOfNode.Collection);
                PerformAction(action, forInOfNode.Body);
            } else if (statement is If ifNode) {
                PerformAction(action, ifNode.Test);
                PerformAction(action, ifNode.TrueStmt);
                PerformAction(action, ifNode.FalseStmt);
            } else if (statement is Try tryNode) {
                PerformAction(action, tryNode.TryBody);
                PerformAction(action, tryNode.CatchBody);
                PerformAction(action, tryNode.FinallyBody);
            } else if (statement is VariableDeclaration declaration) {
                foreach (var dec in declaration.Declarations) {
                    PerformAction(action, dec.Expression);
                }
            }
        }
    }
}