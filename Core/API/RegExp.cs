using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NetJS.Core.API {
    class RegExpAPI {

        public static Constant constructor(Constant _this, Constant[] arguments, Agent agent) {
            var thisObject = (Object)_this;

            thisObject.Set("source", (String)arguments[0]);

            var flags = arguments.Length > 1 ? (String)arguments[1] : new String("");
            thisObject.Set("flags", flags);

            thisObject.Set("global", Boolean.Create(flags.Value.IndexOf(Chars.RegexGlobal) != -1));
            thisObject.Set("ignoreCase", Boolean.Create(flags.Value.IndexOf(Chars.RegexIgnoreCase) != -1));
            thisObject.Set("multiline", Boolean.Create(flags.Value.IndexOf(Chars.RegexMultiLine) != -1));
            thisObject.Set("unicode", Boolean.Create(flags.Value.IndexOf(Chars.RegexUnicode) != -1));
            thisObject.Set("sticky", Boolean.Create(flags.Value.IndexOf(Chars.RegexSticky) != -1));

            return Static.Undefined;
        }

        private static RegexOptions GetOptions(Object _this) {
            var options = new RegexOptions();
            options |= RegexOptions.ECMAScript;

            if ((Convert.ToBoolean(_this.Get("ignoreCase")))) options |= RegexOptions.IgnoreCase;
            if ((Convert.ToBoolean(_this.Get("multiline")))) options |= RegexOptions.Multiline;

            // TODO: only enable this for xdoc
            options |= RegexOptions.IgnoreCase;

            return options;
        }

        public static Constant match(Constant _this, Constant[] arguments, Agent agent) {
            var exp = (Object)_this;
            var str = ((String)arguments[0]).Value;
            var source = ((String)exp.Get("source")).Value;

            var options = GetOptions(exp);

            var result = new List<string>();

            if ((Convert.ToBoolean(exp.Get("global")))) {
                var matches = Regex.Matches(str, source, options);

                if(matches.Count == 0) {
                    return Static.Null;
                }

                foreach(Match match in matches) {
                    result.Add(match.Value);
                }
            } else {
                var match = Regex.Match(str, source, options);

                if (!match.Success) {
                    return Static.Null;
                }

                result.Add(match.Value);
                foreach(Group group in match.Groups) {
                    result.Add(group.Value);
                }
            }

            return Tool.ToArray(result, agent);
        }

        public static Constant test(Constant _this, Constant[] arguments, Agent agent) {
            if (arguments[0] is Undefined) arguments[0] = new String("");

            return Boolean.Create(!(match(_this, arguments, agent) is Null));
        }

        public static Constant replace(Constant _this, Constant[] arguments, Agent agent) {
            var exp = (Object)_this;
            var str = (String)arguments[0];
            var replacement = (String)arguments[1];
            var source = ((String)exp.Get("source")).Value;

            var options = GetOptions(exp);

            var regex = new Regex(source, options);
            return new String(regex.Replace(str.Value, replacement.Value, (Convert.ToBoolean(exp.Get("global"))) ? -1 : 1));
        }

        public static Constant search(Constant _this, Constant[] arguments, Agent agent) {
            var exp = (Object)_this;
            var str = (String)arguments[0];
            var source = ((String)exp.Get("source")).Value;

            var options = GetOptions(exp);

            var match = Regex.Match(str.Value, source, options);
            if (match.Success) {
                return new Number(match.Index);
            } else {
                return new Number(-1);
            }
        }

        public static Constant split(Constant _this, Constant[] arguments, Agent agent) {
            var exp = (Object)_this;
            var str = (String)arguments[0];
            var source = ((String)exp.Get("source")).Value;

            var options = GetOptions(exp);

            var result = Regex.Split(str.Value, source, options);
            return Tool.ToArray(result, agent);
        }
    }
}
