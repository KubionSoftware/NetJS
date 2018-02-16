using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Util;

namespace XDocConvert {

    /// <summary>
    /// Converts XDoc to JSDoc
    /// Warning: this class is not thread-safe!!!
    /// </summary>
    class Converter {

        public class Code {
            public string Content;
            public bool IsExpression;

            public Code(string content, bool isExpression) {
                Content = content;
                IsExpression = isExpression;
            }
        }

        private Dictionary<string, Func<Json, int, int, bool, Code>> _commandMap;
        private Dictionary<string, string> _specialMap;

        private bool InTag = false;
        private bool InAttribute = false;
        private char AttributeToken = '\0';
        private bool InBlock = false;

        private const char EscapeOpen = '†';
        private const char EscapeClose = '‡';

        private const char SingleString = '"';
        private const char MultiString = '`';

        private const char NewLine = '\n';

        private const char HtmlOpen = '<';
        private const char HtmlClose = '>';

        private const string InlineOpen = "#";
        private const string InlineClose = "#";

        private const string Add = " + ";
        private const string Var = "var";

        private const string SplitXX = "SPLITXX";

        public Converter() {
            _commandMap = new Dictionary<string, Func<Json, int, int, bool, Code>>() {
                { "SPAR", ConvertSPAR },
                { "JPAR", ConvertJPAR },
                { "SVSET", ConvertSVSET },
                { "SVGET", ConvertSVGET },
                { "SVDEL", ConvertSVDEL },
                { "IF", ConvertIF },
                { "EXIT", ConvertEXIT },
                { "MSG", ConvertMSG },
                { "Text", ConvertText },
                { "CFG", ConvertCFG },
                { "JLOOP", ConvertJLOOP },
                { "INCLUDE", ConvertINCLUDE },
                { "PAR", ConvertPAR },
                { "DEFPAR", ConvertDEFPAR },
                { "FUNC", ConvertFUNC },
                { "JDATA", ConvertJDATA },
                { "JSET", ConvertJSET },
                { "JKEYS", ConvertJKEYS },
                { "IIF", ConvertIIF }
            };

            _specialMap = new Dictionary<string, string>() {
                { "_templatename_", "[[TemplateName]]" },
                { "_username_", "[[UserName]]" },
                { "_open_bracket_", "(" },
                { "_close_bracket_", ")" },
                { "_dashsign_", "#" },
                { "dash", "#" },
                { "_percentsign_", "%" },
                { "_atsign_", "@" },
                { "_dotsign_", "." },
                { "_commasign_", "," },
                { "_crlfsign_", "\n" },
                { "_tabsign_", "\t" },
                { "_singlequote_", "'" },
                { "_doublequote_", "\"" },
                { "_backslash_", "\\" },
                { "_xdocversion_", "4.2.0" }
            };
        }

        public string XDocToJSDoc(string xdoc) {
            InTag = false;
            InAttribute = false;
            AttributeToken = '\0';
            InBlock = false;

            if (xdoc == "error") return "";

            var builder = new StringBuilder();

            var json = new Json(xdoc);

            if (json.Has("Commands")) {
                ConvertCommands(json.Objects("Commands"), 0, builder, -1, false);
            }

            return builder.ToString();
        }

        public void ConvertCommands(IEnumerable<Json> commands, int depth, StringBuilder builder, int line, bool escapePar) {
            foreach (var command in commands) {
                if (command.String("Type") == "Text") {
                    var value = command.String("Value");
                    if (string.IsNullOrWhiteSpace(value)) continue;
                }

                ConvertCommand(command, depth, builder, line, escapePar);
            }
        }

        public void AppendNewLine(StringBuilder builder, int depth) {
            builder.Append(NewLine);
            Indent(builder, depth);
        }

        public void Indent(StringBuilder builder, int depth) {
            for (var i = 0; i < depth; i++) {
                builder.Append("\t");
            }
        }

        public char LastNonWhiteSpace(string s) {
            for (var i = s.Length - 1; i >= 0; i--) {
                var c = s[i];
                if (!Char.IsWhiteSpace(c)) {
                    return c;
                }
            }

            return '\0';
        }

        public bool IsString(char c) {
            return c == SingleString || c == MultiString;
        }

        public bool StartsString(string s) {
            return s[0] == SingleString || s[0] == MultiString;
        }

        public bool EndsString(string s) {
            var last = s[s.Length - 1];
            return last == SingleString || last == MultiString;
        }

        public void AppendCode(StringBuilder builder, string code, bool isExpression, int depth) {
            if(builder.Length == 0) {
                builder.Append(code);
            } else if (isExpression) {
                var checkLength = Math.Min(builder.Length, 100);
                var checkPart = builder.ToString(builder.Length - checkLength, checkLength);
                var last = LastNonWhiteSpace(checkPart);

                if ((IsString(last) || last == ')') && (!code.StartsWith(HtmlOpen.ToString()) && !code.StartsWith(Var))) {
                    builder.Append(Add);
                } else if (StartsString(code) && last != HtmlClose && last != '{' && last != ';' && last != '}') {
                    builder.Append(Add);
                } else if (last == '}' || last == ';' || last == '{') {
                    AppendNewLine(builder, depth);
                }

                builder.Append(code);
            } else {
                var last = builder.ToString(builder.Length - 1, 1)[0];
                if (last != '\n' && last != '\t') {
                    AppendNewLine(builder, depth);
                }
                builder.Append(code);
            }
        }

        public Code ConvertJPAR(Json command, int depth, int line, bool escapePar) {
            var builder = new StringBuilder();

            var paramName = command.String("ParamName");
            ConvertVar(builder, paramName);

            builder.Append(ConvertValue(command.String("Name")));

            var value = ConvertValue(command.String("Value"), false);
            if (value.Length > 0) {
                if (!value.StartsWith("[")) {
                    builder.Append(".");
                }
                builder.Append(value);
            }

            if (paramName.Length > 0) builder.Append(";");

            return new Code(builder.ToString(), paramName.Length == 0);
        }

        public Code ConvertSVGET(Json command, int depth, int line, bool escapePar) {
            var builder = new StringBuilder();

            if (escapePar) builder.Append(EscapeOpen);

            var paramName = command.String("ParamName");
            ConvertVar(builder, paramName);

            builder.Append($"Session.get({ConvertValue(command.String("Name"))}, {ConvertValue(command.String("Context"))}, {ConvertValue(command.String("ID"))})");

            if (paramName.Length > 0) builder.Append(";");
            if (escapePar) builder.Append(EscapeClose);

            return new Code(builder.ToString(), paramName.Length == 0);
        }

        public Code ConvertSVSET(Json command, int depth, int line, bool escapePar) {
            return new Code($"Session.set({ConvertValue(command.String("Name"))}, {ConvertValue(command.String("Context"))}, {ConvertValue(command.String("ID"))}, {ConvertValue(command.String("Value"))});", false);
        }

        public Code ConvertSVDEL(Json command, int depth, int line, bool escapePar) {
            return new Code($"Session.delete({ConvertValue(command.String("Name"))}, {ConvertValue(command.String("Context"))}, {ConvertValue(command.String("ID"))});", false);
        }

        public Code ConvertIF(Json command, int depth, int line, bool escapePar) {
            var builder = new StringBuilder();

            var condition = "[[UNPARSABLE]]";
            if (command.Has("Value")) {
                condition = command.String("Value");
            } else if (command.Has("Params")) {
                var conditionBuilder = new StringBuilder();
                ConvertCommands(command.Objects("Params"), depth, conditionBuilder, int.Parse(command.String("Line")), true);
                condition = conditionBuilder.ToString();
            }

            if (condition.Length > 0) {
                builder.Append($"if ({ConvertCondition(condition, true)}) ");
            }
            builder.Append("{");
            AppendNewLine(builder, depth + 1);

            if (command.Has("Commands")) {
                var converter = new Converter();
                converter.ConvertCommands(command.Objects("Commands"), depth + 1, builder, line, escapePar);
            }
            
            AppendCode(builder, "}", false, depth);

            if (command.Has("ElseIf")) {
                builder.Append(" else ");
                var elseResult = ConvertIF(command.Object("ElseIf"), depth, line, escapePar);
                AppendCode(builder, elseResult.Content, true, depth);
            }

            return new Code(builder.ToString(), false);
        }

        public Code ConvertText(Json command, int depth, int line, bool escapePar) {
            var builder = new StringBuilder();

            var value = command.String("Value");

            if (InTag && !escapePar) {
                builder.Append(InlineClose);
            }

            ConvertHtml(builder, value, !escapePar);

            if (InTag && !escapePar) {
                builder.Append(InlineOpen);
            }

            return new Code(builder.ToString(), true);
        }

        public Code ConvertCFG(Json command, int depth, int line, bool escapePar) {
            var builder = new StringBuilder();

            var paramName = command.String("ParamName");
            ConvertVar(builder, paramName);

            builder.Append($"Config.{command.String("Context")}.{command.String("Name")};");

            return new Code(builder.ToString(), paramName.Length > 0);
        }

        public Code ConvertJLOOP(Json command, int depth, int line, bool escapePar) {
            var builder = new StringBuilder();

            var paramName = command.String("ParamName");
            var collectionName = ConvertValue(command.String("Name"));

            AppendCode(builder, $"{collectionName}.forEach(function({paramName}, {paramName}__fetchId){{", false, depth);

            if (command.Has("Commands")) {
                ConvertCommands(command.Objects("Commands"), depth + 1, builder, line, escapePar);
            }

            AppendCode(builder, "});", false, depth);

            return new Code(builder.ToString(), false);
        }

        public Code ConvertINCLUDE(Json command, int depth, int line, bool escapePar) {
            var builder = new StringBuilder();

            var paramName = command.String("ParamName");

            var concatBuilder = new StringBuilder();
            ConvertCommands(command.Objects("Params"), depth, concatBuilder, line, true);
            var concat = HttpUtility.UrlDecode(concatBuilder.ToString());

            var templateName = "";
            var arguments = new List<string>();
            var compoundArguments = new List<string>();

            var parameters = concat.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var parameter in parameters) {
                var parts = parameter.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2) {
                    if (parts[0].ToLower() == "templatename") {
                        templateName = ConvertValue(parts[1]);
                    } else {
                        var key = ConvertValue(parts[0]);
                        var value = ConvertValue(parts[1]);

                        if (IsString(key[0]) && IsString(key[key.Length - 1]) && key.Count(IsString) == 2) {
                            arguments.Add($"{key}: {value}");
                        } else {
                            compoundArguments.Add($"[{key}] = {value}");
                        }
                    }
                }
            }

            if (compoundArguments.Count > 0) {
                builder.Append($"{Var} includeData = {{");

                if (arguments.Count > 0) {
                    for (var i = 0; i < arguments.Count; i++) {
                        if (i > 0) builder.Append(",");

                        builder.Append(NewLine);
                        Indent(builder, depth + 1);

                        builder.Append(arguments[i]);
                    }

                    builder.Append(NewLine);
                    Indent(builder, depth);
                }

                builder.Append("};");

                foreach (var argument in compoundArguments) {
                    builder.Append(NewLine);
                    Indent(builder, depth);
                    builder.Append($"includeData{argument};");
                }

                builder.Append(NewLine);
                Indent(builder, depth);
                
                ConvertVar(builder, paramName);
                builder.Append($"IO.include({templateName}, includeData);");
            } else {
                ConvertVar(builder, paramName);
                builder.Append($"IO.include({templateName}, {{");

                if (arguments.Count > 0) {
                    for (var i = 0; i < arguments.Count; i++) {
                        if (i > 0) builder.Append(",");

                        builder.Append(NewLine);
                        Indent(builder, depth + 1);

                        builder.Append(arguments[i]);
                    }

                    builder.Append(NewLine);
                    Indent(builder, depth);
                }

                builder.Append("})");
                if (paramName.Length > 0) builder.Append(";");
            }

            return new Code(builder.ToString(), paramName.Length == 0 && compoundArguments.Count == 0);
        }

        public Code ConvertFUNC(Json command, int depth, int line, bool escapePar) {
            var builder = new StringBuilder();

            if (escapePar) builder.Append(EscapeOpen);

            var paramName = ConvertPar(command.String("ParamName"));
            ConvertVar(builder, paramName);

            var value1 = ConvertValue(command.String("Value"));
            var value2 = ConvertValue(command.String("Value2"));
            var value3 = ConvertValue(command.String("Value3"));

            var func = command.String("Name").ToLower();
            if (func == "string_replace") {
                builder.Append($"{value1}.replace(new RegExp({value2}, \"g\"), {value3})");
            } else if (func == "string_indexof") {
                builder.Append($"{value1}.indexOf({value2})");
            } else {
                builder.Append("[[" + func + "]]");
            }

            if (paramName.Length > 0) builder.Append(";");
            if (escapePar) builder.Append(EscapeClose);

            return new Code(builder.ToString(), paramName.Length == 0);
        }

        public Code ConvertJDATA(Json command, int depth, int line, bool escapePar) {
            var builder = new StringBuilder();

            if (escapePar) builder.Append(EscapeOpen);

            var paramName = command.String("ParamName");

            var value = ConvertValue(command.String("Value"), false);
            
            if (value.StartsWith(SplitXX)) {
                var rowSeperator = value[SplitXX.Length];
                var columnSeperator = value[SplitXX.Length + 1];

                var keysString = value.Substring(SplitXX.Length + 2);
                var keys = keysString.Split(new[] { columnSeperator }, StringSplitOptions.RemoveEmptyEntries);

                AppendCode(builder, $"{Var} {paramName}Keys = [{string.Join(", ", keys.Select(key => '"' + key + '"'))}];", false, depth);
                AppendCode(builder, $"{Var} {ConvertPar(paramName)} = ({ConvertValue(command.String("Name"))}).split(\"{rowSeperator}\").map(function({paramName}Row){{", false, depth);
                AppendCode(builder, $"return {paramName}Row.split(\"{columnSeperator}\").reduce(function({paramName}Map, {paramName}Column, {paramName}Index){{", false, depth + 1);
                AppendCode(builder, $"{paramName}Map[{paramName}Keys[{paramName}Index]] = {paramName}Column;", false, depth + 2);
                AppendCode(builder, $"return {paramName}Map;", false, depth + 2);
                AppendCode(builder, "}, {});", false, depth + 1);
                AppendCode(builder, "});", false, depth);
            } else {
                ConvertVar(builder, paramName);

                builder.Append($"HTTP.execute(\"{value}\", {ConvertValue(command.String("Name"))}, {ConvertValue(command.String("Context"))})");

                if (paramName.Length > 0) builder.Append(";");
            }

            if (escapePar) builder.Append(EscapeClose);

            return new Code(builder.ToString(), paramName.Length > 0);
        }

        public Code ConvertPAR(Json command, int depth, int line, bool escapePar) {
            var output = "";

            if (escapePar) output += EscapeOpen;
            output += ConvertPar(command.String("ParamName"));
            if (escapePar) output += EscapeClose;

            return new Code(output, true);
        }

        public Code ConvertDEFPAR(Json command, int depth, int line, bool escapePar) {
            var builder = new StringBuilder();

            var paramName = command.String("ParamName");

            builder.Append($"if(!{ConvertPar(paramName)}){{ ");
            ConvertVar(builder, paramName);
            builder.Append($"{ConvertValue(command.String("Value"))}; }}");

            return new Code(builder.ToString(), false);
        }

        public Code ConvertSPAR(Json command, int depth, int line, bool escapePar) {
            var builder = new StringBuilder();
            
            var paramName = command.String("ParamName");
            ConvertVar(builder, paramName);

            var getVariable = command.Has("Opt1") && command.String("Opt1") == "1";
            if (getVariable) {
                // TODO: this is totally not solid
                builder.Append(ConvertPar(ConvertValue(command.String("Value"), false, true)).Replace("_", ""));
            } else {
                builder.Append(ConvertValue(command.String("Value")));
            }

            if (paramName.Length > 0) builder.Append(';');

            return new Code(builder.ToString(), false);
        }

        public Code ConvertMSG(Json command, int depth, int line, bool escapePar) {
            return new Code($"Log.write(\"{command.String("Name")} - \" + {ConvertValue(command.String("Value"))});", false);
        }

        public Code ConvertEXIT(Json command, int depth, int line, bool escapePar) {
            return new Code("return;", false);
        }

        public Code ConvertJSET(Json command, int depth, int line, bool escapePar) {
            var paramName = command.String("ParamName");
            var valueName = ConvertValue(command.String("Name"), false);
            var key = ConvertValue(command.String("Value"), false);
            var op = command.String("Opt1");

            var output = "";

            if (paramName != valueName) {
                output = $"{Var} {paramName} = {valueName};";
            }

            if (op == "APP") {
                output += paramName;
                if (key.Length > 0) output += $".{key}";
                output += $".push({ConvertValue(command.String("Value2"))});";
            } else if (op == "INS" || op == "UPD") {
                output += paramName;
                if (key.Length > 0) output += $".{key}";
                output += $"[{ConvertValue(command.String("Value3"))}] = {ConvertValue(command.String("Value2"))}";
            } else if (op.Length == 0) {
                output += $"{paramName}.{key} = {ConvertValue(command.String("Value2"))};";
            } else {
                return new Code("UKNOWN JSET " + op, false);
            }

            return new Code(output, paramName.Length > 0);
        }

        public Code ConvertJKEYS(Json command, int depth, int line, bool escapePar) {
            var builder = new StringBuilder();

            var paramName = command.String("ParamName");
            var collectionName = ConvertValue(command.String("Name"));
            var keyName = paramName + "__key";

            AppendCode(builder, $"for ({Var} {keyName} in {collectionName}) {{", false, depth);
            AppendCode(builder, $"{Var} {paramName}__value = {collectionName}[{keyName}];", false, depth + 1);

            if (command.Has("Commands")) {
                ConvertCommands(command.Objects("Commands"), depth + 1, builder, line, escapePar);
            }

            AppendCode(builder, "}", false, depth);

            return new Code(builder.ToString(), false);
        }

        public Code ConvertIIF(Json command, int depth, int line, bool escapePar) {
            var output = "";

            var paramName = command.String("ParamName");
            if (paramName.Length > 0) output += $"{Var} {paramName} = ";

            output += $"({ConvertCondition(command.String("ID"), true)} ? {ConvertValue(command.String("Value2"))} : {ConvertValue(command.String("Value3"))})";

            if (paramName.Length > 0) output += ";";

            return new Code(output, paramName.Length > 0);
        }

        public void ConvertCommand(Json command, int depth, StringBuilder builder, int line, bool escapePar) {
            var type = command.String("Type");

            if (_commandMap.ContainsKey(type)) {
                var code = _commandMap[type](command, depth, line, escapePar);
                AppendCode(builder, code.Content, code.IsExpression, depth);
            } else {
                AppendCode(builder, $"UNKOWN - {type}", false, depth);
            }
        }

        public void ConvertVar(StringBuilder builder, string paramName) {
            if(paramName.Length > 0) {
                var param = ConvertPar(paramName);

                if(!param.Contains(".") && !param.Contains("[")) {
                    builder.Append(Var + " ");
                }

                builder.Append(param + " = ");
            }
        }

        private string[] IgnoreFuncs = { "urlencode", "urldecode" };

        public string NormalizePar(string s) {
            var lower = s.ToLower();
            if(lower == "_fetchid") {
                return "_fetchId";
            } else {
                return s;
            }
        }

        public string ConvertPar(string value) {
            if (value.Contains(".")) {
                var parts = value.Split(new char[] { '.' });
                if (parts.Length == 2 && IgnoreFuncs.Contains(parts[1].ToLower())) {
                    value = parts[0];
                }
            }
            
            var result = new Regex("(?<!^)_(?!$|_|fetchid)", RegexOptions.IgnoreCase).Replace(value, ".", Math.Max(0, 1 - value.Count(c => c == '.')));
            return string.Join(".", result.Split(new[] { '.' }).Select(NormalizePar));
        }

        public string ConvertEquals(string condition, string equality, bool convertValue) {
            var parts = condition.Split(new string[] { equality }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1 && condition.EndsWith(equality)) {
                var value = convertValue ? ConvertValue(parts[0]) : parts[0];
                return equality == "!=" ? value : "!" + value;
            }

            if (parts.Length == 2) {
                var value0 = convertValue ? ConvertValue(parts[0]) : parts[0];
                var value1 = convertValue ? ConvertValue(parts[1]) : parts[1];

                return value0 + " " + equality + " " + value1;
            }

            return "UNPARSABLE - " + condition;
        }

        public string ConvertRegex(string condition, string equality, bool convertValue) {
            var parts = condition.Split(new string[] { equality }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2) {
                var value0 = convertValue ? ConvertValue(parts[0]) : parts[0];
                var value1 = convertValue ? ConvertValue(parts[1]) : parts[1];

                return (equality == "!==" ? "!" : "") + $"new RegExp({value1}).test({value0})";
            }

            return "UNPARSABLE - " + condition;
        }

        public string ConvertCondition(string condition, bool convertValue) {
            if (condition.Contains("===")) return ConvertRegex(condition, "===", convertValue);
            if (condition.Contains("!==")) return ConvertRegex(condition, "!==", convertValue);

            if (condition.Contains("==")) return ConvertEquals(condition, "==", convertValue);
            if (condition.Contains("!=")) return ConvertEquals(condition, "!=", convertValue);
            if (condition.Contains(">>>")) return ConvertEquals(condition, ">", convertValue);

            return ConvertValue(condition);
        }

        public string EscapeSingleString(string s) {
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r\n", "\\n").Replace("\n", "\\n");
        }

        public bool IsValidParamName(char c) {
            if (c >= 'a' && c <= 'z') return true;
            if (c >= 'A' && c <= 'Z') return true;
            if (c >= '0' && c <= '9') return true;
            if (c == '_' || c == '.') return true;
            return false;
        }

        public string ConvertValue(string value, bool stringify = true, bool arrayify = false) {
            var builder = new StringBuilder();

            var inParam = false;
            var inEscape = false;

            var buffer = "";

            var previousParam = false;

            void appendText(string s) {
                if (builder.Length == 0 && stringify) {
                    builder.Append(SingleString);
                } else if (previousParam && stringify) {
                    builder.Append(Add + SingleString);
                }

                builder.Append(s);

                previousParam = false;
            }

            void appendCode(string s) {
                if (builder.Length > 0) {
                    if (previousParam) {
                        builder.Append(Add);
                    } else if (stringify) {
                        builder.Append(SingleString + Add);
                    }
                }

                if (arrayify) builder.Append('[');
                builder.Append(s);
                if (arrayify) builder.Append(']');

                buffer = "";
                previousParam = true;
            }

            for (var i = 0; i < value.Length; i++) {
                var c = value[i];

                if (inParam) {
                    if (c == '%') {
                        if (_specialMap.ContainsKey(buffer.ToLower())) {
                            appendText(_specialMap[buffer.ToLower()]);
                            buffer = "";
                            inParam = false;
                        } else {
                            appendCode(ConvertPar(EscapeSingleString(buffer)));
                            inParam = false;
                        }
                    } else if (!IsValidParamName(c)) {
                        appendText('%' + EscapeSingleString(buffer));
                        buffer = "";
                        inParam = false;
                    } else {
                        buffer += c;
                    }
                } else if (inEscape) {
                    if(c == EscapeClose) {
                        appendCode(buffer);
                        inEscape = false;
                    } else {
                        buffer += c;
                    }
                } else if (c == '%') {
                    inParam = true;
                } else if (c == EscapeOpen) {
                    inEscape = true;
                } else {
                    appendText(EscapeSingleString(c.ToString()));
                }
            }

            if(buffer.Length > 0) {
                if (inParam) {
                    appendText('%' + EscapeSingleString(buffer));
                }else if (inEscape) {
                    appendText(EscapeOpen + buffer);
                }
            }

            if (builder.Length == 0 && stringify) {
                return "\"\"";
            } else if (!previousParam && stringify) {
                builder.Append(SingleString);
            }

            return builder.ToString();
        }

        public string GetBlock(string html, int index, bool readAll) {
            if (html[index] != '[') return null;

            var expected = "[block";

            var output = "";
            var valid = false;

            while(index < html.Length) {
                output += html[index];

                if (output.Length == expected.Length) {
                    if (output == expected) {
                        valid = true;
                        if (!readAll) return output;
                    } else {
                        return null;
                    }
                }

                if (html[index] == ']') {
                    if (valid) {
                        return output;
                    } else {
                        return null;
                    }
                }

                index++;
            }

            return null;
        }

        public void ConvertHtml(StringBuilder builder, string html, bool stringify) {
            var inText = false;

            if (InBlock) {
                if (stringify) builder.Append(MultiString);
            }

            for (var i = 0; i < html.Length; i++) {
                var c = html[i];

                if (InBlock) {
                    var block = GetBlock(html, i, true);
                    if(block != null) {
                        InBlock = false;
                        builder.Append(block);
                        i += block.Length - 1;
                        if (stringify) builder.Append(MultiString);
                        continue;
                    }
                } else if (inText) {
                    if(c == '<' || c == '>') {
                        inText = false;
                        InTag = c == '<';
                        if (stringify) builder.Append(MultiString);
                    } else {
                        var block = GetBlock(html, i, false);
                        if(block != null) {
                            inText = false;
                            if (stringify) builder.Append(MultiString);

                            InBlock = true;
                            if (stringify) builder.Append(Add + MultiString);
                            builder.Append(block);
                            i += block.Length - 1;
                            continue;
                        }
                    }
                } else if (InTag) {
                    if (InAttribute) {
                        if (c == AttributeToken) {
                            InAttribute = false;
                        }
                    } else {
                        if (c == '>') {
                            InTag = false;
                        } else if (c == '"' || c == '\'') {
                            InAttribute = true;
                            AttributeToken = c;
                        }
                    }
                } else {
                    if (c == '<') {
                        InTag = true;
                    } else if (Char.IsWhiteSpace(c)) {

                    } else {
                        var block = GetBlock(html, i, false);

                        if (block != null) {
                            InBlock = true;
                            if (stringify) builder.Append(MultiString);
                            builder.Append(block);
                            i += block.Length - 1;
                            continue;
                        } else {
                            inText = true;
                            if (stringify) builder.Append(MultiString);
                        }
                    }
                }

                builder.Append(c);
            }

            if ((inText || InBlock) && stringify) {
                builder.Append(MultiString);
            }
        }
    }
}
