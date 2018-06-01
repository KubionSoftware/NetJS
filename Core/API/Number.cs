using NetJS.Core.Javascript;
using System;
using System.Globalization;

namespace NetJS.Core.API {

    class Number {

        public static Constant constructor(Constant _this, Constant[] arguments, Scope scope) {
            var value = arguments.Length == 1 ? Tool.GetArgument<Javascript.Number>(arguments, 0, "Number constructor").Value : 0;

            return new Javascript.Number(value);
        }

        public static Constant toString(Constant _this, Constant[] arguments, Scope scope) {
            return new Javascript.String(((Javascript.Number)_this).Value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
