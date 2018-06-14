using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class PostfixIncrement : UnaryLeftOperator {
        public PostfixIncrement() : base(15) { }

        public override Constant Evaluate(Constant lhs, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-postfix-increment-operator

            var oldValue = Convert.ToNumber(References.GetValue(lhs, agent), agent);
            var newValue = oldValue + 1;
            References.PutValue(lhs, new Number(newValue), agent);
            return new Number(oldValue);
        }

        public override string ToDebugString() {
            return "postfix increment";
        }
    }

    public class PostfixDecrement : UnaryLeftOperator {
        public PostfixDecrement() : base(15) { }

        public override Constant Evaluate(Constant lhs, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-postfix-decrement-operator

            var oldValue = Convert.ToNumber(References.GetValue(lhs, agent), agent);
            var newValue = oldValue - 1;
            References.PutValue(lhs, new Number(newValue), agent);
            return new Number(oldValue);
        }

        public override string ToDebugString() {
            return "postfix decrement";
        }
    }

    public class PrefixIncrement : UnaryRightOperator {
        public PrefixIncrement() : base(14) { }

        public override Constant Evaluate(Constant expr, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-prefix-increment-operator

            var oldValue = Convert.ToNumber(References.GetValue(expr, agent), agent);
            var newValue = oldValue + 1;
            References.PutValue(expr, new Number(newValue), agent);
            return new Number(newValue);
        }

        public override string ToDebugString() {
            return "prefix increment";
        }
    }

    public class PrefixDecrement : UnaryRightOperator {
        public PrefixDecrement() : base(14) { }
        
        public override Constant Evaluate(Constant expr, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-prefix-decrement-operator

            var oldValue = Convert.ToNumber(References.GetValue(expr, agent), agent);
            var newValue = oldValue - 1;
            References.PutValue(expr, new Number(newValue), agent);
            return new Number(newValue);
        }

        public override string ToDebugString() {
            return "prefix decrement";
        }
    }
}
