using System;
using System.Collections.Generic;
using System.Text;

namespace NetJS.Core.Javascript {

    public class Block : Statement {
        public IList<Node> Nodes = new List<Node>();

        public override Result Execute(Scope scope) {
            var output = new StringBuilder();
            var depth = scope.Depth();

            foreach (var node in Nodes) {
#if debug_enabled
                if (Debug.BreakpointNodes.Contains(node.Id)) {
                    Debug.SteppingLevel = scope.Depth();
                    Debug.Break("stopOnBreakpoint", scope.GetStackTrace(Debug.GetNodeLocation(node.Id)), scope.GetScopes());
                } else if (Debug.SteppingInto && depth > Debug.SteppingLevel) {
                    Debug.SteppingLevel++;
                    Debug.SteppingInto = false;
                    Debug.Break("stopOnBreakpoint", scope.GetStackTrace(Debug.GetNodeLocation(node.Id)), scope.GetScopes());
                } else if (Debug.SteppingOut && depth < Debug.SteppingLevel) {
                    Debug.SteppingLevel--;
                    Debug.SteppingOut = false;
                    Debug.Break("stopOnBreakpoint", scope.GetStackTrace(Debug.GetNodeLocation(node.Id)), scope.GetScopes());
                } else if (Debug.SteppingOver && depth <= Debug.SteppingLevel) {
                    Debug.SteppingLevel = depth;
                    Debug.SteppingOver = false;
                    Debug.Break("stopOnBreakpoint", scope.GetStackTrace(Debug.GetNodeLocation(node.Id)), scope.GetScopes());
                }
#endif

                try {
                    if (node is Statement statement) {
                        var result = statement.Execute(scope);
                        if (result.Type == ResultType.None) continue;

                        if (result.Type == ResultType.Return || result.Type == ResultType.Break || result.Type == ResultType.Throw || result.Type == ResultType.Continue) {
                            if (result.Constant == null && output.Length > 0) {
                                result.Constant = new String(output.ToString());
                            }

                            return result;
                        } else {
                            var str = result.Constant.GetString(scope);
                            if (str != null && str.Length > 0) output.Append(str);
                        }
                    } else if (node is Expression expression) {
                        var result = expression.Execute(scope);
                        var str = result.GetString(scope);
                        if (str != null && str.Length > 0) output.Append(str);
                    } else if (node is Html html) {
                        output.Append(html.ToString(scope));
                    }
                } catch (Error e) {
#if debug_enabled
                    Debug.SteppingLevel = scope.Depth();
                    var location = Debug.GetNodeLocation(node.Id);
                    Debug.Break("stopOnException", scope.GetStackTrace(location), scope.GetScopes());

                    e.AddStackTrace(location);
#endif

                    // Rethrow the error so it keeps traveling up
                    throw;
                }
            }

            if (output.Length > 0) {
                return new Result(ResultType.String, new String(output.ToString()));
            } else {
                return new Result(ResultType.None);
            }
        }

        public override void Uneval(StringBuilder builder, int depth) {
            for(var i = 0; i < Nodes.Count; i++) {
                var node = Nodes[i];

                if (i > 0) NewLine(builder, depth);
                node.Uneval(builder, depth);

                if (!(node is If || node is For || node is ForOf || node is ForIn || node is While || node is Try || node is Html)) {
                    builder.Append(Tokens.ExpressionEnd);
                }
            }
        }
    }

    public class If : Statement {
        public class IfBlock {
            public Expression Check;
            public Block Body;

            public IfBlock(Expression check, Block body) {
                Check = check;
                Body = body;
            }
        }

        public List<IfBlock> Ifs = new List<IfBlock>();
        public Block Else;

        public override Result Execute(Scope scope) {
            foreach (var ifNode in Ifs) {
                if (ifNode.Check.IsTrue(scope)) {
                    return ifNode.Body.Execute(scope);
                }
            }

            if (Else != null) {
                return Else.Execute(scope);
            }

            return new Result(ResultType.None, Static.Undefined);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            for(var i = 0; i < Ifs.Count; i++) {
                var ifNode = Ifs[i];
                if (i > 0) builder.Append(Tokens.Else);

                builder.Append(Tokens.If + Tokens.GroupOpen);
                ifNode.Check.Uneval(builder, depth);
                builder.Append(Tokens.GroupClose + Tokens.BlockOpen);

                NewLine(builder, depth + 1);
                ifNode.Body.Uneval(builder, depth + 1);
                NewLine(builder, depth);

                builder.Append(Tokens.BlockClose);
            }

            if(Else != null) {
                builder.Append(Tokens.Else + Tokens.BlockOpen);

                NewLine(builder, depth + 1);
                Else.Uneval(builder, depth + 1);
                NewLine(builder, depth);

                builder.Append(Tokens.BlockClose);
            }
        }
    }

    public abstract class LoopExecution {

        private const int MaxLoops = 100000000;

        public Block Body;

        public LoopExecution(Block body) {
            Body = body;
        }

        public abstract bool Start(Scope scope);
        public abstract bool Before(Scope scope);
        public abstract bool After(Scope scope);

        public Result Execute(Node node, Scope parent) {
            var scope = new Scope(parent, node, ScopeType.Block);
            if (!Start(scope)) return new Result(ResultType.None);

            var output = new StringBuilder();

            var i = 0;
            while (true) {
                if (Before(scope)) {
                    var result = Body.Execute(scope);

                    if (result.Type == ResultType.Break) {
                        var str = result.Constant.GetString(scope);
                        if (str != null) output.Append(str);
                        break;
                    } else if (result.Type == ResultType.Return || result.Type == ResultType.Throw) {
                        return result;
                    } else if (result.Type != ResultType.None) {
                        // continue, variable or html
                        var str = result.Constant.GetString(scope);
                        if (str != null) output.Append(str);
                    }

                    if (!After(scope)) {
                        break;
                    }

                    i++;
                    if (i >= MaxLoops) {
                        var message = "Maximum number of loops exceeded";
#if debug_enabled
                        throw new Exception(Debug.Message(node, message));
#else
                        throw new Exception(message);
#endif
                    }
                } else {
                    break;
                }
            }

            if (output.Length > 0) {
                return new Result(ResultType.String, new String(output.ToString()));
            } else {
                return new Result(ResultType.None);
            }
        }
    }

    public class For : Statement {
        public Declaration Declaration;
        public Expression Check;
        public Expression Action;
        public Block Body;

        class ForExecution : LoopExecution {

            private For _forNode;

            public ForExecution(For forNode) : base(forNode.Body) {
                _forNode = forNode;
            }

            public override bool Start(Scope scope) {
                _forNode.Declaration.Execute(scope);
                return true;
            }

            public override bool Before(Scope scope) {
                return _forNode.Check.IsTrue(scope);
            }

            public override bool After(Scope scope) {
                _forNode.Action.Execute(scope);
                return true;
            }
        }

        public override Result Execute(Scope parent) {
            return new ForExecution(this).Execute(this, parent);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.For + Tokens.GroupOpen);
            Declaration.Uneval(builder, depth);
            builder.Append(Tokens.ExpressionEnd + " ");
            Check.Uneval(builder, depth);
            builder.Append(Tokens.ExpressionEnd + " ");
            Action.Uneval(builder, depth);
            builder.Append(Tokens.GroupClose + Tokens.BlockOpen);
            NewLine(builder, depth + 1);
            Body.Uneval(builder, depth + 1);
            NewLine(builder, depth);
            builder.Append(Tokens.BlockClose);
        }
    }

    public class ForOf : Statement {
        public Declaration Declaration;
        public Expression Collection;
        public Block Body;

        class ForOfExecution : LoopExecution {

            private ForOf _forOfNode;
            private Array _array;
            private int _index;

            public ForOfExecution(ForOf forOfNode) : base(forOfNode.Body) {
                _forOfNode = forOfNode;
            }

            public override bool Start(Scope scope) {
                var array = _forOfNode.Collection.Execute(scope);
                if (!(array is Array)) return false;

                _array = (Array)array;
                _index = 0;

                return true;
            }

            public override bool Before(Scope scope) {
                if (_index >= _array.List.Count) return false;
                // TODO: better way to declare
                _forOfNode.Declaration.Declarations[0].Variable.Assignment(_array.List[_index], scope);
                _index++;
                return true;
            }

            public override bool After(Scope scope) {
                return true;
            }
        }

        public override Result Execute(Scope parent) {
            return new ForOfExecution(this).Execute(this, parent);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.For + Tokens.GroupOpen);
            Declaration.Uneval(builder, depth);
            builder.Append(" " + Tokens.ForOf + " ");
            Collection.Uneval(builder, depth);
            builder.Append(Tokens.GroupClose + Tokens.BlockOpen);
            NewLine(builder, depth + 1);
            Body.Uneval(builder, depth + 1);
            NewLine(builder, depth);
            builder.Append(Tokens.BlockClose);
        }
    }

    public class ForIn : Statement {
        public Declaration Declaration;
        public Expression Collection;
        public Block Body;

        class ForInExecution : LoopExecution {

            private ForIn _forInNode;
            private Object _obj;
            private string[] _keys;
            private int _index;

            public ForInExecution(ForIn forInNode) : base(forInNode.Body) {
                _forInNode = forInNode;
            }

            public override bool Start(Scope scope) {
                var objResult = _forInNode.Collection.Execute(scope);
                if (!(objResult is Object)) return false;

                _obj = (Object)objResult;
                _keys = _obj.GetKeys();
                _index = 0;

                return true;
            }

            public override bool Before(Scope scope) {
                if (_index >= _keys.Length) return false;
                // TODO: better way to declare
                _forInNode.Declaration.Declarations[0].Variable.Assignment(new String(_keys[_index]), scope);
                _index++;
                return true;
            }

            public override bool After(Scope scope) {
                return true;
            }
        }

        public override Result Execute(Scope parent) {
            return new ForInExecution(this).Execute(this, parent);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.For + Tokens.GroupOpen);
            Declaration.Uneval(builder, depth);
            builder.Append(" " + Tokens.ForIn + " ");
            Collection.Uneval(builder, depth);
            builder.Append(Tokens.GroupClose + Tokens.BlockOpen);
            NewLine(builder, depth + 1);
            Body.Uneval(builder, depth + 1);
            NewLine(builder, depth);
            builder.Append(Tokens.BlockClose);
        }
    }

    public class While : Statement {
        public Expression Check;
        public Block Body;

        class WhileExecution : LoopExecution {

            private While _whileNode;

            public WhileExecution(While whileNode) : base(whileNode.Body) {
                _whileNode = whileNode;
            }

            public override bool Start(Scope scope) {
                return true;
            }

            public override bool Before(Scope scope) {
                return _whileNode.Check.IsTrue(scope);
            }

            public override bool After(Scope scope) {
                return true;
            }
        }

        public override Result Execute(Scope parent) {
            return new WhileExecution(this).Execute(this, parent);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.While + Tokens.GroupOpen);
            Check.Uneval(builder, depth);
            builder.Append(Tokens.GroupClose + Tokens.BlockOpen);
            NewLine(builder, depth + 1);
            Body.Uneval(builder, depth + 1);
            NewLine(builder, depth);
            builder.Append(Tokens.BlockClose);
        }
    }

    public class DoWhile : Statement {
        public Expression Check;
        public Block Body;

        class DoWhileExecution : LoopExecution {

            private DoWhile _doWhileNode;

            public DoWhileExecution(DoWhile doWhileNode) : base(doWhileNode.Body) {
                _doWhileNode = doWhileNode;
            }

            public override bool Start(Scope scope) {
                return true;
            }

            public override bool Before(Scope scope) {
                return true;
            }

            public override bool After(Scope scope) {
                return _doWhileNode.Check.IsTrue(scope);
            }
        }

        public override Result Execute(Scope parent) {
            return new DoWhileExecution(this).Execute(this, parent);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.While + Tokens.GroupOpen);
            Check.Uneval(builder, depth);
            builder.Append(Tokens.GroupClose + Tokens.BlockOpen);
            NewLine(builder, depth + 1);
            Body.Uneval(builder, depth + 1);
            NewLine(builder, depth);
            builder.Append(Tokens.BlockClose);
        }
    }

    public class Return : Statement {
        public Expression Expression;

        public override Result Execute(Scope scope) {
            return new Result(ResultType.Return, Expression == null ? null : Expression.Execute(scope));
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.Return);

            if(Expression != null) {
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

    public class Try : Statement {
        public Block TryBody;
        public Variable CatchVariable;
        public Block CatchBody;
        public Block FinallyBody;

        public override Result Execute(Scope scope) {
            Result result;

            try {
                result = TryBody.Execute(scope);
                if (result.Type != ResultType.Throw) {
                    return result;
                }
            } catch (Exception e) {
                result = new Result(ResultType.Throw, new String(e.Message));
            }

            if (CatchBody != null) {
                if (CatchVariable != null) CatchVariable.Assignment(result.Constant, scope);
                var catchResult = CatchBody.Execute(scope);
                if (FinallyBody == null || catchResult.Type == ResultType.Return || catchResult.Type == ResultType.Throw) {
                    return catchResult;
                }
            }

            if (FinallyBody != null) {
                return FinallyBody.Execute(scope);
            }

            return new Result(ResultType.None);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.Try + Tokens.BlockOpen);
            NewLine(builder, depth + 1);
            TryBody.Uneval(builder, depth + 1);
            NewLine(builder, depth);
            builder.Append(Tokens.BlockClose);

            if(CatchBody != null) {
                builder.Append(Tokens.Catch);

                if(CatchVariable != null) {
                    builder.Append(Tokens.GroupOpen);
                    CatchVariable.Uneval(builder, depth);
                    builder.Append(Tokens.GroupClose);
                }

                builder.Append(Tokens.BlockOpen);
                NewLine(builder, depth + 1);
                CatchBody.Uneval(builder, depth + 1);
                NewLine(builder, depth);
                builder.Append(Tokens.BlockClose);
            }

            if(FinallyBody != null) {
                builder.Append(Tokens.Finally + Tokens.BlockOpen);
                NewLine(builder, depth + 1);
                FinallyBody.Uneval(builder, depth + 1);
                NewLine(builder, depth);
                builder.Append(Tokens.BlockClose);
            }
        }
    }

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

            for(var i = 0; i < Declarations.Count; i++) {
                if (i > 0) builder.Append(", ");

                var declaration = Declarations[i];
                declaration.Variable.Uneval(builder, depth);
                builder.Append(" " + Tokens.Assign + " ");
                declaration.Expression.Uneval(builder, depth);
            }
        }
    }
}