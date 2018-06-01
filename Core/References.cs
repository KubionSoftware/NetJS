using NetJS.Core.Javascript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    class References {

        public static Constant GetValue(Constant v, Scope scope) {
            if(v is Reference r) {
                var b = r.GetBase();

                if (r.IsUnresolvableReference()) {
                    // TODO: error message
                    throw new ReferenceError("Can't get value of undefined");
                }

                if (r.IsPropertyReference()) {
                    if (r.HasPrimitiveBase()) {
                        // Assert base is not undefined or null
                        b = Convert.ToObject(b, scope);
                    }

                    return b.GetProperty(r.GetReferencedName(), scope);
                } else {
                    // TODO: base must be an environment record
                    // Return ? base.GetBindingValue(GetReferencedName(V), IsStrictReference(V)) (see 8.1.1).
                    return Static.Undefined;
                }
            } else {
                return v;
            }
        }

        public static void PutValue(Constant v, Constant w, Scope scope) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-putvalue

            if (v is Reference r) {
                var b = r.GetBase();

                if (r.IsUnresolvableReference()) {
                    if (r.IsStrictReference()) {
                        throw new ReferenceError("Can't assign value to undefined in strict mode");
                    }

                    var globalObj = scope.GetGlobalObject();
                    globalObj.SetProperty(r.GetReferencedName(), w, scope);
                    return;
                } else if (r.IsPropertyReference()) {
                    if (r.HasPrimitiveBase()) {
                        // Assert base will never be undefined or null
                        b = Convert.ToObject(b, scope);
                    }

                    // TODO: handle succeeded + this value
                    b.SetProperty(r.GetReferencedName(), w, scope);
                    return;
                } else {
                    // TODO: environment record
                }
            } else {
                throw new ReferenceError($"Can't assign value to {v.ToDebugString()}");
            }
        }

        public static Constant GetThisValue(Reference v) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-getthisvalue

            // Assert IsPropertyReference(v) is true

            if (v.IsSuperReference()) {
                return v.GetThisValue();
            }

            return v.GetBase();
        }

        public static Constant RequireObjectCoercible(Constant argument) {
            if (argument is Undefined) {
                throw new TypeError("Can't get property of undefined");
            } else if (argument is Null) {
                throw new TypeError("Can't get property of null");
            }

            return argument;
        }
    }
}
