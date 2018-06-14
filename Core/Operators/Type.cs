using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {

    public class TypeOf : UnaryRightOperator {
        public TypeOf() : base(14) { }

        public override Constant Evaluate(Constant val, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-typeof-operator

            if (val is Reference r) {
                if (r.IsUnresolvableReference()) return new String("undefined");
            }

            val = References.GetValue(val, agent);
            
            switch (val) {
                case Undefined u:   return new String("undefined");
                case Null n:        return new String("object");
                case Boolean b:     return new String("boolean");
                case Number nr:     return new String("number");
                case String s:      return new String("string");
                case Symbol sy:     return new String("symbol");
                case Function f:    return new String("function");
                case Object o:      return new String("object");
            }

            throw new TypeError($"Can't get type of {val.ToDebugString()}");
        }

        public override string ToDebugString() {
            return "typeof";
        }
    }

    public class InstanceOf : BinaryOperator {
        public InstanceOf() : base(11) { }

        public override Constant Evaluate(Constant lref, Constant rref, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-instanceofoperator

            var lval = References.GetValue(lref, agent);
            var rval = References.GetValue(rref, agent);

            return Boolean.Create(Tool.IsType(lval, rval));
        }

        public override string ToDebugString() {
            return "instanceof";
        }
    }
}
