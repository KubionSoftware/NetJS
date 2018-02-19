using System;
using System.Collections.Generic;
using System.Text;

namespace NetJS.Javascript {
    public class Html : Node {
        public List<object> Parts;

        public Html(string value) {
            Parts = ParseHtml(value);
        }

        public void Combine(Html other) {
            Parts.AddRange(other.Parts);
        }

        public string Cut(string s, int start, int end) {
            return s.Substring(start, end - start);
        }

        public Tuple<int, int> FindCode(string s) {
            var startIndex = -1;

            for (var i = 0; i < s.Length; i++) {
                if (s[i] == '#') {
                    if (startIndex == -1) {
                        startIndex = i;
                    } else {
                        return new Tuple<int, int>(startIndex, i);
                    }
                }
            }

            return new Tuple<int, int>(startIndex, -1);
        }

        public List<object> ParseHtml(string html) {
            var result = new List<object>();

            var location = FindCode(html);

            if (location.Item1 != -1) {
                if (location.Item2 == -1) {
                    throw new Exception("Unclosed #-# in html");
                }

                var left = Cut(html, 0, location.Item1);
                var right = ParseHtml(Cut(html, location.Item2 + 1, html.Length));

                var middleText = Cut(html, location.Item1 + 1, location.Item2);
                var middleTokens = Lexer.Lex(middleText);

                // TODO: parser debug file name
                var parser = new Parser("HTML", middleTokens);
                var middle = parser.Parse();

                result.Add(left);
                result.Add(middle);
                result.AddRange(right);
            } else {
                result.Add(html);
            }

            return result;
        }

        public string ToString(Scope scope) {
            var output = "";

            foreach (var part in Parts) {
                if (part is string s) {
                    output += s;
                } else if (part is Block block) {
                    var result = block.Execute(scope);
                    output += result.Constant.GetString(scope);
                }
            }

            return output;
        }

        public override void Uneval(StringBuilder builder, int depth) {
            foreach (var part in Parts) {
                if (part is string s) {
                    builder.Append(s);
                } else if (part is Block block) {
                    block.Uneval(builder, depth);
                }
            }
        }
    }
}