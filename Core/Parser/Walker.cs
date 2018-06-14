using System.Collections.Generic;
using System;

namespace NetJS.Core {
    public class Walker {

        private static T PerformAction<T>(Func<Node, Node> action, Node node) where T : Node {
            var result = action(node);

            if (result != null) {
                if (result is Expression e) {
                    Walk(e, action);
                } else if (result is Statement s) {
                    Walk(s, action);
                }

                return (T)result;
            } else {
                return (T)node;
            }
        }

        public static Expression Walk(Expression expression, Func<Node, Node> action) {
            if (expression is Operator op) {
                op.SetLeft(PerformAction<Expression>(action, op.GetLeft));
                op.SetRight(PerformAction<Expression>(action, op.GetRight));
            } else if (expression is FunctionLiteral function) {
                function.Body = PerformAction<Statement>(action, function.Body);
            } else if (expression is ClassLiteral c) {
                c.Constructor = (FunctionLiteral)PerformAction<Expression>(action, c.Constructor);

                for (var i = 0; i < c.PrototypeMethods.Count; i++) {
                    c.PrototypeMethods[i] = (FunctionLiteral)PerformAction<Expression>(action, c.PrototypeMethods[i]);
                }

                for (var i = 0; i < c.StaticMethods.Count; i++) {
                    c.StaticMethods[i] = (FunctionLiteral)PerformAction<Expression>(action, c.StaticMethods[i]);
                }
            } else if (expression is ArgumentList a) {
                for (var i = 0; i < a.Arguments.Length; i++) {
                    a.Arguments[i] = PerformAction<Expression>(action, a.Arguments[i]);
                }
            }

            return expression;
        }

        public static Statement Walk(Statement statement, Func<Node, Node> action) {
            if (statement is ExpressionStatement expression) {
                expression.Expression = PerformAction<Expression>(action, expression.Expression);
            } else if (statement is Block block) {
                block.StatementList = PerformAction<StatementList>(action, block.StatementList);
            } else if (statement is StatementList sl) {
                var statements = sl.List;
                for (var i = 0; i < statements.Count; i++) {
                    statements[i] = PerformAction<Statement>(action, statements[i]);
                }
            } else if (statement is For forNode) {
                forNode.First = PerformAction<Node>(action, forNode.First);
                forNode.Second = PerformAction<Expression>(action, forNode.Second);
                forNode.Third = PerformAction<Expression>(action, forNode.Third);
                forNode.Stmt = PerformAction<Statement>(action, forNode.Stmt);
            } else if (statement is While whileNode) {
                whileNode.Check = PerformAction<Expression>(action, whileNode.Check);
                whileNode.Body = PerformAction<Statement>(action, whileNode.Body);
            } else if (statement is DoWhile doWhileNode) {
                doWhileNode.Check = PerformAction<Expression>(action, doWhileNode.Check);
                doWhileNode.Body = PerformAction<Statement>(action, doWhileNode.Body);
            } else if (statement is ForInOf forInOfNode) {
                forInOfNode.Declaration = PerformAction<VariableDeclaration>(action, forInOfNode.Declaration);
                forInOfNode.Collection = PerformAction<Expression>(action, forInOfNode.Collection);
                forInOfNode.Body = PerformAction<Statement>(action, forInOfNode.Body);
            } else if (statement is If ifNode) {
                ifNode.Test = PerformAction<Expression>(action, ifNode.Test);
                ifNode.TrueStmt = PerformAction<Statement>(action, ifNode.TrueStmt);
                ifNode.FalseStmt = PerformAction<Statement>(action, ifNode.FalseStmt);
            } else if (statement is Try tryNode) {
                tryNode.TryBody = PerformAction<Block>(action, tryNode.TryBody);
                tryNode.CatchBody = PerformAction<Block>(action, tryNode.CatchBody);
                tryNode.FinallyBody = PerformAction<Block>(action, tryNode.FinallyBody);
            } else if (statement is VariableDeclaration declaration) {
                foreach (var dec in declaration.Declarations) {
                    dec.Expression = PerformAction<Expression>(action, dec.Expression);
                }
            }

            return statement;
        }
    }
}