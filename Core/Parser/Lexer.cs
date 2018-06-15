using System;
using System.Collections.Generic;

namespace NetJS.Core {

    public class Lexer {

        private string Source;
        private int FileId;

        private int Index;

        public int Line = 1;
        public int Character = 1;

        private string Buffer = "";

        private bool InString = false;
        private char StringStart = Chars.Null;
        private bool Escaping = false;

        private bool InOperator = false;
        private bool InNumber = false;

        private bool InComment = false;
        private string CommentStart = "";

        private bool InTemplate = false;

        private bool ShouldStop = false;

        public Lexer(string source, int fileId, int startIndex = 0) {
            Source = source;
            Index = startIndex;
            FileId = fileId;
        }

        private void ClearBuffer(Token.Group type, Func<Token, bool> action, bool force = false) {
            if (Buffer.Length > 0 || force) {
                if (type == Token.Group.Operator && !Tokens.IsValidOperator(Buffer)) {
                    // Try to split into valid operators
                    for (var start = 0; start < Buffer.Length;) {
                        var found = false;

                        for (var end = Buffer.Length; end > start; end--) {
                            var sub = Buffer.Substring(start, end - start);
                            if (Tokens.IsValidOperator(sub)) {
                                ShouldStop = action(new Token(sub, type, Line, Character));
                                start = end;
                                found = true;
                                break;
                            }
                        }

                        if (!found) throw CreateError($"Invalid operator '{Buffer}'");
                    }
                } else {
                    ShouldStop = action(new Token(Buffer, type, Line, Character));
                }

                Buffer = "";
            }
        }

        private Error CreateError(string message) {
            var error = new SyntaxError(message);
            error.AddStackTrace(new Debug.Location(FileId, Line));
            return error;
        }

        private void IncrementPosition(char c) {
            if (c == Chars.NewLine) {
                Line++;
                Character = 1;
            } else {
                Character++;
            }
        }

        private void HandleTemplate(char c, Func<Token, bool> action) {
            if (c == '$' && Index + 1 < Source.Length && Source[Index + 1] == '{') {
                // Start of an expression
                ClearBuffer(Token.Group.String, action);

                ShouldStop = action(new Token("", Token.Group.ExpressionStart, Line, Character));

                // Skip ${
                Index += 2;

                bool closed = false;
                int depth = 1;

                // Create new lexer and set line and character position
                var lexer = new Lexer(Source, FileId, Index);
                lexer.Character = Character;
                lexer.Line = Line;

                // Lex the expression (inside ${...})
                lexer.Walk(token => {
                    if (token.Is(Tokens.BlockOpen)) {
                        depth++;
                    } else if (token.Is(Tokens.BlockClose)) {
                        depth--;

                        if (depth == 0) {
                            // Matching close token of ${...} expression
                            action(new Token("", Token.Group.ExpressionEnd, Line, Character));
                            closed = true;
                            return true;
                        }
                    }

                    action(token);
                    return false;
                });

                // Advance the index to the end of the lexed expression
                Index = lexer.Index;

                // Advance the line and character position
                Character = lexer.Character;
                Line = lexer.Line;

                if (!closed) throw CreateError("Unclosed template expression");
            } else if (c == Chars.Template) {
                // End of template
                ClearBuffer(Token.Group.String, action);
                ShouldStop = action(new Token(c.ToString(), Token.Group.Template, Line, Character));
                InTemplate = false;
                Index++;
            } else if (c == Chars.CarriageReturn) {
                // Carriage returns are ignored in order to normalize newlines
                Index++;
            } else {
                Buffer += c;
                Index++;
            }
        }

        private void HandleString(char c, Func<Token, bool> action) {
            if (Escaping) {
                // Escape character

                if (c == Chars.SingleQuote || c == Chars.DoubleQuote || c == Chars.Backslash) {
                    Buffer += c;
                } else if (c == 'n') {
                    Buffer += Chars.NewLine;
                } else if (c == 'b') {
                    Buffer += '\b';
                } else if (c == 'r') {
                    Buffer += Chars.CarriageReturn;
                } else if (c == 'f') {
                    Buffer += '\f';
                } else if (c == 't') {
                    Buffer += Chars.Tab;
                } else if (c == 'v') {
                    Buffer += '\v';
                } else {
                    throw CreateError("Invalid escaped character '" + c + "'");
                }

                Escaping = false;
            } else {
                if (c == StringStart) {
                    // Matching string closing found
                    InString = false;
                    ClearBuffer(Token.Group.String, action, true);
                } else if (c == Chars.Backslash) {
                    // Start escaping
                    Escaping = true;
                } else if (c == Chars.CarriageReturn) {
                    // Cariage returns are ignored in order to normalize newlines
                } else if (c == Chars.NewLine) {
                    throw CreateError("Newline in string literal");
                } else {
                    Buffer += c;
                }
            }
        }

        public void Walk(Func<Token, bool> action) {
            while (Index < Source.Length && !ShouldStop) {
                var c = Source[Index];
                IncrementPosition(c);

                if (InOperator) {
                    if (Chars.IsOperator(c)) {
                        // Valid operator token
                        Buffer += c;
                        Index++;
                        continue;
                    } else {
                        InOperator = false;
                        ClearBuffer(Token.Group.Operator, action);
                    }
                }

                if (InNumber) {
                    if (Chars.IsNumerical(c) || c == '.' || c == 'e' || c == 'E' || c == '-') {
                        // Valid number token
                        Buffer += c;
                        Index++;
                        continue;
                    } else {
                        InNumber = false;
                        ClearBuffer(Token.Group.Number, action);
                    }
                }

                if (InComment) {
                    if (c == Chars.NewLine && CommentStart == "//") {
                        // End of single-line comment
                        ClearBuffer(Token.Group.Comment, action);
                        InComment = false;
                    } else if (CommentStart == "/*" && c == '*' && Index + 1 < Source.Length && Source[Index + 1] == '/') {
                        // End of multi-line comment
                        Buffer += "*/";
                        ClearBuffer(Token.Group.Comment, action);

                        // Skip */
                        Index += 2;
                        InComment = false;
                        continue;
                    } else {
                        Buffer += c;
                        Index++;
                        continue;
                    }
                }

                if (InTemplate) {
                    HandleTemplate(c, action);
                    continue;
                } else if (InString) {
                    HandleString(c, action);
                } else {
                    var next = Index + 1 < Source.Length ? Source[Index + 1] : Chars.Null;

                    if (Chars.IsWhitespace(c)) {
                        // Whitespace
                        ClearBuffer(Token.Group.Text, action);
                        if (c != Chars.CarriageReturn) ShouldStop = action(new Token(c.ToString(), Token.Group.WhiteSpace, Line, Character));
                    } else if (Chars.IsStructure(c)) {
                        // Structure
                        ClearBuffer(Token.Group.Text, action);
                        ShouldStop = action(new Token(c.ToString(), Token.Group.Structure, Line, Character));
                    } else if (c == '/' && (next == '/' || next == '*')) {
                        // Comment
                        ClearBuffer(Token.Group.Text, action);

                        InComment = true;
                        CommentStart = c.ToString() + Source[Index + 1];
                        Buffer = CommentStart;
                        Index++;
                    } else if ((Chars.IsNumerical(c) || (c == '.' && Chars.IsNumerical(next))) && Buffer.Length == 0) {
                        // Number
                        InNumber = true;
                        Buffer += c;
                    } else if (Chars.IsOperator(c)) {
                        // Operator
                        ClearBuffer(Token.Group.Text, action);
                        InOperator = true;
                        Buffer += c;
                    } else if (Chars.IsString(c)) {
                        // String
                        ClearBuffer(Token.Group.Text, action);
                        InString = true;
                        StringStart = c;
                    } else if (c == Chars.Template) {
                        // Template
                        ClearBuffer(Token.Group.Text, action);
                        InTemplate = true;
                        ShouldStop = action(new Token(c.ToString(), Token.Group.Template, Line, Character));
                    } else {
                        Buffer += c;
                    }
                }

                Index++;
            }

            if (InOperator) {
                ClearBuffer(Token.Group.Operator, action);
            } else if (InNumber) {
                ClearBuffer(Token.Group.Number, action);
            } else if (InComment) {
                ClearBuffer(Token.Group.Comment, action);
            } else if (InString) {
                throw new SyntaxError("Unterminated string literal");
            } else {
                ClearBuffer(Token.Group.Text, action);
            }
        }

        public IList<Token> Lex() {
            var tokens = new List<Token>();

            Walk(token => {
                tokens.Add(token);
                return false;
            });

            return tokens;
        }
    }
}
