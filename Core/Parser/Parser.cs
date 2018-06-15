using System;
using System.Collections.Generic;
using System.Globalization;

namespace NetJS.Core {

    public class Parser {

        private const int GroupPrecedence = 20;

        private IList<Token> _tokens;
        private int _index = 0;

        private Dictionary<string, Func<Statement>> _statements;
        private Dictionary<string, Func<Expression, Expression>> _expressions;
        private Dictionary<string, Func<Expression, Expression>> _parseExpressions;
        private HashSet<string> _stopTokens;
        
        private int _fileId;

        public Parser(int fileId, IList<Token> tokens) {
            _tokens = tokens;
            
            _fileId = fileId;

            _statements = new Dictionary<string, Func<Statement>>() {
                { Tokens.Var, ParseDeclaration },
                { Tokens.Let, ParseDeclaration },
                { Tokens.Const, ParseDeclaration },
                { Tokens.Function, ParseFunctionDeclaration },
                { Tokens.Class, ParseClassDeclaration },
                { Tokens.Interface, ParseInterfaceDeclaration },
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
                { Tokens.True, (l) => new BooleanLiteral(true) },
                { Tokens.False, (l) => new BooleanLiteral(false) },
                { Tokens.Null, (l) => new NullLiteral() },
                { Tokens.Undefined, (l) => new UndefinedLiteral() },
                { Tokens.This, (l) => new This() },
                { Tokens.NotANumber, (l) => new NaNLiteral() },
                { Tokens.Infinity, (l) => new InfinityLiteral() },
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
                { Tokens.Add + Tokens.Assign, (l) => new AssignmentOperator(new Addition()) },
                { Tokens.Substract + Tokens.Assign, (l) => new AssignmentOperator(new Substraction()) },
                { Tokens.Multiply + Tokens.Assign, (l) => new AssignmentOperator(new Multiplication()) },
                { Tokens.Divide + Tokens.Assign, (l) => new AssignmentOperator(new Division()) },
                { Tokens.Remainder + Tokens.Assign, (l) => new AssignmentOperator(new Remainder()) },
                { Tokens.LeftShift + Tokens.Assign, (l) => new AssignmentOperator(new LeftShift()) },
                { Tokens.RightShift + Tokens.Assign, (l) => new AssignmentOperator(new RightShift()) },
                { Tokens.BitwiseAnd + Tokens.Assign, (l) => new AssignmentOperator(new BitwiseAnd()) },
                { Tokens.BitwiseOr + Tokens.Assign, (l) => new AssignmentOperator(new BitwiseOr()) },
                { Tokens.BitwiseXor + Tokens.Assign, (l) => new AssignmentOperator(new BitwiseXor()) },
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
                { Tokens.Function, (l) => ParseFunctionExpression() },
                { Tokens.BlockOpen, (l) => ParseObject() },
                { Tokens.ArrayOpen, (l) => {
                    return IsVariable(LastExpression(l)) ? (Expression)ParseArrayAccess() : ParseArray();
                } },
                { Tokens.Conditional, (l) => ParseConditional() },
                { Tokens.New, (l) => ParseNew() }
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

            var result = new Block(ParseStatements());
            Optimizer.Optimize(result);
            return result;
        }

        public Error CreateError(string s) {
            var error = new SyntaxError(s);

#if debug_enabled
            error.AddStackTrace(GetLocation(_index));
#endif

            return error;
        }

        public StatementList ParseStatements(int maxStatements = -1) {
            var list = new List<Statement>();

            while (_index < _tokens.Count) {
                var token = _tokens[_index];

                if (token.Type == Token.Group.WhiteSpace || token.Is(Tokens.ExpressionEnd)) {
                    // Skip whitespace and semicolons
                    _index++;
                } else if (token.Type == Token.Group.Comment) {
                    // Skip comments
                    _index++;
                } else if (token.Is(Tokens.BlockClose) || token.Is(Tokens.Case) || token.Is(Tokens.Default)) {
                    // Break when end of block found or start of new switch case/default
                    break;
                } else if (_statements.ContainsKey(token.Content)) {
                    // Parse statement
                    var startIndex = _index;
                    var node = _statements[token.Content]();

#if debug_enabled
                    node.RegisterDebug(GetLocation(startIndex));
#endif

                    list.Add(node);
                } else {
                    // Try to parse as an expression
                    var startIndex = _index;
                    var expression = ParseExpression();
                    if (expression == null) {
                        if (_index == startIndex) {
                            // The parse result is null and the index didn't advance
                            throw CreateError($"Did not expect token '{token.Content}' while parsing statement");
                        }
                    } else {
                        list.Add(new ExpressionStatement(expression));
                    }
                }

                if(maxStatements != -1 && list.Count >= maxStatements) {
                    // Break if the maximum number of statements is reached
                    break;
                }
            }

            return new StatementList(list);
        }

        // Advances the index and returns the next token that is not a whitespace or comment
        public Token Next(string context) {
            while (_index < _tokens.Count) {
                var token = _tokens[_index];
                _index++;
                if (token.Type != Token.Group.WhiteSpace && token.Type != Token.Group.Comment) return token;
            }

            throw CreateError($"No '{context}' token found");
        }

        // Returns the next token that is not a whitespace or comment, without advancing the index
        public Token Peek(int offset = 0) {
            var index = _index;

            while (index < _tokens.Count) {
                var token = _tokens[index];
                if (token.Type != Token.Group.WhiteSpace && token.Type != Token.Group.Comment) {
                    if (offset == 0) return token;
                    offset--;
                }
                index++;
            }

            return new Token("", Token.Group.None, -1, -1);
        }

        // Skips the next token that is equal to the parameter skipToken
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

        // Skips the next token where the type is equal to the parameter type
        public void Skip(Token.Group type) {
            while (_index < _tokens.Count) {
                var token = _tokens[_index];

                if (token.Type != Token.Group.WhiteSpace && token.Type != Token.Group.Comment) {
                    if (token.Type == type) {
                        _index++;
                        return;
                    } else {
                        throw CreateError("Expected token with type '" + type + "' but found '" + token + "'");
                    }
                }
                _index++;
            }
            throw CreateError("Expected token with type '" + type + "'");
        }

        // Parses a typescript type
        public Type ParseType() {
            Skip(Tokens.TypeSeperator);
            var type = Next("type name").Content;

            if (Peek().Is(Tokens.ArrayOpen) && Peek(1).Is(Tokens.ArrayClose)) {
                Skip(Tokens.ArrayOpen);
                Skip(Tokens.ArrayClose);
                type += Tokens.ArrayOpen + Tokens.ArrayClose;
            }

            return ParseType(type);
        }

        // Parses a variable declaration
        public VariableDeclaration ParseDeclaration() {
            var token = Next("parse declaration");
            if (!(token.Is(Tokens.Var) || token.Is(Tokens.Let) || token.Is(Tokens.Const))) {
                throw CreateError($"Invalid declaration token '{token}'");
            }

            var lex = token.Is(Tokens.Var) ? DeclarationScope.Function : DeclarationScope.Block;
            var constant = token.Is(Tokens.Const);

            var result = new VariableDeclaration(lex, constant);

            while (true) {
                var name = Next("variable name").Content;
                Type type;

                if(Peek().Is(Tokens.TypeSeperator)) {
                    type = ParseType();
                } else {
                    type = new AnyType();
                }

                Expression expression = null;
                if (Peek().Is(Tokens.Assign)) {
                    Skip(Tokens.Assign);
                    expression = ParseExpression();

                    if(expression is FunctionLiteral function) {
                        function.Name = name;
                    }
                }

                result.Declarations.Add(new VariableDeclaration.DeclarationVariable(name, type, expression));

                if (!Peek().Is(Tokens.Sequence)) {
                    break;
                }
                Skip(Tokens.Sequence);
            }

            return result;
        }

        public Return ParseReturn() {
            Skip(Tokens.Return);

            if (Peek().Is(Tokens.ExpressionEnd)) {
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
            return expression is Identifier || expression is Access || expression is New || expression is FunctionLiteral || expression is ArgumentList;
        }

        public void CombineExpression(ref Expression left, Expression expression) {
            if (left == null) {
                left = expression;
            } else if (expression == null) {
                throw CreateError("Expression is null");
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
            } else {
                throw CreateError($"Could not combine expression {left.ToDebugString()} with {expression.ToDebugString()}");
            }

#if debug_enabled
            expression.RegisterDebug(GetLocation(_index));
#endif
        }

        public Expression ParseExpression() {
            var parts = new List<string>();
            Expression left = null;
            var previousNewLine = false;

            while (_index < _tokens.Count) {
                var token = _tokens[_index];

                if (token.Type != Token.Group.String && (_stopTokens.Contains(token.Content) || token.Type == Token.Group.Html)) break;
                if (token.Type == Token.Group.ExpressionEnd) break;
                
                var startIndex = _index;

                try {
                    if (token.Type == Token.Group.WhiteSpace) {
                        if (token.Content.Contains("\n")) previousNewLine = true;

                        _index++;
                        continue;
                    } else if (token.Type == Token.Group.Comment) {
                        _index++;
                        continue;
                    } else if (token.Type == Token.Group.String) {
                        CombineExpression(ref left, new StringLiteral(token.Content));
                        _index++;
                    } else if (token.Type == Token.Group.Template) {
                        CombineExpression(ref left, ParseTemplate());
                    } else if (token.Type == Token.Group.Number) {
                        CombineExpression(ref left, new NumberLiteral(Double.Parse(token.Content, CultureInfo.InvariantCulture)));
                        _index++;
                    } else if (_expressions.ContainsKey(token.Content)) {
                        CombineExpression(ref left, _expressions[token.Content](left));
                        _index++;
                    } else if (_parseExpressions.ContainsKey(token.Content)) {
                        CombineExpression(ref left, _parseExpressions[token.Content](left));
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
                        CombineExpression(ref left, ParseIdentifier(token.Content));
                    }
                    
                    previousNewLine = false;
                } catch (Exception e) {
                    if (previousNewLine) {
                        _index = startIndex;
                        break;
                    } else {
                        throw;
                    }
                }
            }

            // Walk up the tree to return highest expression
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

        public Expression ParseIdentifier(string content) {
            _index++;

            if (Peek().Is(Tokens.ArrowFunction)) {
                var parameters = new ParameterList();
                parameters.Parameters.Add(new Parameter(content, new AnyType()));
                return ParseArrowFunction(parameters);
            } else {
                if (content.StartsWith("//")) {
                    Console.Write("WRONG");
                }
                return new Identifier(content);
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
                if (Peek(2).Is(Tokens.Sequence)) {
                    var parameters = ParseParameters();
                    return ParseArrowFunction(parameters);
                } else {
                    Skip(Tokens.GroupOpen);
                    var expression = ParseExpression();
                    Skip(Tokens.GroupClose);

                    if (expression == null && Peek().Is(Tokens.ArrowFunction)) {
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

            var flags = valid ? Next("regex flags").Content : "";

            return new New() {
                NewExpression = new Identifier("RegExp"),
                Arguments = new ArgumentList(
                    new StringLiteral(buffer),
                    new StringLiteral(flags)
                )
            };
        }

        public NumberLiteral ParseNumber() {
            var token = _tokens[_index];
            var s = token.Content;
            
            if(token.Content == Tokens.Access) {
                Skip(Tokens.Access);
                s += Next("number");
            }

            var peek = Peek();
            if(peek.Content.ToLower() == Tokens.Exponent) {
                Skip(peek.Content);
                s += Tokens.Exponent;
                s += Next("number exponent");
            }

            double value;
            if(double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out value)) {
                return new NumberLiteral(value);
            } else {
                throw CreateError("Invalid number '" + s + "'");
            }
        }

        public File ParseFile() {
            var expressions = ParseTemplateExpressions();
            return new File(expressions);
        }

        public Expression ParseTemplate() {
            var expressions = ParseTemplateExpressions();
            if (expressions.Count == 0) return new StringLiteral("");

            Expression result = expressions[0];
            for (var i = 1; i < expressions.Count; i++) {
                if (result is StringLiteral rs && expressions[i] is StringLiteral es) {
                    result = new StringLiteral(rs.Value + es.Value);
                } else {
                    result = new Addition() { Left = result, Right = expressions[i] };
                }
            }

            if (result is Operator op) op.Precedence = GroupPrecedence;
            return result;
        }

        public List<Expression> ParseTemplateExpressions() {
            var parts = new List<Expression>();

            Skip(Token.Group.Template);

            while (true) {
                var next = Next("template string");

                if(next.Type == Token.Group.Template) {
                    break;
                } else if(next.Type == Token.Group.ExpressionStart) {
                    parts.Add(ParseExpression());

                    Skip(Token.Group.ExpressionEnd);
                } else if(next.Type == Token.Group.String) {
                    parts.Add(new StringLiteral(next.Content));
                }
            }

            return parts;
        }

        public While ParseWhile() {
            Skip(Tokens.While);

            Skip(Tokens.GroupOpen);
            var check = ParseExpression();
            Skip(Tokens.GroupClose);

            var body = ParseBlockOrStatement();

            return new While() { Check = check, Body = body };
        }

        public DoWhile ParseDoWhile() {
            Skip(Tokens.Do);

            var body = ParseBlockOrStatement();

            Skip(Tokens.While);

            Skip(Tokens.GroupOpen);
            var check = ParseExpression();
            Skip(Tokens.GroupClose);

            return new DoWhile() { Check = check, Body = body };
        }

        public Statement ParseBlockOrStatement() {
            if (Peek().Is(Tokens.BlockOpen)) {
                return ParseBlock();
            } else {
                var statementList = ParseStatements(1);
                if (Peek().Is(Tokens.ExpressionEnd)) {
                    Skip(Tokens.ExpressionEnd);
                }
                return statementList.List[0];
            }
        }

        public Block ParseBlock() {
            Skip(Tokens.BlockOpen);
            var statementList = ParseStatements();
            Skip(Tokens.BlockClose);

            return new Block(statementList);
        }

        public Statement ParseFor() {
            Skip(Tokens.For);
            Skip(Tokens.GroupOpen);

            var declaration = ParseDeclaration();

            var peek = Peek();
            if (peek.Is(Tokens.ExpressionEnd)) {
                Skip(Tokens.ExpressionEnd);
                var check = ParseExpression();
                Skip(Tokens.ExpressionEnd);
                var action = ParseExpression();
                Skip(Tokens.GroupClose);

                return new For() { First = declaration, Second = check, Third = action, Stmt = ParseBlockOrStatement() };
            } else if (peek.Is(Tokens.ForIn)) {
                Skip(Tokens.ForIn);
                var collection = ParseExpression();
                Skip(Tokens.GroupClose);

                return new ForInOf() { Kind = IterationKind.Enumerate, Declaration = declaration, Collection = collection, Body = ParseBlockOrStatement() };
            } else if (peek.Is(Tokens.ForOf)) {
                Skip(Tokens.ForOf);
                var collection = ParseExpression();
                Skip(Tokens.GroupClose);

                return new ForInOf() { Kind = IterationKind.Iterate, Declaration = declaration, Collection = collection, Body = ParseBlockOrStatement() };
            }

            throw CreateError("Incorrect for loop");
        }

        public If ParseIf() {
            Skip(Tokens.If);
            Skip(Tokens.GroupOpen);

            var result = new If();

            result.Test = ParseExpression();

            Skip(Tokens.GroupClose);

            result.TrueStmt = ParseBlockOrStatement();

            if (Peek().Is(Tokens.Else)) {
                Skip(Tokens.Else);
                result.FalseStmt = ParseBlockOrStatement();
            }

            return result;
        }

        public Switch ParseSwitch() {
            Skip(Tokens.Switch);

            Skip(Tokens.GroupOpen);
            var test = ParseExpression();
            Skip(Tokens.GroupClose);

            Skip(Tokens.BlockOpen);

            var clauses = new List<CaseClause>();

            while (Peek().Is(Tokens.Case)) {
                Skip(Tokens.Case);
                var value = ParseExpression();

                Skip(Tokens.CaseSeperator);

                var body = ParseStatements();

                clauses.Add(new CaseClause() { Expression = value, StatementList = body });
            }

            if (Peek().Is(Tokens.Default)) {
                Skip(Tokens.Default);
                Skip(Tokens.CaseSeperator);

                var body = ParseStatements();

                clauses.Add(new CaseClause() { IsDefault = true, StatementList = body });
            }

            Skip(Tokens.BlockClose);

            return new Switch() { Test = test, Block = new CaseBlock() { Clauses = clauses } };
        }

        public Access ParseArrayAccess() {
            Skip(Tokens.ArrayOpen);

            var value = ParseExpression();

            Skip(Tokens.ArrayClose);

            return new Access(false) { Right = value };
        }

        public ArrayLiteral ParseArray() {
            var list = new List<Expression>();

            Skip(Tokens.ArrayOpen);
            
            while (!Peek().Is(Tokens.ArrayClose)) {
                var value = ParseExpression();
                list.Add(value);

                var next = Peek();

                if (next.Is(Tokens.ArrayClose)) {
                    break;
                } else if (next.Is(Tokens.Sequence)) {
                    Skip(Tokens.Sequence);
                } else {
                    throw CreateError("Invalid array");
                }
            }

            Skip(Tokens.ArrayClose);

            return new ArrayLiteral(list);
        }

        public ObjectLiteral ParseObject() {
            var blueprints = new Dictionary<string, Expression>();

            Skip(Tokens.BlockOpen);

            while (!Peek().Is(Tokens.BlockClose)) {
                var key = Next("object key");

                Skip(Tokens.KeyValueSeperator);

                var value = ParseExpression();

                blueprints[key.Content] = value;

                var peek = Peek();
                if (!peek.Is(Tokens.Sequence) && !peek.Is(Tokens.BlockClose)) {
                    throw CreateError("Invalid object");
                }

                if (peek.Is(Tokens.BlockClose)) {
                    break;
                }

                Skip(Tokens.Sequence);
            }

            Skip(Tokens.BlockClose);

            return new ObjectLiteral(blueprints);
        }

        public ParameterList ParseParameters() {
            var list = new ParameterList();

            Skip(Tokens.GroupOpen);

            if (!Peek().Is(Tokens.GroupClose)) {
                while (_index < _tokens.Count) {
                    var name = Next("parameter name");
                    
                    if (!Tokens.IsValidName(name.Content)) {
                        throw CreateError($"Invalid parameter name '{name.Content}'");
                    }

                    var peek = Peek();

                    if (peek.Is(Tokens.TypeSeperator)) {
                        var type = ParseType();
                        peek = Peek();

                        list.Parameters.Add(new Parameter(name.Content, type));
                    } else {
                        list.Parameters.Add(new Parameter(name.Content, new AnyType()));
                    }

                    if (peek.Is(Tokens.GroupClose)) {
                        break;
                    } else if (peek.Is(Tokens.Sequence)) {
                        Skip(Tokens.Sequence);
                        continue;
                    }

                    throw CreateError("Unclosed parameter list");
                }
            }

            Skip(Tokens.GroupClose);

            return list;
        }

        public FunctionLiteral ParseFunction(string name) {
            var parameters = ParseParameters();

            Type type = null;
            if (Peek().Is(Tokens.TypeSeperator)) {
                type = ParseType();
            }

            var body = ParseBlockOrStatement();

            var function = new FunctionLiteral(name, FunctionKind.Normal, type, parameters, body);
            return function;
        }

        public Expression ParseFunctionExpression() {
            Skip(Tokens.Function);

            return ParseFunction("");
        }

        public Statement ParseFunctionDeclaration() {
            Skip(Tokens.Function);

            var name = Next("function name");
            var function = ParseFunction(name.Content);
            
            var declaration = new VariableDeclaration(DeclarationScope.Global, false);
            declaration.Declarations.Add(new VariableDeclaration.DeclarationVariable(name.Content, new AnyType(), function));

            return declaration;
        }

        public Statement ParseClassDeclaration() {
            Skip(Tokens.Class);

            var className = Next("class name");
            var classBlueprint = new ClassLiteral();

            if (Peek().Is(Tokens.Extends)) {
                Skip(Tokens.Extends);
                var prototype = Next("class extends");
                classBlueprint.Prototype = prototype.Content;
            }

            Skip(Tokens.BlockOpen);

            while (!Peek().Is(Tokens.BlockClose)) {
                var functionName = Next("class member name");
                var isStatic = functionName.Is(Tokens.Static);
                if (isStatic) functionName = Next("class member name");

                var function = ParseFunction(functionName.Content);

                if(functionName.Is(Tokens.Constructor)) {
                    if (classBlueprint.Constructor != null) throw CreateError("A class can only have one constructor");
                    classBlueprint.Constructor = function;
                } else if (isStatic) {
                    classBlueprint.StaticMethods.Add(function);
                } else {
                    classBlueprint.PrototypeMethods.Add(function);
                }
            }

            Skip(Tokens.BlockClose);

            var declaration = new VariableDeclaration(DeclarationScope.Global, false);
            declaration.Declarations.Add(new VariableDeclaration.DeclarationVariable(className.Content, new AnyType(), classBlueprint));

            return declaration;
        }

        public ArgumentList ParseArguments() {
            Skip(Tokens.GroupOpen);

            // parse arguments
            var arguments = new List<Expression>();
            while (_tokens[_index].Type == Token.Group.String || _tokens[_index].Content != Tokens.GroupClose) {
                if (_tokens[_index].Type == Token.Group.String || _tokens[_index].Content != Tokens.Sequence) {
                    var expression = ParseExpression();
                    if (expression == null) throw CreateError($"Did not expect token '{_tokens[_index].Content}' while parsing call");
                    arguments.Add(expression);
                    continue;
                }

                _index++;
            }

            Skip(Tokens.GroupClose);

            return new ArgumentList(arguments.ToArray());
        }

        public Call ParseCall() {
            var arguments = ParseArguments();
            return new Call() { Right = arguments };
        }

        public Throw ParseThrow() {
            Skip(Tokens.Throw);

            return new Throw() { Expression = ParseExpression() };
        }

        public Try ParseTry() {
            Skip(Tokens.Try);

            var result = new Try();
            result.TryBody = ParseBlock();

            if (Peek().Is(Tokens.Catch)) {
                Skip(Tokens.Catch);

                if (Peek().Is(Tokens.GroupOpen)) {
                    Skip(Tokens.GroupOpen);
                    result.CatchVariable = new String(Next("catch variable").Content);
                    Skip(Tokens.GroupClose);
                }

                result.CatchBody = ParseBlock();
            }

            if (Peek().Is(Tokens.Finally)) {
                Skip(Tokens.Finally);

                result.FinallyBody = ParseBlock();
            }

            return result;
        }

        public Conditional ParseConditional() {
            Skip(Tokens.Conditional);
            
            var trueExpression = ParseExpression();
            Skip(Tokens.ConditionalSeperator);
            var falseExpression = ParseExpression();

            return new Conditional() { TrueExpression = trueExpression, FalseExpression = falseExpression };
        }

        public New ParseNew() {
            Skip(Tokens.New);

            var identifier = new Identifier(Next("new identifier").Content);
            var arguments = ParseArguments();

            return new New() { NewExpression = identifier, Arguments = arguments };
        }

        public FunctionLiteral ParseArrowFunction(ParameterList parameters) {
            Skip(Tokens.ArrowFunction);

            Statement body;

            if (Peek().Is(Tokens.BlockOpen)) {
                body = ParseBlockOrStatement();
            } else {
                body = new Return() { Expression = ParseExpression() };
            }

            return new FunctionLiteral("", FunctionKind.Arrow, null, parameters, body);
        }

        public static Type ParseType(string type) {
            if (type == Tokens.Any) {
                return new AnyType();
            } else if (type == Tokens.Void) {
                return new VoidType();
            } else if (type == Tokens.String) {
                return new StringType();
            } else if (type == Tokens.Number) {
                return new NumberType();
            } else if (type == Tokens.Boolean) {
                return new BooleanType();
            } else if (type == Tokens.Object) {
                return new ObjectType();
            } else if (type.EndsWith(Tokens.Array)) {
                var itemType = type.Replace(Tokens.Array, "");
                return new ArrayType(ParseType(itemType));
            } else {
                return new InstanceType(type);
            }
        }

        public Statement ParseInterfaceDeclaration() {
            Skip(Tokens.Interface);

            var name = Next("interface name");
            var i = new Interface(name.Content);

            Skip(Tokens.BlockOpen);

            while (!Peek().Is(Tokens.BlockClose)) {
                var key = Next("interface key");

                var optional = false;
                if(Peek().Is(Tokens.TypeOptional)) {
                    optional = true;
                    Skip(Tokens.TypeOptional);
                }

                Skip(Tokens.TypeSeperator);

                var type = ParseType(Next("interface type").Content);

                Skip(Tokens.ExpressionEnd);

                i.Add(key.Content, optional, type);
            }

            Skip(Tokens.BlockClose);
            
            var declaration = new VariableDeclaration(DeclarationScope.Global, false);
            declaration.Declarations.Add(new VariableDeclaration.DeclarationVariable(name.Content, new AnyType(), new InterfaceBlueprint(i)));

            return declaration;
        }
    }
}
