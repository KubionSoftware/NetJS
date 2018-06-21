using NetJS.Core;
using System;
using System.Globalization;

namespace NetJS.Core.API {

    class NumberAPI {

        private const string Primitive = "[[PrimitiveValue]]";

        public static Constant constructor(Constant _this, Constant[] arguments, Agent agent) {
            var value = arguments.Length == 1 ? Tool.GetArgument<Number>(arguments, 0, "Number constructor") : new Number(0);
            var obj = _this as Object;
            obj.Set(Primitive, value, agent);
            return _this;
        }

        private static Number GetNumber(Constant _this, Agent agent) {
            if (_this is Number s) return s;
            return (_this as Object).Get(Primitive, agent) as Number;
        }

        public static Constant toString(Constant _this, Constant[] arguments, Agent agent) {
            var num = GetNumber(_this, agent);
            return new String(num.Value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
