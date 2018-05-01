using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Chars {

        public const char Null = '\0';

        public const char NewLine = '\n';
        public const char CarriageReturn = '\r';
        public const char Tab = '\t';
        public const char Backslash = '\\';
        public const char SingleQuote = '\'';
        public const char DoubleQuote = '"';

        public const char Template = '`';

        public const char RegexGlobal = 'g';
        public const char RegexIgnoreCase = 'i';
        public const char RegexMultiLine = 'm';
        public const char RegexUnicode = 'u';
        public const char RegexSticky = 'y';

        public static bool IsWhitespace(char c) {
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }

        public static bool IsStructure(char c) {
            return
                c == '{' || c == '}' ||
                c == '(' || c == ')' ||
                c == '[' || c == ']' ||
                c == ';' || c == ',' ||
                c == '?' || c == ':';
        }

        public static bool IsOperator(char c) {
            return
                c == '=' || c == '+' || c == '-' ||
                c == '<' || c == '>' || c == '/' ||
                c == '*' || c == '&' || c == '|' ||
                c == '!' || c == '%' || c == '.' ||
                c == '^' || c == '~';
        }

        public static bool IsString(char c) {
            return c == Chars.SingleQuote || c == Chars.DoubleQuote;
        }

        public static bool IsNumerical(char c) {
            return c >= '0' && c <= '9';
        }
    }
}
