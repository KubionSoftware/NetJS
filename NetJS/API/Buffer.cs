using NetJS.Core.Javascript;

namespace NetJS.API {
    public class Buffer {

        public static Constant set(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var value = ((Core.Javascript.String)arguments[0]).Value;
            lex.Buffer.Clear();
            lex.Buffer.Append(value);
            return Static.Undefined;
        }

        public static Constant get(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            return new Core.Javascript.String(lex.Buffer.ToString());
        }

        public static Constant clear(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            lex.Buffer.Clear();
            return Static.Undefined;
        }
    }
}