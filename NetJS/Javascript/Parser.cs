using System;
using System.Collections.Generic;

namespace NetJS.Javascript {

    public class Parser {

        private const int GroupPrecedence = 20;

        private IList<Token> _tokens;
        private int _index = 0;

        private Dictionary<string, Func<Statement>> _statements;
        private Dictionary<string, Func<Expression, Expression>> _expressions;
        private Dictionary<string, Func<Expression, Expression>> _parseExpressions;
        private Dictionary<string, Func<Operator>> _assignExpressions;
        private HashSet<string> _stopTokens;

        private string _fileName;
        private int _fileId;

        private List<int> _newLineLocations;

        public Parser(string fileName, IList<Token> tokens) {
            _tokens = tokens;

            _fileName = fileName;
            _fileId = Debug.GetFileId(fileName);

            _statements = new Dictionary<string, Func<Statement>>() {
                { Tokens.Variable, ParseDeclaration },
                { Tokens.Function, ParseFunctionDeclaration },
                { Tokens.Return, ParseReturn },
                { Tokens.If, ParseIf },
                { Tokens.Switch, ParseSwitch },
                { Tokens.For, ParseFor },
                { Tokens.While, ParseWhile },
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
                { Tokens.BlockOpen, (l) => Peek(1) == Tokens.BlockOpen ? (Expression)ParseAngular() : ParseObject() },
                { Tokens.ArrayOpen, (l) => {
                    var last = LastExpression(l);
                    return last is Variable || last is Access || last is Call ? (Expression)ParseArrayAccess() : ParseArray();
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

            _newLineLocations = new List<int>();
#if DEBUG
            for(var i = 0; i < _tokens.Count; i++) {
                if(_tokens[i].Content == Tokens.NewLine) {
                    _newLineLocations.Add(i);
                }
            }
#endif
        }

        public Debug.Location GetLocation(int index) {
            var line = 0;
            while(line < _newLineLocations.Count) {
                if (index < _newLineLocations[line]) break;
                line++;
            }

            return new Debug.Location(_fileId, line + 1);
        }

        public Block Parse() {
#if DEBUG
            Debug.RemoveNodes(_fileId);
#endif

            var result = ParseStatements();
            result = Optimizer.Optimize(result);
            return result;
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
                } else if (token.Type != Token.Group.String && (token.Content == Tokens.BlockClose || token.Content == Tokens.Case || token.Content == Tokens.ConditionalSeperator || token.Content == Tokens.Default)) {
                    break;
                } else if (token.Content.StartsWith(Tokens.SingleComment) || token.Content.StartsWith(Tokens.MultiCommentOpen)) {
                    ParseComment();
                } else if (_statements.ContainsKey(token.Content)) {
                    var startIndex = _index;
                    var node = _statements[token.Content]();

#if DEBUG
                    node.RegisterDebug(GetLocation(startIndex));
#endif

                    list.Nodes.Add(node);
                } else {
                    var startIndex = _index;
                    var expression = ParseExpression();
                    if (expression == null) {
                        if (_index == startIndex) {
                            throw new SyntaxError($"Did not expect token '{token.Content}' while parsing statement");
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

        public string Next() {
            while (_index < _tokens.Count) {
                var token = _tokens[_index];
                _index++;
                if (token.Type != Token.Group.WhiteSpace) return token.Content;
            }

            throw new SyntaxError("No next token found");
        }

        public string Peek(int offset = 0) {
            var index = _index;

            while (index < _tokens.Count) {
                var token = _tokens[index];
                if (token.Type != Token.Group.WhiteSpace) {
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

                if (token.Type != Token.Group.WhiteSpace) {
                    if (token.Content == skipToken) {
                        _index++;
                        return;
                    } else {
                        throw new SyntaxError("Expected token '" + skipToken + "' but found '" + token + "'");
                    }
                }
                _index++;
            }
            throw new SyntaxError("Expected token '" + skipToken + "'");
        }

        public string ParseType() {
            Skip(Tokens.TypeSeperator);
            var type = Next();

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
                var name = Next();
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

        public Expression ParseExpression() {
            var parts = new List<string>();
            Expression left = null;

            void found(Expression expression) {
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
                                while(currentLeftOperation.Parent != null && expressionOperation.Precedence <= currentLeftOperation.Parent.Precedence) {
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
                        if (leftOperation.AcceptsRight && !leftOperation.HasRight) {
                            leftOperation.SetRight(expression);
                        } else {
                            // TODO: check if in every circumstance, now just for html + statementlist
                            left = new Addition() { Left = left, Right = expression };
                        }
                    }
                } else if (left is StringBlueprint leftString) {
                    if (expression is StringBlueprint expressionString) {
                        leftString.Combine(expressionString);
                    } else if (expression is Operator op) {
                        if(op.AcceptsLeft && !op.HasLeft) {
                            op.SetLeft(left);
                            left = op;
                        } else {
                            throw new SyntaxError($"Cannot parse string and '{op.GetType()}'");
                        }
                    } else {
                        var addition = new Addition();
                        addition.Left = left;
                        addition.Right = expression;
                        left = addition;
                    }
                } else if (expression is Operator op) {
                    if (op.AcceptsLeft) {
                        op.SetLeft(left);
                        left = op;
                    } else {
                        throw new SyntaxError($"Operator '{op.GetType()}' doesn't accept a left-hand argument");
                    }
                }

#if DEBUG
                expression.RegisterDebug(GetLocation(_index));
#endif
            }

            void foundAssign(Operator op) {
                var l = left;
                found(new Assignment());
                found(l);
                op.Precedence = 2.5f;
                found(op);
            }

            while (_index < _tokens.Count) {
                var token = _tokens[_index];

                if (token.Type != Token.Group.String && (_stopTokens.Contains(token.Content) || token.Type == Token.Group.Html)) break;

                if (token.Type == Token.Group.WhiteSpace) {
                    _index++;
                } else if (token.Type == Token.Group.String) {
                    found(new StringBlueprint(token.Content));
                    _index++;
                } else if (token.Type == Token.Group.Template) {
                    found(ParseTemplate(token.Content));
                    _index++;
                } else if (token.Type == Token.Group.Number) {
                    found(new NumberBlueprint(Double.Parse(token.Content)));
                    _index++;
                } else if (_expressions.ContainsKey(token.Content)) {
                    found(_expressions[token.Content](left));
                    _index++;
                } else if (_parseExpressions.ContainsKey(token.Content)) {
                    found(_parseExpressions[token.Content](left));
                } else if (_assignExpressions.ContainsKey(token.Content)) {
                    foundAssign(_assignExpressions[token.Content]());
                    _index++;
                } else if (token.Content == Tokens.Divide) {
                    if (left == null) {
                        found(ParseRegex());
                        continue;
                    }

                    if (left is Operator) {
                        var op = (Operator)left;
                        if (!op.HasRight) {
                            found(ParseRegex());
                            continue;
                        }
                    }

                    found(new Division());
                    _index++;
                } else if (token.Content == Tokens.Increment) {
                    if (left == null) {
                        found(new PrefixIncrement());
                        _index++;
                        continue;
                    }

                    if (left is Operator) {
                        var op = (Operator)left;
                        if (!op.HasRight) {
                            found(new PrefixIncrement());
                            _index++;
                            continue;
                        }
                    }

                    found(new PostfixIncrement());
                    _index++;
                } else if (token.Content == Tokens.Decrement) {
                    // TODO: double code

                    if (left == null) {
                        found(new PrefixDecrement());
                        _index++;
                        continue;
                    }

                    if (left is Operator) {
                        var op = (Operator)left;
                        if (!op.HasRight) {
                            found(new PrefixDecrement());
                            _index++;
                            continue;
                        }
                    }

                    found(new PostfixDecrement());
                    _index++;
                } else if (token.Content == Tokens.GroupOpen) {
                    var last = LastExpression(left);
                    if (last is Variable || last is Access || last is New) {
                        found(ParseCall());
                    } else {
                        if (Peek(2) == Tokens.Sequence) {
                            var parameters = ParseParameters();
                            found(ParseArrowFunction(parameters));
                        } else {
                            Skip(Tokens.GroupOpen);
                            var expression = ParseExpression();
                            Skip(Tokens.GroupClose);

                            if (expression == null && Peek() == Tokens.ArrowFunction) {
                                found(ParseArrowFunction(new ParameterList()));
                            } else {
                                if (expression is Operator expressionOperation) {
                                    expressionOperation.Precedence = GroupPrecedence;
                                }
                                found(expression);
                            }
                        }
                    }
                } else if (_statements.ContainsKey(token.Content)) {
                    break;
                } else if (token.Content.StartsWith(Tokens.SingleComment) || token.Content.StartsWith(Tokens.MultiCommentOpen)) {
                    ParseComment();
                } else {
                    var variable = new Variable(token.Content);
                    _index++;

                    if (Peek() == Tokens.ArrowFunction) {
                        var parameters = new ParameterList();
                        parameters.Parameters.Add(variable);
                        found(ParseArrowFunction(parameters));
                    } else {
                        found(variable);
                    }
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

            var flags = valid ? Next() : "";

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
                s += Next();
            }

            var peek = Peek();
            if(peek.ToLower() == Tokens.Exponent) {
                Skip(peek);
                s += Tokens.Exponent;
                s += Next();
            }

            double value;
            if(double.TryParse(s, out value)) {
                return new NumberBlueprint(value);
            } else {
                throw new SyntaxError("Invalid number '" + s + "'");
            }
        }

        public StringBlueprint ParseAngular() {
            // TODO: this is very specific to angular tags, maybe remove this at some point

            Skip(Tokens.BlockOpen);
            Skip(Tokens.BlockOpen);

            var template = Tokens.BlockOpen + Tokens.BlockOpen;

            while (!(_tokens[_index].Content == Tokens.BlockClose && _tokens[_index + 1].Content == Tokens.BlockClose)) {
                template += _tokens[_index];
                _index++;
            }

            template += Tokens.BlockClose + Tokens.BlockClose;

            Skip(Tokens.BlockClose);
            Skip(Tokens.BlockClose);

            return new StringBlueprint(template);
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
                                var tokens = Lexer.Lex(buffer);
                                var parser = new Parser("TEMPLATE", tokens);
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

            throw new SyntaxError("Incorrect for loop");
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
                    throw new SyntaxError("Invalid array");
                }
            }

            Skip(Tokens.ArrayClose);

            return new ArrayBlueprint(list);
        }

        public ObjectBlueprint ParseObject() {
            var blueprints = new Dictionary<string, Expression>();

            Skip(Tokens.BlockOpen);

            while (_tokens[_index].Content != Tokens.BlockClose) {
                var key = Next();

                Skip(Tokens.KeyValueSeperator);

                var value = ParseExpression();

                blueprints[key] = value;

                if (_tokens[_index].Content != Tokens.Sequence && _tokens[_index].Content != Tokens.BlockClose) {
                    throw new SyntaxError("Invalid object");
                }

                if (_tokens[_index].Content == Tokens.BlockClose) {
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
                    var name = Next();
                    
                    if (!Tokens.IsValidName(name)) {
                        throw new SyntaxError($"Invalid parameter name '{name}'");
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

                    throw new SyntaxError("Unclosed parameter list");
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

            var name = Next();

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
                    if(expression == null) throw new SyntaxError($"Did not expect token '{_tokens[_index].Content}' while parsing call");
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
                    result.CatchVariable = new Variable(Next());
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

        public void ParseComment() {
            var multiline = _tokens[_index].Content.StartsWith(Tokens.MultiCommentOpen);
            while (_index < _tokens.Count) {
                var token = _tokens[_index];
                _index++;

                if (multiline) {
                    if (token.Content.EndsWith(Tokens.MultiCommentClose)) break;
                } else {
                    if (token.Content == Tokens.NewLine) break;
                }
            }
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
