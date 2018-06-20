using NetJS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    class References {

        public static Constant GetValue(Constant v, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-getvalue

            if (v is Reference r) {
                var b = r.GetBase();

                if (r.IsUnresolvableReference()) {
                    // TODO: error message
                    throw new ReferenceError(r.GetReferencedName().ToDebugString() + " is undefined");
                }

                if (r.IsPropertyReference()) {
                    if (r.HasPrimitiveBase()) {
                        // Assert base is not undefined or null
                        b = Convert.ToObject(b, agent);
                    }

                    return ((Object)b).Get(r.GetReferencedName(), agent);
                } else if (b is ExoticConstant e) {
                    return e.GetProperty(r.GetReferencedName(), agent);
                } else {
                    var record = (EnvironmentRecord)b;
                    return record.GetBindingValue(r.GetReferencedName(), r.IsStrictReference(), agent);
                }
            } else {
                return v;
            }
        }

        public static Completion PutValue(Constant v, Constant w, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-putvalue

            if (v is Reference r) {
                var b = r.GetBase();

                if (r.IsUnresolvableReference()) {
                    if (r.IsStrictReference()) {
                        throw new ReferenceError("Can't assign value to undefined in strict mode");
                    }

                    var globalObj = agent.Running.GetGlobalObject();
                    var succeeded = globalObj.Set(r.GetReferencedName(), w);
                    return new Completion(CompletionType.Normal, Boolean.Create(succeeded));
                } else if (r.IsPropertyReference()) {
                    if (r.HasPrimitiveBase()) {
                        // Assert base will never be undefined or null
                        b = Convert.ToObject(b, agent);
                    }

                    // TODO: handle succeeded + this value
                    var succeeded = ((Object)b).Set(r.GetReferencedName(), w);
                    return new Completion(CompletionType.Normal, Boolean.Create(succeeded));
                } else if (b is ExoticConstant e) {
                    e.SetProperty(r.GetReferencedName(), w, agent);
                    return Static.NormalCompletion;
                } else {
                    var record = (EnvironmentRecord)b;
                    return record.SetMutableBinding(r.GetReferencedName(), w, r.IsStrictReference(), agent);
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

        public static Reference GetIdentifierReference(LexicalEnvironment lex, Constant name, bool isStrict, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-getidentifierreference

            if (lex == null) {
                return new Reference(Static.Undefined, name, isStrict);
            }

            var envRec = lex.Record;
            var exists = envRec.HasBinding(name, agent);

            if (exists) {
                return new Reference(envRec, name, isStrict);
            } else {
                var outer = lex.Outer;
                return GetIdentifierReference(outer, name, isStrict, agent);
            }
        }

        public static Reference ResolveBinding(Constant name, LexicalEnvironment lex, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-resolvebinding

            if (lex == null) lex = agent.Running.Lex;

            return GetIdentifierReference(lex, name, agent.Running.IsStrict, agent);
        }

        public static void InitializeReferencedBinding(Reference v, Constant w, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-initializereferencedbinding

            var b = v.GetBase();
            if (b is EnvironmentRecord e) {
                e.InitializeBinding(v.GetReferencedName(), w, agent);
            } else {
                throw new Error("Base of reference is not an environment record");
            }
        }

        public static Completion InitializeBoundName(Constant name, Constant value, LexicalEnvironment lex, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-identifiers-runtime-semantics-bindinginitialization

            if (lex != null) {
                var env = lex.Record;
                env.InitializeBinding(name, value, agent);
                return Static.NormalCompletion;
            } else {
                var lhs = ResolveBinding(name, null, agent);
                return PutValue(lhs, value, agent);
            }
        }

        public static Constant ResolvethisBinding(Agent agent) {
            var envRec = agent.Running.GetThisEnvironment();
            return envRec.GetThisBinding();
        }
    }
}
