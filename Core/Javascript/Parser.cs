using System;
using System.Collections.Generic;

namespace NetJS.Core.Javascript {

    public class Parser {

        private const int GroupPrecedence = 20;

        private IList<Token> _tokens;
        private int _index = 0;

        private Dictionary<string, Func<Statement>> _statements;
        private Dictionary<string, Func<Expression, Expression>> _expressions;
        private Dictionary<string, Func<Expression, Expression>> _parseExpressions;
        private Dictionary<string, Func<Operator>> _assignExpressions;
        private HashSet<string> _stopTokens;
        
        private int _fileId;

        public Parser(int fileId, IList<Token> tokens) {
            _tokens = tokens;
            
            _fileId = fileId;

            _statements = new Dictionary<string, Func<Statement>>() {
                { Tokens.Variable, ParseDeclaration },
                { Tokens.Function, ParseFunctionDeclaration },
                { Tokens.Return, ParseReturn },
                { Tokens.If, ParseIf },
                { Tokens.Switch, ParseSwitch },
                { Tokens.For, ParseFor },
                { Tokens.While, ParseWhile },
                { Tokens.Do, ParseDoWhile },
                { Tokens.Break, () => { _index++; return new Break(); } },
                { Tokens.Continue, () => { _index++; return new Continue(); } },
                { Tokens.Throw, ParseThrow },
                { Tokens.Try, ParseTry }
            };

            _expressions = new Dictionary<string, Func<Expression, Expression>> {
                { Tokens.True, (l) => new BooleanBlueprint(true) },
                { Tokens.False, (l) => new BooleanBlueprint(false) },
                { Tokens.Null, (l) => Static.Null },
                { Tokens.Undefined, (l) => Static.Undefined },
                { Tokens.NotANumber, (l) => Static.NaN },
                { Tokens.Infinity, (l) => Static.Infinity },
                { Tokens.New, (l) => new New() },
                { Tokens.Add, (l) => new Addition() },
                { Tokens.Substract, (l) => {
                    if(l == null) {
                        return new Negation();
                    }

                    if(l is Operator) {
                        var op = (Operator)l;
                        if (!op.HasRight) {
                            return new Negation();
                        }
                    }

                    return new Substraction();
                } },
                { Tokens.Multiply, (l) => new Multiplication() },
                { Tokens.Remainder, (l) => new Remainder() },
                { Tokens.Assign, (l) => new Assignment() },
                { Tokens.Equals, (l) => new Equals() },
                { Tokens.NotEquals, (l) => new NotEquals() },
                { Tokens.StrictEquals, (l) => new StrictEquals() },
                { Tokens.StrictNotEquals, (l) => new StrictNotEquals() },
                { Tokens.GreaterThan, (l) => new GreaterThan() },
                { Tokens.GreaterThanEquals, (l) => new GreaterThanEquals() },
                { Tokens.LessThan, (l) => new LessThan() },
                { Tokens.LessThanEquals, (l) => new LessThanEquals() },
                { Tokens.LogicalAnd, (l) => new LogicalAnd() },
                { Tokens.LogicalOr, (l) => new LogicalOr() },
                { Tokens.LogicalNot, (l) => new LogicalNot() },
                { Tokens.TypeOf, (l) => new TypeOf() },
                { Tokens.InstanceOf, (l) => new InstanceOf() },
                { Tokens.Void, (l) => new Void() },
                { Tokens.Delete, (l) => new Delete() },
                { Tokens.BitwiseAnd, (l) => new BitwiseAnd() },
                { Tokens.BitwiseOr, (l) => new BitwiseOr() },
                { Tokens.BitwiseXor, (l) => new BitwiseXor() },
                { Tokens.BitwiseNot, (l) => new BitwiseNot() },
                { Tokens.LeftShift, (l) => new LeftShift() },
                { Tokens.RightShift, (l) => new RightShift() },
                { Tokens.In, (l) => new In() },
                { Tokens.Access, (l) => new Access(true) }
            };

            _parseExpressions = new Dictionary<string, Func<Expression, Expression>> {
                { Tokens.Function, (l) => ParseAnonymousFunction() },
                { Tokens.BlockOpen, (l) => ParseObject() },
                { Tokens.ArrayOpen, (l) => {
                    return IsVariable(LastExpression(l)) ? (Expression)ParseArrayAccess() : ParseArray();
                } },
                { Tokens.Conditional, (l) => ParseConditional() }
            };

            _assignExpressions = new Dictionary<string, Func<Operator>> {
                { Tokens.Add + Tokens.Assign, () => new Addition() },
                { Tokens.Substract + Tokens.Assign, () => new Substraction() },
                { Tokens.Multiply + Tokens.Assign, () => new Multiplication() },
                { Tokens.Divide + Tokens.Assign, () => new Division() },
                { Tokens.Remainder + Tokens.Assign, () => new Remainder() },
                { Tokens.LeftShift + Tokens.Assign, () => new LeftShift() },
                { Tokens.RightShift + Tokens.Assign, () => new RightShift() },
                { Tokens.BitwiseAnd + Tokens.Assign, () => new BitwiseAnd() },
                { Tokens.BitwiseOr + Tokens.Assign, () => new BitwiseOr() },
                { Tokens.BitwiseXor + Tokens.Assign, () => new BitwiseXor() }
            };

            _stopTokens = new HashSet<string>() {
                Tokens.ExpressionEnd,
                Tokens.Sequence,
                Tokens.GroupClose,
                Tokens.BlockClose,
                Tokens.ArrayClose,
                Tokens.ConditionalSeperator,
                Tokens.Case,
                Tokens.Default
            };
        }

#if debug_enabled
        public Debug.Location GetLocation(int index) {
            // TODO: throw error?
            if (index >= _tokens.Count) return new Debug.Location(_fileId, -1);
            var token = _tokens[index];
            return new Debug.Location(_fileId, token.Line);
        }
#endif

        public Block Parse() {
#if debug_enabled
            Debug.RemoveNodes(_fileId);
#endif

            var result = ParseStatements();
            result = Optimizer.Optimize(result);
            return result;
        }

        public Error CreateError(string s) {
            var error = new SyntaxError(s);

#if debug_enabled
            error.AddStackTrace(GetLocation(_index));
#endif

            return error;
        }

        public Block ParseStatements(int maxStatements = -1) {
            var list = new Block();

            while (_index < _tokens.Count) {
                var token = _tokens[_index];

                if (token.Type == Token.Group.WhiteSpace || (token.Type != Token.Group.String && token.Content == Tokens.ExpressionEnd)) {
                    _index++;
                } else if (token.Type == Token.Group.Html) {
                    var html = new Html(token.Content);

                    if (list.Nodes.Count > 0) {
                        var last = list.Nodes[list.Nodes.Count - 1];
                        if (last is Html lastHtml) {
                            lastHtml.Combine(html);
                            _index++;
                            continue;
                        }
                    }

                    list.Nodes.Add(html);
                    _index++;
                } else if (token.Type == Token.Group.Comment) {
                    _index++;
                } else if (token.Type != Token.Group.String && (token.Content == Tokens.BlockClose || token.Content == Tokens.Case || token.Content == Tokens.ConditionalSeperator || token.Content == Tokens.Default)) {
                    break;
                } else if (_statements.ContainsKey(token.Content)) {
                    var startIndex = _index;
                    var node = _statements[token.Content]();

#if debug_enabled
                    node.RegisterDebug(GetLocation(startIndex));
#endif

                    list.Nodes.Add(node);
                } else {
                    var startIndex = _index;
                    var expression = ParseExpression();
                    if (expression == null) {
                        if (_index == startIndex) {
                            throw CreateError($"Did not expect token '{token.Content}' while parsing statement");
                        }
                    } else {
                        list.Nodes.Add(expression);
                    }
                }

                if(maxStatements != -1 && list.Nodes.Count >= maxStatements) {
                    break;
                }
            }

            return list;
        }

        public string Next(string context) {
            while (_index < _tokens.Count) {
                var token = _tokens[_index];
                _index++;
                if (token.Type != Token.Group.WhiteSpace && token.Type != Token.Group.Comment) return token.Content;
            }

            throw CreateError($"No '{context}' token found");
        }

        public string Peek(int offset = 0) {
            var index = _index;

            while (index < _tokens.Count) {
                var token = _tokens[index];
                if (token.Type != Token.Group.WhiteSpace && token.Type != Token.Group.Comment) {
                    if (offset == 0) return token.Content;
                    offset--;
                }
                index++;
            }

            return "";
        }

        public void Skip(string skipToken) {
            while (_index < _tokens.Count) {
                var token = _tokens[_index];

                if (token.Type != Token.Group.WhiteSpace && token.Type != Token.Group.Comment) {
                    if (token.Content == skipToken) {
                        _index++;
                        return;
                    } else {
                        throw CreateError("Expected token '" + skipToken + "' but found '" + token + "'");
                    }
                }
                _index++;
            }
            throw CreateError("Expected token '" + skipToken + "'");
        }

        public string ParseType() {
            Skip(Tokens.TypeSeperator);
            var type = Next("type name");

            if (Peek() == Tokens.ArrayOpen && Peek(1) == Tokens.ArrayClose) {
                Skip(Tokens.ArrayOpen);
                Skip(Tokens.ArrayClose);
                type += Tokens.ArrayOpen + Tokens.ArrayClose;
            }

            return type;
        }

        public Declaration ParseDeclaration() {
            Skip(Tokens.Variable);

            var result = new Declaration();

            while (true) {
                var name = Next("variable name");
                Variable variable;

                if(Peek() == Tokens.TypeSeperator) {
                    var type = ParseType();
                    variable = new TypedVariable(name, type);
                } else {
                    variable = new Variable(name);
                }

                Expression expression = null;
                if (Peek() == Tokens.Assign) {
                    Skip(Tokens.Assign);
                    expression = ParseExpression();

                    if(expression is FunctionBlueprint function) {
                        function.Name = variable.Name;
                    }
                }

                result.Declarations.Add(new Declaration.DeclarationVariable(variable, expression));

                if (Peek() != Tokens.Sequence) {
                    break;
                }
                Skip(Tokens.Sequence);
            }

            return result;
        }

        public Return ParseReturn() {
            Skip(Tokens.Return);

            if (Peek() == Tokens.ExpressionEnd) {
                return new Return();
            }

            return new Return() { Expression = ParseExpression() };
        }

        public Expression LastExpression(Expression expression) {
            if(expression is Operator) {
                var op = (Operator)expression;
                if (op.HasRight) return op.GetRight;
            }

            return expression;
        }

        public bool IsVariable(Expression expression) {
            return expression is Variable || expression is Access || expression is New || expression is FunctionBlueprint || expression is ArgumentList;
        }

        public void CombineExpression(ref Expression left, Expression expression) {
            if (left == null) {
                left = expression;
            } else if (left is Operator leftOperation) {
                if (expression is Operator expressionOperation) {
                    if (expressionOperation.Precedence > leftOperation.Precedence) {
                        // Go down
                        if (expressionOperation.AcceptsLeft && !expressionOperation.HasLeft) {
                            expressionOperation.SetLeft(leftOperation.GetRight);
                        }

                        leftOperation.SetRight(expression);
                        expressionOperation.Parent = leftOperation;
                    } else {
                        if (expressionOperation.AcceptsLeft) {
                            // Go up
                            var currentLeftOperation = leftOperation;
                            while (currentLeftOperation.Parent != null && expressionOperation.Precedence <= currentLeftOperation.Parent.Precedence) {
                                currentLeftOperation = currentLeftOperation.Parent;
                            }

                            expressionOperation.SetLeft(currentLeftOperation);

                            if (currentLeftOperation.Parent != null) {
                                expressionOperation.Parent = currentLeftOperation.Parent;

                                if (currentLeftOperation.Parent.GetLeft == currentLeftOperation) currentLeftOperation.Parent.SetLeft(expressionOperation);
                                if (currentLeftOperation.Parent.GetRight == currentLeftOperation) currentLeftOperation.Parent.SetRight(expressionOperation);
                            }

                            currentLeftOperation.Parent = expressionOperation;
                        } else {
                            // Go down
                            leftOperation.SetRight(expressionOperation);
                            expressionOperation.Parent = leftOperation;
                        }
                    }

                    left = expression;
                } else {
                    if (!leftOperation.AcceptsRight) {
                        throw CreateError($"Operator {leftOperation.ToDebugString()} doesn't accept a right-hand argument {expression.ToDebugString()}");
                    }else if (leftOperation.HasRight) {
                        throw CreateError($"Operator {leftOperation.ToDebugString()} already has a right-hand argument {leftOperation.GetRight.ToDebugString()} can't set {expression.ToDebugString()}");
                    }

                    leftOperation.SetRight(expression);
                }
            } else if (expression is Operator op) {
                if (op.AcceptsLeft) {
                    op.SetLeft(left);
                    left = op;
                } else {
                    throw CreateError($"Operator {op.ToDebugString()} doesn't accept a left-hand argument {left.ToDebugString()}");
                }
            }

            if (expression == null) {
                throw CreateError("Expression is null");
            }

#if debug_enabled
            expression.RegisterDebug(GetLocation(_index));
#endif
        }

        public void CombineAssign(ref Expression left, Operator op) {
            var l = left;
            CombineExpression(ref left, new Assignment());
            CombineExpression(ref left, l);
            op.Precedence = 2.5f;
            CombineExpression(ref left, op);
        }

        public Expression ParseExpression() {
            var parts = new List<string>();
            Expression left = null;

            while (_index < _tokens.Count) {
                var token = _tokens[_index];

                if (token.Type != Token.Group.String && (_stopTokens.Contains(token.Content) || token.Type == Token.Group.Html)) break;

                if (token.Type == Token.Group.WhiteSpace) {
                    _index++;
                } else if (token.Type == Token.Group.String) {
                    CombineExpression(ref left, new StringBlueprint(token.Content));
                    _index++;
                } else if (token.Type == Token.Group.Template) {
                    CombineExpression(ref left, ParseTemplate(token.Content));
                    _index++;
                } else if (token.Type == Token.Group.Number) {
                    CombineExpression(ref left, new NumberBlueprint(Double.Parse(token.Content)));
                    _index++;
                } else if (token.Type == Token.Group.Comment) {
                    _index++;
                } else if (_expressions.ContainsKey(token.Content)) {
                    CombineExpression(ref left, _expressions[token.Content](left));
                    _index++;
                } else if (_parseExpressions.ContainsKey(token.Content)) {
                    CombineExpression(ref left, _parseExpressions[token.Content](left));
                } else if (_assignExpressions.ContainsKey(token.Content)) {
                    CombineAssign(ref left, _assignExpressions[token.Content]());
                    _index++;
                } else if (token.Content == Tokens.Divide) {
                    CombineExpression(ref left, ParseDivide(left));
                } else if (token.Content == Tokens.Increment) {
                    CombineExpression(ref left, ParsePreOrPost(left, new PrefixIncrement(), new PostfixIncrement()));
                    _index++;
                } else if (token.Content == Tokens.Decrement) {
                    CombineExpression(ref left, ParsePreOrPost(left, new PrefixDecrement(), new PostfixDecrement()));
                    _index++;
                } else if (token.Content == Tokens.GroupOpen) {
                    CombineExpression(ref left, ParseGroup(left));
                } else if (_statements.ContainsKey(token.Content)) {
                    break;
                } else {
                    CombineExpression(ref left, ParseVariable(token.Content));
                }
            }

            while (true) {
                if (!(left is Operator)) {
                    break;
                }

                var op = (Operator)left;
                if (op.Parent != null) {
                    left = op.Parent;
                } else {
                    break;
                }
            }

            return left;
        }

        public Expression ParseVariable(string content) {
            var variable = new Variable(content);
            _index++;

            if (Peek() == Tokens.ArrowFunction) {
                var parameters = new ParameterList();
                parameters.Parameters.Add(variable);
                return ParseArrowFunction(parameters);
            } else {
                return variable;
            }
        }

        public Expression ParseDivide(Expression left) {
            if (left == null) {
                return ParseRegex();
            }

            if (left is Operator) {
                var op = (Operator)left;
                if (!op.HasRight) {
                    return ParseRegex();
                }
            }

            _index++;
            return new Division();
        }

        public Expression ParsePreOrPost(Expression left, Expression pre, Expression post) {
            if (left == null) {
                return pre;
            }

            if (left is Operator) {
                var op = (Operator)left;
                if (!op.HasRight) {
                    return pre;
                }
            }

            return post;
        }

        public Expression ParseGroup(Expression left) {
            if (IsVariable(LastExpression(left))) {
                return ParseCall();
            } else {
                if (Peek(2) == Tokens.Sequence) {
                    var parameters = ParseParameters();
                    return ParseArrowFunction(parameters);
                } else {
                    Skip(Tokens.GroupOpen);
                    var expression = ParseExpression();
                    Skip(Tokens.GroupClose);

                    if (expression == null && Peek() == Tokens.ArrowFunction) {
                        return ParseArrowFunction(new ParameterList());
                    } else {
                        if (expression is Operator expressionOperation) {
                            expressionOperation.Precedence = GroupPrecedence;
                        }
                        return expression;
                    }
                }
            }
        }

        public int CountEscapes(string text) {
            var count = 0;

            for (var i = text.Length - 1; i >= 0; i--) {
                if (text[i] != '\\') return count;
                count++;
            }

            return count;
        }

        public Expression ParseRegex() {
            Skip(Tokens.RegexOpen);

            string buffer = "";
            while(_index < _tokens.Count) {
                var token = _tokens[_index];

                if (token.Content == Tokens.RegexClose && CountEscapes(buffer) % 2 == 0) break;

                buffer += token.Content;
                _index++;
            }

            Skip(Tokens.RegexClose);

            var peek = _index < _tokens.Count ? _tokens[_index].Content : "\0";
            var valid = true;
            foreach(var c in peek) {
                if (!Tokens.IsValidRegexFlag(c)) {
                    valid = false;
                    break;
                }
            }

            var flags = valid ? Next("regex flags") : "";

            return new Call() {
                Left = new New() {
                    Right = new Variable("RegExp")
                },
                Right = new ArgumentList() {
                    Arguments = new List<Expression>() {
                        new StringBlueprint(buffer),
                        new StringBlueprint(flags)
                    }
                }
            };
        }

        public NumberBlueprint ParseNumber() {
            var token = _tokens[_index];
            var s = token.Content;
            
            if(token.Content == Tokens.Access) {
                Skip(Tokens.Access);
                s += Next("number");
            }

            var peek = Peek();
            if(peek.ToLower() == Tokens.Exponent) {
                Skip(peek);
                s += Tokens.Exponent;
                s += Next("number exponent");
            }

            double value;
            if(double.TryParse(s, out value)) {
                return new NumberBlueprint(value);
            } else {
                throw CreateError("Invalid number '" + s + "'");
            }
        }

        public Expression ParseTemplate(string template) {
            var inExpression = false;
            var depth = 0;
            var buffer = "";

            var parts = new List<Expression>();

            for (var i = 0; i < template.Length; i++) {
                var c = template[i];

                if (inExpression) {
                    if(c == '{') {
                        buffer += c;
                        depth++;
                    }else if(c == '}') {
                        depth--;
                        if(depth == 0) {
                            if (buffer.Length > 0) {
                                var tokens = Lexer.Lex(buffer, _fileId);
                                var parser = new Parser(_fileId, tokens);
                                var expression = parser.ParseExpression();

                                parts.Add(expression);
                                buffer = "";
                            }
                            inExpression = false;
                        } else {
                            buffer += c;
                        }
                    } else {
                        buffer += c;
                    }
                } else if (c == '$' && i < template.Length - 1 && template[i + 1] == '{') {
                    inExpression = true;
                    i++;
                    depth = 1;
                    if(buffer.Length > 0) {
                        parts.Add(new StringBlueprint(buffer));
                        buffer = "";
                    }
                } else {
                    buffer += c;
                }
            }

            if (buffer.Length > 0) {
                parts.Add(new StringBlueprint(buffer));
                buffer = "";
            }

            if (parts.Count == 0) return new StringBlueprint("");

            Expression result = parts[0];
            for(var i = 1; i < parts.Count; i++) {
                result = new Addition() { Left = result, Right = parts[i] };
            }

            if (result is Operator op) op.Precedence = GroupPrecedence;
            return result;
        }

        public While ParseWhile() {
            Skip(Tokens.While);

            Skip(Tokens.GroupOpen);
            var check = ParseExpression();
            Skip(Tokens.GroupClose);

            var body = ParseBlock();

            return new While() { Check = check, Body = body };
        }

        public DoWhile ParseDoWhile() {
            Skip(Tokens.Do);

            var body = ParseBlock();

            Skip(Tokens.While);

            Skip(Tokens.GroupOpen);
            var check = ParseExpression();
            Skip(Tokens.GroupClose);

            return new DoWhile() { Check = check, Body = body };
        }

        public Block ParseBlock() {
            if (Peek() == Tokens.BlockOpen) {
                Skip(Tokens.BlockOpen);
                var body = ParseStatements();
                Skip(Tokens.BlockClose);

                return body;
            } else {
                return ParseStatements(1);
            }
        }

        public Statement ParseFor() {
            Skip(Tokens.For);
            Skip(Tokens.GroupOpen);

            var declaration = ParseDeclaration();

            var peek = Peek();
            if (peek == Tokens.ExpressionEnd) {
                Skip(Tokens.ExpressionEnd);
                var check = ParseExpression();
                Skip(Tokens.ExpressionEnd);
                var action = ParseExpression();
                Skip(Tokens.GroupClose);

                return new For() { Declaration = declaration, Check = check, Action = action, Body = ParseBlock() };
            } else if (peek == Tokens.ForIn) {
                Skip(Tokens.ForIn);
                var collection = ParseExpression();
                Skip(Tokens.GroupClose);

                return new ForIn() { Declaration = declaration, Collection = collection, Body = ParseBlock() };
            } else if (peek == Tokens.ForOf) {
                Skip(Tokens.ForOf);
                var collection = ParseExpression();
                Skip(Tokens.GroupClose);

                return new ForOf() { Declaration = declaration, Collection = collection, Body = ParseBlock() };
            }

            throw CreateError("Incorrect for loop");
        }

        public If.IfBlock ParseIfPart() {
            Skip(Tokens.If);
            Skip(Tokens.GroupOpen);

            var check = ParseExpression();

            Skip(Tokens.GroupClose);

            var body = ParseBlock();

            return new If.IfBlock(check, body);
        }

        public If ParseIf() {
            var result = new If();
            result.Ifs.Add(ParseIfPart());

            while (Peek() == Tokens.Else) {
                Skip(Tokens.Else);

                if (Peek() == Tokens.If) {
                    result.Ifs.Add(ParseIfPart());
                } else {
                    result.Else = ParseBlock();
                }
            }

            return result;
        }

        public If ParseSwitch() {
            Skip(Tokens.Switch);

            Skip(Tokens.GroupOpen);
            var reference = ParseExpression();
            Skip(Tokens.GroupClose);

            Skip(Tokens.BlockOpen);

            var result = new If();

            while (Peek() == Tokens.Case) {
                Skip(Tokens.Case);
                var value = ParseExpression();
                var check = new Equals() { Left = reference, Right = value };

                Skip(Tokens.CaseSeperator);

                var body = ParseStatements();

                result.Ifs.Add(new If.IfBlock(check, body));
            }

            if (Peek() == Tokens.Default) {
                Skip(Tokens.Default);
                Skip(Tokens.CaseSeperator);

                var body = ParseStatements();

                result.Else = body;
            }

            Skip(Tokens.BlockClose);

            return result;
        }

        public Access ParseArrayAccess() {
            Skip(Tokens.ArrayOpen);

            var value = ParseExpression();

            Skip(Tokens.ArrayClose);

            return new Access(false) { Right = value };
        }

        public ArrayBlueprint ParseArray() {
            var list = new List<Expression>();

            Skip(Tokens.ArrayOpen);
            
            while (Peek() != Tokens.ArrayClose) {
                var value = ParseExpression();
                list.Add(value);

                var next = Peek();

                if (next == Tokens.ArrayClose) {
                    break;
                } else if (next == Tokens.Sequence) {
                    Skip(Tokens.Sequence);
                } else {
                    throw CreateError("Invalid array");
                }
            }

            Skip(Tokens.ArrayClose);

            return new ArrayBlueprint(list);
        }

        public ObjectBlueprint ParseObject() {
            var blueprints = new Dictionary<string, Expression>();

            Skip(Tokens.BlockOpen);

            while (Peek() != Tokens.BlockClose) {
                var key = Next("object key");

                Skip(Tokens.KeyValueSeperator);

                var value = ParseExpression();

                blueprints[key] = value;

                var peek = Peek();
                if (peek != Tokens.Sequence && peek != Tokens.BlockClose) {
                    throw CreateError("Invalid object");
                }

                if (peek == Tokens.BlockClose) {
                    break;
                }

                Skip(Tokens.Sequence);
            }

            Skip(Tokens.BlockClose);

            return new ObjectBlueprint(blueprints);
        }

        public ParameterList ParseParameters() {
            var list = new ParameterList();

            Skip(Tokens.GroupOpen);

            if (Peek() != Tokens.GroupClose) {
                while (_index < _tokens.Count) {
                    var name = Next("parameter name");
                    
                    if (!Tokens.IsValidName(name)) {
                        throw CreateError($"Invalid parameter name '{name}'");
                    }

                    var peek = Peek();

                    if (peek == Tokens.TypeSeperator) {
                        var type = ParseType();
                        peek = Peek();

                        list.Parameters.Add(new TypedVariable(name, type));
                    } else {
                        list.Parameters.Add(new Variable(name));
                    }

                    if (peek == Tokens.GroupClose) {
                        break;
                    } else if (peek == Tokens.Sequence) {
                        Skip(Tokens.Sequence);
                        continue;
                    }

                    throw CreateError("Unclosed parameter list");
                }
            }

            Skip(Tokens.GroupClose);

            return list;
        }

        public Expression ParseAnonymousFunction() {
            Skip(Tokens.Function);

            var parameters = ParseParameters();

            var type = "";
            if(Peek() == Tokens.TypeSeperator) {
                type = ParseType();
            }

            var body = ParseBlock();

            return new FunctionBlueprint("", type, parameters, body);
        }

        public Statement ParseFunctionDeclaration() {
            Skip(Tokens.Function);

            var name = Next("function name");

            var parameters = ParseParameters();

            var type = "";
            if (Peek() == Tokens.TypeSeperator) {
                type = ParseType();
            }

            var body = ParseBlock();

            var function = new FunctionBlueprint(name, type, parameters, body);

            var list = new Block();
            var declaration = new Declaration();
            declaration.Declarations.Add(new Declaration.DeclarationVariable(new Variable(name), function));
            list.Nodes.Add(declaration);

            return list;
        }

        public Call ParseCall() {
            Skip(Tokens.GroupOpen);

            // parse arguments
            var arguments = new List<Expression>();
            while (_tokens[_index].Type == Token.Group.String || _tokens[_index].Content != Tokens.GroupClose) {
                if (_tokens[_index].Type == Token.Group.String || _tokens[_index].Content != Tokens.Sequence) {
                    var expression = ParseExpression();
                    if(expression == null) throw CreateError($"Did not expect token '{_tokens[_index].Content}' while parsing call");
                    arguments.Add(expression);
                    continue;
                }

                _index++;
            }

            Skip(Tokens.GroupClose);

            return new Call() { Right = new ArgumentList() { Arguments = arguments } };
        }

        public Throw ParseThrow() {
            Skip(Tokens.Throw);

            return new Throw() { Expression = ParseExpression() };
        }

        public Try ParseTry() {
            Skip(Tokens.Try);

            var result = new Try();
            result.TryBody = ParseBlock();

            if (Peek() == Tokens.Catch) {
                Skip(Tokens.Catch);

                if (Peek() == Tokens.GroupOpen) {
                    Skip(Tokens.GroupOpen);
                    result.CatchVariable = new Variable(Next("catch variable"));
                    Skip(Tokens.GroupClose);
                }

                result.CatchBody = ParseBlock();
            }

            if (Peek() == Tokens.Finally) {
                Skip(Tokens.Finally);

                result.FinallyBody = ParseBlock();
            }

            return result;
        }

        public Conditional ParseConditional() {
            Skip(Tokens.Conditional);

            var arguments = new List<Expression>();
            arguments.Add(ParseExpression());
            Skip(Tokens.ConditionalSeperator);
            arguments.Add(ParseExpression());

            return new Conditional() { Right = new ArgumentList() { Arguments = arguments } };
        }

        public FunctionBlueprint ParseArrowFunction(ParameterList parameters) {
            Skip(Tokens.ArrowFunction);

            Block body;

            if (Peek() == Tokens.BlockOpen) {
                body = ParseBlock();
            } else {
                body = new Block();
                body.Nodes.Add(new Return() { Expression = ParseExpression() });
            }

            return new FunctionBlueprint("", "", parameters, body);
        }
    }
}
