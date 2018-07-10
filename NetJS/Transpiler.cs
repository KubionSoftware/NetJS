using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NetJS {

    // Transpiles NetJS templates to JavaScript so it can have simple logic like if, for and expressions
    class Transpiler {

        // Token definitions
        private const string For = "for";
        private const string If = "if";
        private const string ElseIf = "elif";
        private const string Else = "else";
        private const string End = "/";

        private const char TokenStart = '$';

        // Grouping tokens
        private static Dictionary<char, char> Groups = new Dictionary<char, char>() {
            { '{', '}' },
            { '(', ')' }
        };

        // All valid tokens
        private static string[] ValidTokens = new string[] {
            For,
            End + For,
            If,
            ElseIf,
            Else,
            End + If
        };

        // Maximum length before stopping parsing
        private static int MaxTokenLength = 4;

        // Stores a token
        private class Token {
            public int StartIndex;
            public int EndIndex;
            public string Tag;
            public string Expression;
        }

        // Tries to parse a token, returns null if not successful
        private static Token ParseToken(string source, int index) {
            var startIndex = index;

            if (source[index] != TokenStart) return null;
            // Skip start token
            index++;

            // Read the token in a buffer
            var tokenBuffer = "";
            while (index < source.Length) {
                // If a group start token is found, break
                if (Groups.ContainsKey(source[index])) break;

                // Add character to the buffer
                tokenBuffer += source[index];

                // If the buffer is longer than the max token length, the token is not valid, return null
                if (tokenBuffer.Length > MaxTokenLength) return null;
                
                index++;

                // If the buffer contains a valid token, break
                if (ValidTokens.Contains(tokenBuffer)) break;
            }

            // If the token is not read successfully, return null
            if (tokenBuffer.Length > 0 && !ValidTokens.Contains(tokenBuffer)) return null;

            // Read the expression in the buffer
            var expressionBuffer = "";
            if (index < source.Length && Groups.ContainsKey(source[index])) {
                var start = source[index];
                // Skip opening
                index++;

                var success = false;
                var depth = 1;

                while (index < source.Length) {
                    if (source[index] == start) {
                        // If another starting group token is found, increase the depth
                        depth++;
                    } else if (Groups[start] == source[index]) {
                        // If a closing group token is found, decrease the depth
                        depth--;

                        if (depth == 0) {
                            // If there are as many closing tokens as starting tokens (depth = 0),
                            // the expression has been parsed successfully
                            success = true;
                            break;
                        }
                    }

                    // Add the character to the buffer
                    expressionBuffer += source[index];
                    index++;
                }

                // If the expression is not read successfully, return null
                if (!success) return null;
            }

            // Return a new token object
            return new Token() {
                StartIndex = startIndex,
                EndIndex = index,
                Tag = tokenBuffer,
                Expression = expressionBuffer
            };
        }

        // Indent the buffer with N number of tabs
        private static void Indent(StringBuilder builder, int n) {
            for (var i = 0; i < n; i++) {
                builder.Append("\t");
            }
        }

        // Transpiles a template to javascript, writes the result to the output buffer
        private static void TranspileTemplate(string source, StringBuilder output, int depth) {
            // Used for storing intermediate results
            var buffer = new StringBuilder();

            var index = 0;

            while (index < source.Length) {
                if (source[index] == TokenStart) {
                    // Try to parse the token
                    var token = ParseToken(source, index);
                    if (token != null) {
                        // The token parsed successfully

                        // If the buffer is not empty, clear it
                        if (buffer.Length > 0) {
                            Indent(output, depth);
                            output.Append($"Buffer.write(`{buffer.ToString().Replace("`", "\\`")}`);\n");
                            buffer.Clear();
                        }
                        
                        Indent(output, depth);

                        if (token.Tag == "") {
                            // Expression token, execute the expression and call toString on the result
                            output.Append($"Buffer.write({token.Expression});\n");
                        } else if (token.Tag == For || token.Tag == If) {
                            Indent(output, depth);
                            output.Append($"{token.Tag}({token.Expression}){{\n");
                            depth++;
                        } else if (token.Tag == ElseIf) {
                            Indent(output, depth - 1);
                            output.Append($"}}else if({token.Expression}){{\n");
                        } else if (token.Tag == Else) {
                            Indent(output, depth - 1);
                            output.Append($"}}{token.Tag}{{\n");
                        } else if (token.Tag.StartsWith(End)) {
                            // End token like /for or /if
                            Indent(output, depth);
                            output.Append("}\n");
                            depth--;
                        }

                        // Set the index to the index after the token
                        index = token.EndIndex + 1;
                        continue;
                    }
                }

                // Add the character to the buffer
                buffer.Append(source[index]);
                index++;
            }

            // Clear the buffer if it is not empty
            if (buffer.Length > 0) {
                output.Append($"Buffer.write(`{buffer.ToString().Replace("`", "\\`")}`);\n");
            }
        }

        // Transpile the template to javascript
        public static string TranspileTemplate(string source, bool returnVar) {
            // Decode HTML entities like $lt;
            source = HttpUtility.HtmlDecode(source);

            // Buffer to write result to
            var builder = new StringBuilder();

            // Transpile the template
            TranspileTemplate(source, builder, 1);

            // Wrap the result in a function, so it can be called with arguments
            var result = $@"(function(args){{
                with (args || {{}}){{
                    {builder.ToString()}
                    {(returnVar ? "return Buffer.get();" : "")}
                }}
            }}).valueOf()";

            return result;
        }
    }
}
