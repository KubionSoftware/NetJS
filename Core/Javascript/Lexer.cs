using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NetJS.Core.Javascript {

    public class Tokens {
        public const string NewLine = "\n";
        public const string Tab = "\t";

        public const string BlockOpen = "{";
        public const string BlockClose = "}";
        public const string ArrayOpen = "[";
        public const string ArrayClose = "]";
        public const string GroupOpen = "(";
        public const string GroupClose = ")";
        public const string RegexOpen = "/";
        public const string RegexClose = "/";

        public const string ExpressionEnd = ";";
        public const string Sequence = ",";

        public const string SingleComment = "//";
        public const string MultiCommentOpen = "/*";
        public const string MultiCommentClose = "*/";

        public const string KeyValueSeperator = ":";
        public const string TypeSeperator = ":";

        public const string Variable = "var";
        public const string Function = "function";
        public const string ArrowFunction = "=>";
        public const string Return = "return";
        public const string If = "if";
        public const string Else = "else";
        public const string Switch = "switch";
        public const string Case = "case";
        public const string CaseSeperator = ":";
        public const string Default = "default";
        public const string For = "for";
        public const string ForIn = "in";
        public const string ForOf = "of";
        public const string While = "while";
        public const string Do = "do";
        public const string Break = "break";
        public const string Continue = "continue";
        public const string Throw = "throw";
        public const string Try = "try";
        public const string Catch = "catch";
        public const string Finally = "finally";

        public const string True = "true";
        public const string False = "false";
        public const string Null = "null";
        public const string Undefined = "undefined";
        public const string NotANumber = "NaN";
        public const string Infinity = "Infinity";
        public const string New = "new";

        public const string Add = "+";
        public const string Substract = "-";
        public const string Multiply = "*";
        public const string Divide = "/";
        public const string Remainder = "%";
        public const string Assign = "=";
        public const string Exponent = "e";

        public const string Increment = "++";
        public const string Decrement = "--";

        public new const string Equals = "==";
        public const string NotEquals = "!=";
        public const string StrictEquals = "===";
        public const string StrictNotEquals = "!==";

        public const string GreaterThan = ">";
        public const string GreaterThanEquals = ">=";
        public const string LessThan = "<";
        public const string LessThanEquals = "<=";

        public const string LogicalAnd = "&&";
        public const string LogicalOr = "||";
        public const string LogicalNot = "!";

        public const string TypeOf = "typeof";
        public const string InstanceOf = "instanceof";
        public const string Void = "void";
        public const string Delete = "delete";

        public const string BitwiseAnd = "&";
        public const string BitwiseOr = "|";
        public const string BitwiseXor = "^";
        public const string BitwiseNot = "~";
        public const string LeftShift = "<<";
        public const string RightShift = ">>";

        public const string Conditional = "?";
        public const string ConditionalSeperator = ":";

        public const string In = "in";
        public const string Access = ".";

        public const char RegexGlobal = 'g';
        public const char RegexIgnoreCase = 'i';
        public const char RegexMultiLine = 'm';
        public const char RegexUnicode = 'u';
        public const char RegexSticky = 'y';

        public static bool IsValidRegexFlag(char c) {
            return c == RegexGlobal || c == RegexIgnoreCase || c == RegexMultiLine || c == RegexUnicode || c == RegexSticky;
        }

        public static bool IsValidOperator(string op) {
            var validOperators = new [] {
                Add, Substract, Multiply, Divide, Remainder,
                LogicalNot, LogicalAnd, LogicalOr,
                BitwiseAnd, BitwiseOr, BitwiseNot, BitwiseXor,
                Equals, NotEquals, StrictEquals, StrictNotEquals,
                GreaterThan, GreaterThanEquals, LessThan, LessThanEquals,
                LeftShift, RightShift,
                Conditional, ConditionalSeperator,
                Assign,
                Add + Assign, Substract + Assign, Divide + Assign, Multiply + Assign, Remainder + Assign,
                LeftShift + Assign, RightShift + Assign,
                BitwiseAnd + Assign, BitwiseOr + Assign, BitwiseXor + Assign,
                Access,
                ArrowFunction,
                Increment, Decrement, Sequence
            };
            return validOperators.Contains(op);
        }

        public static bool IsValidName(string name) {
            return Regex.IsMatch(name, "^[a-zA-Z_$][0-9a-zA-Z_$]*$");
        }
    }

    public class Token {
        public enum Group {
            Structure,
            Operator,
            Text,
            Html,
            String,
            Template,
            Number,
            WhiteSpace,
            Comment
        }

        public string Content;
        public Group Type;

        public int Line;
        public int Character;

        public Token(string content, Group type, int line, int character) {
            Content = content;
            Type = type;
            Line = line;
            Character = character;
        }

        public override string ToString() {
            return Content;
        }
    }

    public class Lexer {

        enum State {
            None,
            String,
            Number
        }

        public static IList<Token> Lex(string code, int fileId) {
            var tokens = new List<Token>();

            var index = 0;
            var buffer = "";
            var inString = false;
            var stringStart = '\0';
            var escaping = false;
            var inOperator = false;
            var inNumber = false;
            var inComment = false;
            var commentStart = "";

            var line = 1;
            var character = 1;

            Error createError(string message) {
                var error = new SyntaxError(message);
                error.AddStackTrace(new Debug.Location(fileId, line));
                return error;
            }

            void clearBuffer(Token.Group type, bool force = false) {
                if (buffer.Length > 0 || force) {
                    if (type == Token.Group.Operator && !Tokens.IsValidOperator(buffer)) {
                        for(var start = 0; start < buffer.Length;) {
                            var found = false;

                            for(var end = buffer.Length; end > start; end--) {
                                var sub = buffer.Substring(start, end - start);
                                if (Tokens.IsValidOperator(sub)) {
                                    tokens.Add(new Token(sub, type, line, character));
                                    start = end;
                                    found = true;
                                    break;
                                }
                            }

                            if (!found) throw createError($"Invalid operator '{buffer}'");
                        }
                    } else {
                        tokens.Add(new Token(buffer, type, line, character));
                    }

                    buffer = "";
                }
            }

            void incrementPosition(char c) {
                if (c == '\n') {
                    line++;
                    character = 1;
                } else {
                    character++;
                }
            }

            while(index < code.Length) {
                var c = code[index];
                incrementPosition(c);

                if (inOperator) {
                    if (IsOperator(c)) {
                        buffer += c;
                        index++;
                        continue;
                    } else {
                        inOperator = false;
                        clearBuffer(Token.Group.Operator);
                    }
                }

                if (inNumber) {
                    if((c >= '0' && c <= '9') || c == '.' || c == 'e' || c == 'E' || c == '-') {
                        buffer += c;
                        index++;
                        continue;
                    } else {
                        inNumber = false;
                        clearBuffer(Token.Group.Number);
                    }
                }

                if (inComment) {
                    if (c == '\n' && commentStart == "//") {
                        clearBuffer(Token.Group.Comment);
                        inComment = false;
                    } else if (commentStart == "/*" && c == '*' && index + 1 < code.Length && code[index + 1] == '/') {
                        buffer += "*/";
                        clearBuffer(Token.Group.Comment);
                        index += 2;
                        inComment = false;
                        continue;
                    } else {
                        buffer += c;
                        index++;
                        continue;
                    }
                }

                if (inString) {
                    if (escaping) {
                        if (c == '\'' || c == '"' || c == '\\') {
                            buffer += c;
                        } else if (c == 'n') {
                            buffer += '\n';
                        } else if (c == 'b') {
                            buffer += '\b';
                        } else if (c == 'r') {
                            buffer += '\r';
                        } else if (c == 'f') {
                            buffer += '\f';
                        } else if (c == 't') {
                            buffer += '\t';
                        } else if (c == 'v') {
                            buffer += '\v';
                        } else {
                            throw createError("Invalid escaped character '" + c + "'");
                        }

                        escaping = false;
                    } else {
                        if (c == stringStart) {
                            inString = false;
                            clearBuffer(stringStart == '`' ? Token.Group.Template : Token.Group.String, true);
                        } else if (c == '\\') {
                            escaping = true;
                        } else if (c == '\r') {
                            // Cariage returns are ignored in order to normalize newlines
                        } else if (c == '\n' && stringStart != '`') {
                            throw createError("Newline in string literal");
                        } else {
                            buffer += c;
                        }
                    }
                } else {
                    if (IsWhitespace(c)) {
                        clearBuffer(Token.Group.Text);
                        if (c != '\r') tokens.Add(new Token(c.ToString(), Token.Group.WhiteSpace, line, character));
                    } else if (IsStructure(c)) {
                        clearBuffer(Token.Group.Text);
                        tokens.Add(new Token(c.ToString(), Token.Group.Structure, line, character));
                    } else if (c == '/' && index + 1 < code.Length && (code[index + 1] == '/' || code[index + 1] == '*')) {
                        clearBuffer(Token.Group.Text);

                        inComment = true;
                        commentStart = c.ToString() + code[index + 1];
                        buffer = commentStart;
                        index++;
                    } else if (IsOperator(c)) {
                        clearBuffer(Token.Group.Text);

                        if (c == '.') {
                            var next = index + 1 < code.Length ? code[index + 1] : '\0';
                            if (next >= '0' && next <= '9') {
                                inNumber = true;
                            }
                        }

                        if (!inNumber) {
                            inOperator = true;
                        }

                        buffer += c;
                    } else if (IsString(c)) {
                        inString = true;
                        stringStart = c;
                    } else if (c >= '0' && c <= '9' && buffer.Length == 0) {
                        inNumber = true;
                        buffer += c;
                    } else {
                        buffer += c;
                    }
                }
                
                index++;
            }

            if (inOperator) {
                clearBuffer(Token.Group.Operator);
            } else if (inNumber) {
                clearBuffer(Token.Group.Number);
            } else if (inString) {
                throw new SyntaxError("Unterminated string literal");
            } else {
                clearBuffer(Token.Group.Text);
            }

            return tokens;
        }

        private static bool IsWhitespace(char c) {
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }

        private static bool IsStructure(char c) {
            return
                c == '{' || c == '}' ||
                c == '(' || c == ')' ||
                c == '[' || c == ']' ||
                c == ';' || c == ',' || 
                c == '?' || c == ':';
        }

        private static bool IsOperator(char c) {
            return
                c == '=' || c == '+' || c == '-' ||
                c == '<' || c == '>' || c == '/' ||
                c == '*' || c == '&' || c == '|' ||
                c == '!' || c == '%' || c == '.' ||
                c == '^' || c == '~';
        }

        private static bool IsString(char c) {
            return c == '\'' || c == '"' || c == '`';
        }

        private static int CountFromEnd(string s, char c) {
            var count = 0;
            var index = s.Length - 1;

            while(index >= 0 && s[index] == c) {
                count++;
                index--;
            }

            return count;
        }
    }
}
