using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class Access : BinaryOperator {

        // If the variable is a key like obj.a (true) or a variable like obj[a] (false)
        public bool IsKey;

        public Access(bool isKey) : base(16) {
            IsKey = isKey;
        }

        public override Constant Evaluate(Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-property-accessors

            var baseReference = Left.Evaluate(agent);
            var baseValue = References.GetValue(baseReference, agent);

            if (IsKey) {
                if (Right is Identifier id && id.Name is String propertyNameString) {
                    var bv = References.RequireObjectCoercible(baseValue);

                    // TODO: strict mode
                    return new Reference(bv, propertyNameString, false);
                } else {
                    throw new SyntaxError("Access key is not a string");
                }
            } else {
                var propertyNameReference = Right.Evaluate(agent);
                var propertyNameValue = References.GetValue(propertyNameReference, agent);

                var bv = References.RequireObjectCoercible(baseValue);
                var propertyKey = Convert.ToPropertyKey(propertyNameValue, agent);

                // TODO: strict mode
                return new Reference(bv, propertyKey, false);
            }
        }

        public override string ToDebugString() {
            return "access";
        }
    }
}
