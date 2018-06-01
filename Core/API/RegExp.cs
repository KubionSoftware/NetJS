using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NetJS.Core.API {
    class RegExp {

        public static Javascript.Constant constructor(Javascript.Constant _this, Javascript.Constant[] arguments, Javascript.Scope scope) {
            var thisObject = (Javascript.Object)_this;

            thisObject.Set("source", (Javascript.String)arguments[0]);

            var flags = arguments.Length > 1 ? (Javascript.String)arguments[1] : new Javascript.String("");
            thisObject.Set("flags", flags);

            thisObject.Set("global", new Javascript.Boolean(flags.Value.IndexOf(Javascript.Chars.RegexGlobal) != -1));
            thisObject.Set("ignoreCase", new Javascript.Boolean(flags.Value.IndexOf(Javascript.Chars.RegexIgnoreCase) != -1));
            thisObject.Set("multiline", new Javascript.Boolean(flags.Value.IndexOf(Javascript.Chars.RegexMultiLine) != -1));
            thisObject.Set("unicode", new Javascript.Boolean(flags.Value.IndexOf(Javascript.Chars.RegexUnicode) != -1));
            thisObject.Set("sticky", new Javascript.Boolean(flags.Value.IndexOf(Javascript.Chars.RegexSticky) != -1));

            return Javascript.Static.Undefined;
        }

        private static RegexOptions GetOptions(Javascript.Object _this) {
            var options = new RegexOptions();
            options |= RegexOptions.ECMAScript;

            if ((_this.Get<Javascript.Boolean>("ignoreCase")).Value) options |= RegexOptions.IgnoreCase;
            if ((_this.Get<Javascript.Boolean>("multiline")).Value) options |= RegexOptions.Multiline;

            // TODO: only enable this for xdoc
            options |= RegexOptions.IgnoreCase;

            return options;
        }

        public static Javascript.Constant match(Javascript.Constant _this, Javascript.Constant[] arguments, Javascript.Scope scope) {
            var exp = (Javascript.Object)_this;
            var str = ((Javascript.String)arguments[0]).Value;
            var source = ((Javascript.String)exp.Get("source")).Value;

            var options = GetOptions(exp);

            var result = new List<string>();

            if ((exp.Get<Javascript.Boolean>("global")).Value) {
                var matches = Regex.Matches(str, source, options);

                if(matches.Count == 0) {
                    return Javascript.Static.Null;
                }

                foreach(Match match in matches) {
                    result.Add(match.Value);
                }
            } else {
                var match = Regex.Match(str, source, options);

                if (!match.Success) {
                    return Javascript.Static.Null;
                }

                result.Add(match.Value);
                foreach(Group group in match.Groups) {
                    result.Add(group.Value);
                }
            }

            return Tool.ToArray(result);
        }

        public static Javascript.Constant test(Javascript.Constant _this, Javascript.Constant[] arguments, Javascript.Scope scope) {
            if (arguments[0] is Javascript.Undefined) arguments[0] = new Javascript.String("");

            return new Javascript.Boolean(!(match(_this, arguments, scope) is Javascript.Null));
        }

        public static Javascript.Constant replace(Javascript.Constant _this, Javascript.Constant[] arguments, Javascript.Scope scope) {
            var exp = (Javascript.Object)_this;
            var str = (Javascript.String)arguments[0];
            var replacement = (Javascript.String)arguments[1];
            var source = ((Javascript.String)exp.Get("source")).Value;

            var options = GetOptions(exp);

            var regex = new Regex(source, options);
            return new Javascript.String(regex.Replace(str.Value, replacement.Value, (exp.Get<Javascript.Boolean>("global")).Value ? -1 : 1));
        }

        public static Javascript.Constant search(Javascript.Constant _this, Javascript.Constant[] arguments, Javascript.Scope scope) {
            var exp = (Javascript.Object)_this;
            var str = (Javascript.String)arguments[0];
            var source = ((Javascript.String)exp.Get("source")).Value;

            var options = GetOptions(exp);

            var match = Regex.Match(str.Value, source, options);
            if (match.Success) {
                return new Javascript.Number(match.Index);
            } else {
                return new Javascript.Number(-1);
            }
        }

        public static Javascript.Constant split(Javascript.Constant _this, Javascript.Constant[] arguments, Javascript.Scope scope) {
            var exp = (Javascript.Object)_this;
            var str = (Javascript.String)arguments[0];
            var source = ((Javascript.String)exp.Get("source")).Value;

            var options = GetOptions(exp);

            var result = Regex.Split(str.Value, source, options);
            return Tool.ToArray(result);
        }
    }
}
