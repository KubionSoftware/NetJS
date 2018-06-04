using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Access : BinaryOperator {

        // If the variable is a key like obj.a (true) or a variable like obj[a] (false)
        public bool IsKey;

        public Access(bool isKey) : base(16) {
            IsKey = isKey;
        }

        public override Constant Evaluate(Constant baseReference, Constant propertyNameReference, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-property-accessors

            var baseValue = References.GetValue(baseReference, agent);

            if (IsKey) {
                var propertyNameString = ((Javascript.String)propertyNameReference).Value;
                var bv = References.RequireObjectCoercible(baseValue);

                // TODO: strict mode
                return new Reference(bv, new String(propertyNameString), false);
            } else {
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
