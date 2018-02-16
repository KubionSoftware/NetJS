using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NetJS.Javascript {
    
    public abstract class Expression : Node {

        public virtual Constant Execute(Scope scope, bool getValue = true) {
            return Static.Undefined;
        }

        public bool IsTrue(Scope scope) {
            var result = Execute(scope);

            if (result is Boolean) {
                return ((Boolean)result).Value;
            } else if(result is Number) {
                return ((Number)result).Value != 0;
            } else if(result is String) {
                return ((String)result).Value.Length > 0;
            } else if(result is Object) {
                return true;
            }

            // undefined, null, NaN
            return false;
        }

        public virtual Constant GetValue(Scope scope) {
            return Static.Undefined;
        }
    }

    public class ParameterList : Node {
        public IList<Variable> Parameters = new List<Variable>();

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append("(");

            for(var i = 0; i < Parameters.Count; i++) {
                if (i > 0) builder.Append(", ");
                Parameters[i].Uneval(builder, depth);
            }

            builder.Append(")");
        }
    }
    
    public class Html : Node {
        public List<object> Parts;

        public Html(string value) {
            Parts = ParseHtml(value);
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
                if (part is string) {
                    output += (string)part;
                } else if (part is Block) {
                    var result = ((Block)part).Execute(scope);
                    output += result.Constant.GetString(scope);
                }
            }

            return output;
        }

        public override void Uneval(StringBuilder builder, int depth) {
            foreach (var part in Parts) {
                if (part is string) {
                    builder.Append((string)part);
                } else if (part is Block) {
                    ((Block)part).Uneval(builder, depth);
                }
            }
        }
    }

    public abstract class Statement : Node {
        
        public virtual Scope.Result Execute(Scope scope) {
            return new Scope.Result(Scope.ResultType.None);
        }
    }

    public abstract class Node {

        public int Id = -1;

        public void RegisterDebug(Debug.Location location) {
            Id = Debug.AddNode(location);
        }

        public abstract void Uneval(StringBuilder builder, int depth);
        public static void NewLine(StringBuilder builder, int depth) {
            builder.Append("\n");
            for(var i = 0; i < depth; i++) {
                builder.Append("\t");
            }
        }
    }
}