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

                // Convert all static access to paths
                if (op is Access access && access.IsKey) {
                    if (access.Left is Constant left && access.Right is Constant right) {
                        var path = new Path() {
                            Parts = new List<Constant>() { }
                        };

                        if (left is Path pl) {
                            path.Parts.AddRange(pl.Parts);
                        } else {
                            path.Parts.Add(left);
                        }

                        if (right is Path pr) {
                            path.Parts.AddRange(pr.Parts);
                        } else {
                            path.Parts.Add(right);
                        }

                        return path;
                    }
                }
            } else if (expression is FunctionBlueprint function) {
                function.Body = Optimize(function.Body);
            } else if (expression is ClassBlueprint c) {
                c.Constructor = (FunctionBlueprint)Optimize(c.Constructor);

                for (var i = 0; i < c.PrototypeMethods.Count; i++) {
                    c.PrototypeMethods[i] = (FunctionBlueprint)Optimize(c.PrototypeMethods[i]);
                }

                for (var i = 0; i < c.StaticMethods.Count; i++) {
                    c.StaticMethods[i] = (FunctionBlueprint)Optimize(c.StaticMethods[i]);
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
                forNode.Body = Optimize(forNode.Body);
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
            } else if (node is Declaration declaration) {
                foreach (var dec in declaration.Declarations) {
                    dec.Expression = Optimize(dec.Expression);
                }
            }

            return node;
        }
    }
}