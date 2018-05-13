using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        public const string TypeOptional = "?";
        public const string Interface = "interface";
        public const string Any = "any";
        public const string String = "string";
        public const string Number = "number";
        public const string Boolean = "boolean";
        public const string Object = "object";
        public const string Array = "[]";

        public const string Var = "var";
        public const string Let = "let";
        public const string Const = "const";

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

        public const string Class = "class";
        public const string Constructor = "constructor";
        public const string Static = "static";
        public const string Extends = "extends";

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

        public static bool IsValidRegexFlag(char c) {
            return c == Chars.RegexGlobal || c == Chars.RegexIgnoreCase || c == Chars.RegexMultiLine || c == Chars.RegexUnicode || c == Chars.RegexSticky;
        }

        public static bool IsValidOperator(string op) {
            var validOperators = new[] {
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
}
