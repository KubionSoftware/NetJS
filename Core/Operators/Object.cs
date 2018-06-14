using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class Delete : UnaryRightOperator {
        public Delete() : base(14) { }

        public override Constant Evaluate(Constant c, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-delete-operator

            if (c is Reference reference) {
                if (reference.IsUnresolvableReference()) {
                    Assert.IsTrue(!reference.IsStrictReference(), $"Can't delete {c.ToDebugString()} because it is undefined");
                    return Boolean.True;
                }

                if (reference.IsPropertyReference()) {
                    if (reference.IsSuperReference()) {
                        throw new ReferenceError("Can't delete a super reference");
                    }

                    var baseObj = Convert.ToObject(reference.GetBase(), agent);
                    var deleteStatus = baseObj.Delete(reference.GetReferencedName());
                    return Boolean.Create(deleteStatus);
                } else {
                    var bindings = (EnvironmentRecord)reference.GetBase();
                    return Boolean.Create(bindings.DeleteBinding(reference.GetReferencedName()));
                }
            } else {
                return Boolean.True;
            }
        }

        public override string ToDebugString() {
            return "delete";
        }
    }

    public class In : BinaryOperator {
        public In() : base(10) { }

        public override Constant Evaluate(Constant lref, Constant rref, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-relational-operators-runtime-semantics-evaluation

            var lval = References.GetValue(lref, agent);
            var rval = References.GetValue(rref, agent);

            if (rval is Object o) {
                return Boolean.Create(o.HasProperty(Convert.ToPropertyKey(lval, agent)));
            } else {
                throw new TypeError("In operator requires an object as right-hand argument");
            }
        }

        public override string ToDebugString() {
            return "in";
        }
    }
}
