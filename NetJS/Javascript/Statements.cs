using System;
using System.Collections.Generic;
using System.Text;

namespace NetJS.Javascript {

    public class Block : Statement {
        public IList<Node> Nodes = new List<Node>();

        public override Scope.Result Execute(Scope scope) {
            var output = new StringBuilder();

            foreach (var node in Nodes) {
                try {
                    if (node is Statement) {
#if DEBUG
                        if (Debug.BreakpointNodes.Contains(node.Id)) {
                            Debug.Break("stopOnBreakpoint", scope.GetStackTrace(Debug.GetNodeLocation(node.Id)), scope.GetScopes());
                        }
#endif

                        var result = ((Statement)node).Execute(scope);

                        if (result.Type == Scope.ResultType.Return || result.Type == Scope.ResultType.Break || result.Type == Scope.ResultType.Throw || result.Type == Scope.ResultType.Continue) {
                            if (result.Constant == null && output.Length > 0) {
                                result.Constant = new String(output.ToString());
                            }

                            return result;
                        } else {
                            var str = result.Constant.GetString(scope);
                            if (str != null && str.Length > 0) output.Append(str);
                        }
                    } else if (node is Expression) {
                        var result = ((Expression)node).Execute(scope);
                        var str = result.GetString(scope);
                        if (str != null && str.Length > 0) output.Append(str);
                    } else if (node is Html) {
                        output.Append(((Html)node).ToString(scope));
                    }
                } catch (Error e) {
#if DEBUG
                    var location = Debug.GetNodeLocation(node.Id);
                    Debug.Break("stopOnException", scope.GetStackTrace(location), scope.GetScopes());

                    e.AddStackTrace(location);
#endif

                    // Rethrow the error so it keeps traveling up
                    throw;
                }
            }

            if (output.Length > 0) {
                return new Scope.Result(Scope.ResultType.String, new String(output.ToString()));
            } else {
                return new Scope.Result(Scope.ResultType.None);
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

        public override Scope.Result Execute(Scope scope) {
            foreach (var ifNode in Ifs) {
                if (ifNode.Check.IsTrue(scope)) {
                    return ifNode.Body.Execute(scope);
                }
            }

            if (Else != null) {
                return Else.Execute(scope);
            }

            return new Scope.Result(Scope.ResultType.None, Static.Undefined);
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

    public abstract class Loopscope {

        private const int MaxLoops = 10000;

        public Block Body;

        public Loopscope(Block body) {
            Body = body;
        }

        public abstract bool Start(Scope scope);
        public abstract bool Before(Scope scope);
        public abstract void After(Scope scope);

        public Scope.Result Execute(Node node, Scope parent) {
            var scope = new Scope(parent, node);
            if (!Start(scope)) return new Scope.Result(Scope.ResultType.None);

            var output = new StringBuilder();

            var i = 0;
            while (true) {
                if (Before(scope)) {
                    var result = Body.Execute(scope);

                    if (result.Type == Scope.ResultType.Break) {
                        var str = result.Constant.GetString(scope);
                        if (str != null) output.Append(str);
                        break;
                    } else if (result.Type == Scope.ResultType.Return || result.Type == Scope.ResultType.Throw) {
                        return result;
                    } else if (result.Type != Scope.ResultType.None) {
                        // continue, variable or html
                        var str = result.Constant.GetString(scope);
                        if (str != null) output.Append(str);
                    }

                    After(scope);

                    i++;
                    if (i >= MaxLoops) {
                        throw new Exception(Debug.Message(node, "Maximum number of loops exceeded"));
                    }
                } else {
                    break;
                }
            }

            if (output.Length > 0) {
                return new Scope.Result(Scope.ResultType.String, new String(output.ToString()));
            } else {
                return new Scope.Result(Scope.ResultType.None);
            }
        }
    }

    public class For : Statement {
        public Declaration Declaration;
        public Expression Check;
        public Expression Action;
        public Block Body;

        class Forscope : Loopscope {

            private For _forNode;

            public Forscope(For forNode) : base(forNode.Body) {
                _forNode = forNode;
            }

            public override bool Start(Scope scope) {
                _forNode.Declaration.Execute(scope);
                return true;
            }

            public override bool Before(Scope scope) {
                return _forNode.Check.IsTrue(scope);
            }

            public override void After(Scope scope) {
                _forNode.Action.Execute(scope);
            }
        }

        public override Scope.Result Execute(Scope parent) {
            return new Forscope(this).Execute(this, parent);
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

        class ForOfscope : Loopscope {

            private ForOf _forOfNode;
            private Array _array;
            private int _index;

            public ForOfscope(ForOf forOfNode) : base(forOfNode.Body) {
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

            public override void After(Scope scope) { }
        }

        public override Scope.Result Execute(Scope parent) {
            return new ForOfscope(this).Execute(this, parent);
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

        class ForInscope : Loopscope {

            private ForIn _forInNode;
            private Object _obj;
            private string[] _keys;
            private int _index;

            public ForInscope(ForIn forInNode) : base(forInNode.Body) {
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

            public override void After(Scope scope) { }
        }

        public override Scope.Result Execute(Scope parent) {
            return new ForInscope(this).Execute(this, parent);
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

        class Whilescope : Loopscope {

            private While _whileNode;

            public Whilescope(While whileNode) : base(whileNode.Body) {
                _whileNode = whileNode;
            }

            public override bool Start(Scope scope) {
                return true;
            }

            public override bool Before(Scope scope) {
                return _whileNode.Check.IsTrue(scope);
            }

            public override void After(Scope scope) { }
        }

        public override Scope.Result Execute(Scope parent) {
            return new Whilescope(this).Execute(this, parent);
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

        public override Scope.Result Execute(Scope scope) {
            return new Scope.Result(Scope.ResultType.Return, Expression == null ? null : Expression.Execute(scope));
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

        public override Scope.Result Execute(Scope scope) {
            return new Scope.Result(Scope.ResultType.Break);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.Break);
        }
    }

    public class Continue : Statement {

        public override Scope.Result Execute(Scope scope) {
            return new Scope.Result(Scope.ResultType.Continue);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.Continue);
        }
    }

    public class Throw : Statement {
        public Expression Expression;

        public override Scope.Result Execute(Scope scope) {
            return new Scope.Result(Scope.ResultType.Throw, Expression.Execute(scope));
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

        public override Scope.Result Execute(Scope scope) {
            Scope.Result result;

            try {
                result = TryBody.Execute(scope);
                if (result.Type != Scope.ResultType.Throw) {
                    return result;
                }
            } catch (Exception e) {
                result = new Scope.Result(Scope.ResultType.Throw, new String(e.Message));
            }

            if (CatchBody != null) {
                if (CatchVariable != null) CatchVariable.Assignment(result.Constant, scope);
                var catchResult = CatchBody.Execute(scope);
                if (FinallyBody == null || catchResult.Type == Scope.ResultType.Return || catchResult.Type == Scope.ResultType.Throw) {
                    return catchResult;
                }
            }

            if (FinallyBody != null) {
                return FinallyBody.Execute(scope);
            }

            return new Scope.Result(Scope.ResultType.None);
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

        public override Scope.Result Execute(Scope scope) {
            foreach (var declaration in Declarations) {
                scope.DeclareVariable(declaration.Variable.Name, Static.Undefined);
                if (declaration.Expression != null) {
                    declaration.Variable.Assignment(declaration.Expression.Execute(scope), scope);
                }
            }

            return new Scope.Result(Scope.ResultType.None);
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