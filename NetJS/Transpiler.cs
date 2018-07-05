using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS {
    class Transpiler {

        private const string For = "for";
        private const string If = "if";
        private const string End = "/";

        private const char TokenStart = '$';

        private static Dictionary<char, char> Groups = new Dictionary<char, char>() {
            { '{', '}' },
            { '(', ')' }
        };

        private static string[] ValidTokens = new string[] {
            For,
            End + For,
            If,
            End + If
        };
        private static int MaxTokenLength = 4;

        private class Token {
            public int StartIndex;
            public int EndIndex;
            public string Tag;
            public string Expression;
        }

        private static Token ParseToken(string source, int index) {
            var startIndex = index;

            if (source[index] != TokenStart) return null;
            // Skip start token
            index++;

            var tokenBuffer = "";
            while (index < source.Length) {
                if (Groups.ContainsKey(source[index])) break;
                tokenBuffer += source[index];
                if (tokenBuffer.Length > MaxTokenLength) return null;
                index++;
                if (ValidTokens.Contains(tokenBuffer)) break;
            }

            if (tokenBuffer.Length > 0 && !ValidTokens.Contains(tokenBuffer)) return null;

            var expressionBuffer = "";
            if (index < source.Length && Groups.ContainsKey(source[index])) {
                var start = source[index];
                // Skip opening
                index++;

                var success = false;
                var depth = 1;

                while (index < source.Length) {
                    if (source[index] == start) {
                        depth++;
                    } else if (Groups[start] == source[index]) {
                        depth--;
                        if (depth == 0) {
                            success = true;
                            break;
                        }
                    }
                    expressionBuffer += source[index];
                    index++;
                }

                if (!success) return null;
            }

            return new Token() {
                StartIndex = startIndex,
                EndIndex = index,
                Tag = tokenBuffer,
                Expression = expressionBuffer
            };
        }

        private static void AppendDepth(StringBuilder builder, int depth) {
            for (var i = 0; i < depth; i++) {
                builder.Append("\t");
            }
        }

        private static int ParseTemplate(string source, StringBuilder output, int index, int depth) {
            var buffer = new StringBuilder();

            while (index < source.Length) {
                if (source[index] == TokenStart) {
                    var token = ParseToken(source, index);
                    if (token != null) {
                        if (buffer.Length > 0) {
                            output.Append($"Buffer.write(`{buffer.ToString()}`);\n");
                            buffer.Clear();
                        }

                        AppendDepth(output, depth);

                        if (token.Tag == "") {
                            output.Append($"Buffer.write({token.Expression}.toString());\n");
                            index = token.EndIndex + 1;
                        } else if (token.Tag == "for") {
                            output.Append($"for({token.Expression}){{\n");
                            index = ParseTemplate(source, output, token.EndIndex + 1, depth + 1);
                            AppendDepth(output, depth);
                            output.Append("}\n");
                        } else if (token.Tag == "if") {
                            output.Append($"if({token.Expression}){{\n");
                            index = ParseTemplate(source, output, token.EndIndex + 1, depth + 1);
                            AppendDepth(output, depth);
                            output.Append("}\n");
                        } else if (token.Tag.StartsWith("/")) {
                            return token.EndIndex + 1;
                        }

                        continue;
                    }
                }

                buffer.Append(source[index]);
                index++;
            }

            if (buffer.Length > 0) {
                output.Append($"Buffer.write(`{buffer.ToString()}`);\n");
            }

            return index;
        }

        public static string TranspileTemplate(string source, bool returnVar) {
            var builder = new StringBuilder();
            ParseTemplate(source, builder, 0, 1);
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
