using NetJS.Core.Javascript;

namespace NetJS.Core.API {
    class Object {

        public static Constant constructor(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var thisObject = (Javascript.Object)_this;

            return Static.Undefined;
        }

        [StaticFunction]
        public static Constant keys(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var o = Tool.GetArgument<Javascript.Object>(arguments, 0, "Object.keys");
            return Tool.ToArray(o.GetKeys());
        }

        [StaticFunction]
        public static Constant values(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var o = Tool.GetArgument<Javascript.Object>(arguments, 0, "Object.values");
            var keys = o.GetKeys();
            var array = new Javascript.Array();
            foreach(var key in keys) {
                array.List.Add(o.Get(key));
            }
            return array;
        }

        public static Constant toString(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            // Because this is actually useful
            return JSON.stringify(null, new Constant[] { _this }, lex);

            // According to javascript specification
            // return new Javascript.String("[object Object]");
        }
    }
}
