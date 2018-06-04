using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {

    // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-property-attributes

    public class DataProperty : Property {

        public Constant Value;
        public bool Writable;

        public override Property Clone() {
            return new DataProperty() {
                Value = Value,
                Writable = Writable,
                Enumerable = Enumerable,
                Configurable = Configurable
            };
        }
    }

    public class AccessorProperty : Property {

        public Function Get;
        public Function Set;

        public override Property Clone() {
            return new AccessorProperty() {
                Get = Get,
                Set = Set,
                Enumerable = Enumerable,
                Configurable = Configurable
            };
        }
    }

    public abstract class Property {
        public bool Enumerable;
        public bool Configurable;

        public abstract Property Clone();
    }

    public class Object : Constant {
        private Dictionary<Constant, Property> Properties = new Dictionary<Constant, Property>();

        // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-ordinary-object-internal-methods-and-internal-slots

        public Object Prototype;
        public bool Extensible;

        public Object (Object proto) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-objectcreate

            // TODO: internalSlotsList

            Prototype = proto;
            Extensible = true;
        }

        public Object GetPrototypeOf() {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-ordinarygetprototypeof
            return Prototype;
        }

        public bool SetPrototypeOf(Object v) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-ordinarysetprototypeof

            var extensible = Extensible;
            var current = Prototype;

            if (Compare.SameValue(v, current)) return true;
            if (!extensible) return false;

            var p = v;
            var done = false;
            while (!done) {
                if (p == null) {
                    done = true;
                } else if (Compare.SameValue(p, this)) {
                    return false;
                } else {
                    // TODO: If p.[[GetPrototypeOf]] is not the ordinary object internal method defined in 9.1.1, set done to true.
                    p = p.Prototype;
                }
            }

            Prototype = v;
            return true;
        }

        public bool IsExtensible() {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-ordinaryisextensible

            return Extensible;
        }

        public bool PreventExtensions() {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-ordinarypreventextensions

            Extensible = false;
            return true;
        }

        public Property GetOwnProperty(Constant p) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-ordinarygetownproperty

            Assert.IsPropertyKey(p);
            if (!Properties.ContainsKey(p)) return null;
            return Properties[p].Clone(); 
        }

        public bool HasOwnProperty(Constant p) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-hasownproperty

            Assert.IsPropertyKey(p);

            var desc = GetOwnProperty(p);
            return desc != null;
        }

        public bool ValidateAndApplyPropertyDescriptor(Constant p, bool extensible, Property desc, Property current) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-validateandapplypropertydescriptor

            Assert.IsPropertyKey(p);

            if(current == null) {
                if (!extensible) return false;

                Properties[p] = desc;
                return true;
            }

            // TODO: every field absent

            if (!current.Configurable) {
                if (desc.Configurable) return false;
                if (desc.Enumerable != current.Enumerable) return false;
            }

            // TODO: generic data descriptor
            if ((current is DataProperty) != (desc is DataProperty)){
                if (!current.Configurable) return false;
                if (current is DataProperty) {
                    Properties[p] = new AccessorProperty() {
                        Get = null,
                        Set = null,
                        Configurable = current.Configurable,
                        Enumerable = current.Enumerable
                    };
                } else {
                    Properties[p] = new DataProperty() {
                        Value = Static.Undefined,
                        Writable = false,
                        Configurable = current.Configurable,
                        Enumerable = current.Enumerable
                    };
                }
            } else if (current is DataProperty cd && desc is DataProperty dd) {
                if (!cd.Configurable && !cd.Writable) {
                    if (dd.Writable) return true;
                    if (dd.Value != null && !Compare.SameValue(dd.Value, cd.Value)) return false;
                    return true;
                }
            } else if (current is AccessorProperty ca && desc is AccessorProperty da) {
                if (!ca.Configurable) {
                    if (da.Set != null && !Compare.SameValue(da.Set, ca.Set)) return false;
                    if (da.Get != null && !Compare.SameValue(da.Get, ca.Get)) return false;
                    return true;
                }
            }

            Properties[p] = desc;
            return true;
        }

        public bool DefineOwnProperty(Constant p, Property desc) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-ordinarydefineownproperty

            var current = GetOwnProperty(p);
            var extensible = Extensible;
            return ValidateAndApplyPropertyDescriptor(p, extensible, desc, current);
        }

        public bool HasProperty(Constant p) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-ordinaryhasproperty

            Assert.IsPropertyKey(p);
            var hasOwn = GetOwnProperty(p);
            if (hasOwn != null) return true;

            var parent = GetPrototypeOf();
            if (parent != null) return parent.HasProperty(p);
            return false;
        }

        public Constant Get(Constant p, Constant receiver = null) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-ordinaryget

            Assert.IsPropertyKey(p);

            if (receiver == null) receiver = this;

            var desc = GetOwnProperty(p);
            if (desc == null) {
                var parent = GetPrototypeOf();
                if (parent == null) return Static.Undefined;
                return parent.Get(p, receiver);
            }

            if (desc is DataProperty dp) return dp.Value;

            var ap = (AccessorProperty)desc;
            var getter = ap.Get;
            if (getter == null) return Static.Undefined;
            return getter.Call(receiver);
        }

        public bool Set(Constant p, Constant v, Constant receiver = null) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-set-o-p-v-throw

            Assert.IsPropertyKey(p);

            if (receiver == null) receiver = this;

            var ownDesc = GetOwnProperty(p);
            if (ownDesc == null) {
                var parent = GetPrototypeOf();
                if (parent != null) {
                    return parent.Set(p, v, receiver);
                } else {
                    ownDesc = new DataProperty() {
                        Value = null,
                        Writable = true,
                        Enumerable = true,
                        Configurable = true
                    };
                }
            }

            if (ownDesc is DataProperty data) {
                if (!data.Writable) return false;
                if (receiver is Object receiverObject) {
                    var existingDescriptor = receiverObject.GetOwnProperty(p);
                    if (existingDescriptor != null) {
                        if (existingDescriptor is AccessorProperty) return false;
                        var existingData = (DataProperty)existingDescriptor;
                        if (!existingData.Writable) return false;
                        var valueDesc = new DataProperty() { Value = v };
                        return receiverObject.DefineOwnProperty(p, valueDesc);
                    } else {
                        return receiverObject.CreateDataProperty(p, v);
                    }
                } else {
                    return false;
                }
            }

            var accessor = (AccessorProperty)ownDesc;
            var setter = accessor.Set;
            if (setter == null) return false;
            setter.Call(receiver, new Constant[] { v });
            return true;
        }

        public bool CreateDataProperty(Constant p, Constant v) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-createdataproperty

            Assert.IsPropertyKey(p);
            var newDesc = new DataProperty() {
                Value = v,
                Writable = true,
                Enumerable = true,
                Configurable = true
            };
            return DefineOwnProperty(p, newDesc);
        }

        public bool Delete(Constant p) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-ordinarydelete

            Assert.IsPropertyKey(p);

            var desc = GetOwnProperty(p);
            if (desc == null) return true;
            if (desc.Configurable) {
                Properties.Remove(p);
                return true;
            }

            return false;
        }

        public List<Constant> OwnPropertyKeys() {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-ordinaryownpropertykeys

            var keys = new List<Constant>();

            // TODO: correct order
            foreach (var p in Properties.Keys) {
                keys.Add(p);
            }

            return keys;
        }

        public static Object CreateFromConstructor(Function constructor, string intrinsicDefaultProto) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-ordinarycreatefromconstructor

            var proto = GetPrototypeFromConstructor(constructor, intrinsicDefaultProto);
            return new Object(proto);
        }

        public static Object GetPrototypeFromConstructor(Function constructor, string intrinsicDefaultProto) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-getprototypefromconstructor

            var proto = constructor.Get(new String("prototype"));
            if (!(proto is Object)) {
                var realm = constructor.Realm;
                proto = realm.GetPrototype(intrinsicDefaultProto);
            }
            return (Object)proto;
        }

        public bool DefinePropertyOrThrow(Constant p, Property desc) {
            Assert.IsPropertyKey(p);

            var success = DefineOwnProperty(p, desc);
            if (!success) throw new TypeError($"Could not define property {p.ToDebugString()}");
            return success;
        }

        public override string ToDebugString() {
            return "{}";

            // TODO: fix this
            //return $"{{\n{string.Join(",\n", Properties.Select(pair => pair.Key + ": " + pair.Value.ToDebugString()))}\n}}";
        }
    }
}
