using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {

    // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-property-attributes

    public class DataProperty : Property {

        public Constant Value;

        public bool? Writable = null;
        public bool IsWritable => Writable.HasValue && Writable.Value;

        public override Property Clone() {
            return new DataProperty() {
                Value = Value,
                Writable = Writable,
                Enumerable = Enumerable,
                Configurable = Configurable
            };
        }

        public override void SetAttributes(Property prop) {
            if (prop is DataProperty d) {
                if (d.Enumerable.HasValue) Enumerable = d.Enumerable;
                if (d.Configurable.HasValue) Configurable = d.Configurable;
                if (d.Value != null) Value = d.Value;
                if (d.Writable.HasValue) Writable = d.Writable;
            }
        }

        public override Object ToObject(Agent agent) {
            var obj = Tool.Construct("Object", agent);
            obj.Set("value", Value, agent);
            obj.Set("writable", Boolean.Create(IsWritable), agent);
            obj.Set("enumerable", Boolean.Create(IsEnumerable), agent);
            obj.Set("configurable", Boolean.Create(IsConfigurable), agent);
            return obj;
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

        public override void SetAttributes(Property prop) {
            if (prop is AccessorProperty a) {
                if (a.Enumerable.HasValue) Enumerable = a.Enumerable;
                if (a.Configurable.HasValue) Configurable = a.Configurable;
                if (a.Get != null) Get = a.Get;
                if (a.Set != null) Set = a.Set;
            }
        }

        public override Object ToObject(Agent agent) {
            var obj = Tool.Construct("Object", agent);
            obj.Set("get", Get, agent);
            obj.Set("set", Set, agent);
            obj.Set("enumerable", Boolean.Create(IsEnumerable), agent);
            obj.Set("configurable", Boolean.Create(IsConfigurable), agent);
            return obj;
        }
    }

    public abstract class Property {
        public bool? Enumerable = null;
        public bool? Configurable = null;

        public bool IsEnumerable => Enumerable.HasValue && Enumerable.Value;
        public bool IsConfigurable => Configurable.HasValue && Configurable.Value;

        public abstract Property Clone();
        public abstract void SetAttributes(Property prop);
        public abstract Object ToObject(Agent agent);
        
        public static Property FromObject(Object obj, Agent agent) {
            Property prop;

            // TODO: handle non-existing properties better

            if (obj.HasOwnProperty("value")) {
                var dataProp = new DataProperty() {
                    Value = obj.Get("value", agent)
                };

                if (obj.HasOwnProperty("writable")) {
                    dataProp.Writable = (obj.Get("writable", agent) as Boolean).Value;
                }
                prop = dataProp;
            } else {
                prop = new AccessorProperty() {
                    Get = obj.Get("get", agent) as Function,
                    Set = obj.Get("set", agent) as Function
                };
            }

            if (obj.HasOwnProperty("enumerable")) {
                prop.Enumerable = (obj.Get("enumerable", agent) as Boolean).Value;
            }

            if (obj.HasOwnProperty("configurable")) {
                prop.Configurable = (obj.Get("configurable", agent) as Boolean).Value;
            }

            return prop;
        }
    }

    public class Object : Constant {
        private ConcurrentDictionary<Constant, Property> _properties = new ConcurrentDictionary<Constant, Property>();
        private List<Constant> _keys = new List<Constant>();

        // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-ordinary-object-internal-methods-and-internal-slots

        public Object Prototype;
        public bool Extensible;

        private void AddProperty(Constant key, Property prop) {
            _properties[key] = prop;
            _keys.Add(key);
        }

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

            Property prop;
            if (!_properties.TryGetValue(p, out prop)) return null;
            return _properties[p].Clone(); 
        }

        public bool HasOwnProperty(Constant p) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-hasownproperty

            Assert.IsPropertyKey(p);

            var desc = GetOwnProperty(p);
            return desc != null;
        }

        public bool HasOwnProperty(string p) {
            return HasOwnProperty(new String(p));
        }

        public bool ValidateAndApplyPropertyDescriptor(Constant p, bool extensible, Property desc, Property current) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-validateandapplypropertydescriptor

            Assert.IsPropertyKey(p);

            if(current == null) {
                if (!extensible) return false;

                AddProperty(p, desc);
                return true;
            }

            // TODO: every field absent

            if (!current.IsConfigurable) {
                if (desc.IsConfigurable) return false;
                if (desc.Enumerable != current.Enumerable) return false;
            }

            // TODO: generic data descriptor
            if ((current is DataProperty) != (desc is DataProperty)){
                if (!current.IsConfigurable) return false;
                if (current is DataProperty) {
                    AddProperty(p, new AccessorProperty() {
                        Get = null,
                        Set = null,
                        Configurable = current.Configurable,
                        Enumerable = current.Enumerable
                    });
                } else {
                    AddProperty(p, new DataProperty() {
                        Value = Static.Undefined,
                        Writable = false,
                        Configurable = current.Configurable,
                        Enumerable = current.Enumerable
                    });
                }
            } else if (current is DataProperty cd && desc is DataProperty dd) {
                if (!cd.IsConfigurable && !cd.IsWritable) {
                    if (dd.IsWritable) return true;
                    if (dd.Value != null && !Compare.SameValue(dd.Value, cd.Value)) return false;
                    return true;
                }
            } else if (current is AccessorProperty ca && desc is AccessorProperty da) {
                if (!ca.IsConfigurable) {
                    if (da.Set != null && !Compare.SameValue(da.Set, ca.Set)) return false;
                    if (da.Get != null && !Compare.SameValue(da.Get, ca.Get)) return false;
                    return true;
                }
            }

            _properties[p].SetAttributes(desc);

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

        public virtual Constant Get(Constant p, Agent agent, Constant receiver = null, int depth = 0) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-ordinaryget

            Assert.IsPropertyKey(p);

            if (receiver == null) receiver = this;

            var desc = GetOwnProperty(p);
            if (desc == null) {
                var parent = GetPrototypeOf();
                if (parent == null) return Static.Undefined;
                if (depth > 20) {
                    throw new Error("Object prototype loop");
                }
                return parent.Get(p, agent, receiver, depth + 1);
            }

            if (desc is DataProperty dp) return dp.Value;

            var ap = (AccessorProperty)desc;
            var getter = ap.Get;
            if (getter == null) return Static.Undefined;
            return getter.Call(receiver, agent);
        }

        public Constant Get(string p, Agent agent) {
            return Get(new String(p), agent);
        }

        public virtual bool Set(Constant p, Constant v, Agent agent, Constant receiver = null, int depth = 0) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-set-o-p-v-throw

            Assert.IsPropertyKey(p);

            if (receiver == null) receiver = this;

            var ownDesc = GetOwnProperty(p);
            if (ownDesc == null) {
                var parent = GetPrototypeOf();
                if (parent != null) {
                    if (depth > 20) {
                        throw new Error("Object prototype loop");
                    }
                    return parent.Set(p, v, agent, receiver, depth + 1);
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
                if (!data.IsWritable) return false;
                if (receiver is Object receiverObject) {
                    var existingDescriptor = receiverObject.GetOwnProperty(p);
                    if (existingDescriptor != null) {
                        if (existingDescriptor is AccessorProperty) return false;
                        var existingData = (DataProperty)existingDescriptor;
                        if (!existingData.IsWritable) return false;
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
            setter.Call(receiver, agent, new Constant[] { v });
            return true;
        }

        public bool Set(string p, Constant v, Agent agent) {
            return Set(new String(p), v, agent);
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
            if (desc.IsConfigurable) {
                _properties.TryRemove(p, out Property removed);
                _keys.Remove(p);
                return true;
            }

            return false;
        }

        public List<Constant> OwnPropertyKeys() {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-ordinaryownpropertykeys

            return _keys;
        }

        public static Object CreateFromConstructor(Function constructor, string intrinsicDefaultProto) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-ordinarycreatefromconstructor

            var proto = GetPrototypeFromConstructor(constructor, intrinsicDefaultProto);
            return new Object(proto);
        }

        public static Object GetPrototypeFromConstructor(Function constructor, string intrinsicDefaultProto) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-getprototypefromconstructor

            var proto = constructor.Get(new String("prototype"), null);
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
