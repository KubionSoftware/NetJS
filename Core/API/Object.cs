using NetJS.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetJS.Core.API {
    class ObjectAPI {

        public static Constant constructor(Constant _this, Constant[] arguments, Agent agent) {
            // See: http://ecma-international.org/ecma-262/#sec-object-value

            if (arguments.Length > 0) {
                var value = arguments[0];
                if (value is Null || value is Undefined) {
                    return _this;
                } else {
                    return Convert.ToObject(value, agent);
                }
            }

            return _this;
        }

        [StaticFunction]
        public static Constant keys(Constant _this, Constant[] arguments, Agent agent) {
            var o = Tool.GetArgument<Object>(arguments, 0, "Object.keys");
            return Tool.ToArray(o.OwnPropertyKeys().Select(key => key.ToString()), agent);
        }

        [StaticFunction]
        public static Constant values(Constant _this, Constant[] arguments, Agent agent) {
            var o = Tool.GetArgument<Object>(arguments, 0, "Object.values");
            var keys = o.OwnPropertyKeys();
            var array = new Array(0, agent);
            foreach(var key in keys) {
                array.Add(o.Get(key, agent), agent);
            }
            return array;
        }

        [StaticFunction]
        public static Constant getOwnPropertyDescriptor(Constant _this, Constant[] arguments, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-object.getownpropertydescriptor

            var o = Tool.GetArgument(arguments, 0, "Object.getOwnPropertyDescriptor");
            var obj = Convert.ToObject(o, agent);

            var p = Tool.GetArgument(arguments, 1, "Object.getOwnPropertyDescriptor");
            var key = Convert.ToPropertyKey(p, agent);

            var desc = obj.GetOwnProperty(key);
            return desc == null ? Static.Undefined : (Constant)desc.ToObject(agent);
        }

        [StaticFunction]
        public static Constant getOwnPropertyDescriptors(Constant _this, Constant[] arguments, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-object.getownpropertydescriptors

            var o = Tool.GetArgument(arguments, 0, "Object.getOwnPropertyDescriptors");
            var obj = Convert.ToObject(o, agent);

            var descriptors = Tool.Construct("Object", agent);
            foreach (var key in obj.OwnPropertyKeys()) {
                var desc = obj.GetOwnProperty(key);
                var value = desc == null ? Static.Undefined : (Constant)desc.ToObject(agent);
                descriptors.Set(key, value, agent);
            }

            return descriptors;
        }

        [StaticFunction]
        public static Constant getOwnPropertyNames(Constant _this, Constant[] arguments, Agent agent) {
            // See: http://ecma-international.org/ecma-262/#sec-object.getownpropertynames

            var o = Tool.GetArgument(arguments, 0, "Object.getOwnPropertyNames");
            return GetOwnPropertyKeys(o, typeof(String), agent);
        }

        [StaticFunction]
        public static Constant getOwnPropertySymbols(Constant _this, Constant[] arguments, Agent agent) {
            // See: http://ecma-international.org/ecma-262/#sec-object.getownpropertysymbols

            var o = Tool.GetArgument(arguments, 0, "Object.getOwnPropertySymbols");
            return GetOwnPropertyKeys(o, typeof(Symbol), agent);
        }

        public static Constant GetOwnPropertyKeys(Constant o, System.Type type, Agent agent) {
            // http://ecma-international.org/ecma-262/#sec-getownpropertykeys

            var obj = Convert.ToObject(o, agent);

            var keys = obj.OwnPropertyKeys();
            var nameList = new List<Constant>();

            foreach (var nextKey in keys) {
                if (nextKey.GetType() == type) {
                    nameList.Add(nextKey);
                }
            }

            return Tool.ToArray(nameList, agent);
        }

        [StaticFunction]
        public static Constant assign(Constant _this, Constant[] arguments, Agent agent) {
            // See: http://ecma-international.org/ecma-262/#sec-object.assign

            var to = Convert.ToObject(Tool.GetArgument(arguments, 0, "Object.assign"), agent);

            if (arguments.Length == 1) return to;

            for (var i = 1; i < arguments.Length; i++) {
                var nextSource = arguments[i];

                if (nextSource is Undefined || nextSource is Null) {
                    continue;
                }

                var from = Convert.ToObject(nextSource, agent);
                var keys = from.OwnPropertyKeys();
                foreach (var nextKey in keys) {
                    var desc = from.GetOwnProperty(nextKey);
                    if (desc != null && desc.IsEnumerable) {
                        var propValue = from.Get(nextKey, agent);
                        to.Set(nextKey, propValue, agent);
                    }
                }
            }

            return to;
        }

        [StaticFunction]
        public static Constant create(Constant _this, Constant[] arguments, Agent agent) {
            // See: http://ecma-international.org/ecma-262/#sec-object.create

            var arg = Tool.GetArgument(arguments, 0, "Object.create");
            Object o;
            if (arg is Object oo) {
                o = oo;
            } else if (arg is Null) {
                o = null;
            } else {
                throw new TypeError("Object.create requires an object or null as first argument");
            }

            var obj = new Object(o);

            if (arguments.Length > 0) {
                ObjectDefineProperties(obj, arguments[0], agent);
            }

            return obj;
        }

        [StaticFunction]
        public static Constant @is(Constant _this, Constant[] arguments, Agent agent) {
            // See: http://ecma-international.org/ecma-262/#sec-object.is

            var value1 = Tool.GetArgument(arguments, 0, "Object.is");
            var value2 = Tool.GetArgument(arguments, 1, "Object.is");

            return Boolean.Create(Compare.SameValue(value1, value2));
        }

        [StaticFunction]
        public static Constant defineProperties(Constant _this, Constant[] arguments, Agent agent) {
            // See: http://ecma-international.org/ecma-262/#sec-object.defineproperties

            var o = Tool.GetArgument<Object>(arguments, 0, "Object.defineProperties");
            var properties = Tool.GetArgument(arguments, 1, "Object.defineProperties");

            return ObjectDefineProperties(o, properties, agent);
        }

        private static Constant ObjectDefineProperties(Object o, Constant properties, Agent agent) {
            // See: http://ecma-international.org/ecma-262/#sec-objectdefineproperties

            var props = Convert.ToObject(properties, agent);

            var descriptors = new List<Tuple<Constant, Property>>();
            foreach (var key in props.OwnPropertyKeys()) {
                var propDesc = props.GetOwnProperty(key);
                if (propDesc != null && propDesc.IsEnumerable) {
                    var descObj = props.Get(key, agent) as Object;
                    var desc = Property.FromObject(descObj, agent);
                    descriptors.Add(new Tuple<Constant, Property>(key, desc));
                }
            }

            foreach (var pair in descriptors) {
                o.DefinePropertyOrThrow(pair.Item1, pair.Item2);
            }

            return o;
        }

        [StaticFunction]
        public static Constant defineProperty(Constant _this, Constant[] arguments, Agent agent) {
            // See: http://ecma-international.org/ecma-262/#sec-object.defineproperty

            var o = Tool.GetArgument<Object>(arguments, 0, "Object.defineProperty");
            var p = Tool.GetArgument(arguments, 1, "Object.defineProperty");
            var attributes = Tool.GetArgument<Object>(arguments, 2, "Object.defineProperty");

            var key = Convert.ToPropertyKey(p, agent);
            var desc = Property.FromObject(attributes, agent);

            o.DefinePropertyOrThrow(key, desc);
            return o;
        }

        [StaticFunction]
        public static Constant getPrototypeOf(Constant _this, Constant[] arguments, Agent agent) {
            // See: http://ecma-international.org/ecma-262/#sec-object.getprototypeof

            var o = Tool.GetArgument(arguments, 0, "Object.getPrototypeOf");
            var obj = Convert.ToObject(o, agent);

            return obj.GetPrototypeOf();
        }

        [StaticFunction]
        public static Constant setPrototypeOf(Constant _this, Constant[] arguments, Agent agent) {
            // See: http://ecma-international.org/ecma-262/#sec-object.setprototypeof

            var o = Tool.GetArgument(arguments, 0, "Object.setPrototypeOf");
            o = References.RequireObjectCoercible(o);
            Object obj;
            if (o is Object oo) {
                obj = oo;
            } else {
                throw new TypeError("Can only set prototype of object");
            }

            var proto = Tool.GetArgument(arguments, 1, "Object.setPrototypeOf");
            Object protoObj;
            if (proto is Object po) {
                protoObj = po;
            } else { 
                if (proto is Null) {
                    protoObj = null;
                } else {
                    throw new TypeError("A prototype can only be an object or null");
                }
            }

            var status = obj.SetPrototypeOf(protoObj);
            if (!status) {
                throw new TypeError($"Can't set prototype of {o.ToDebugString()}");
            }

            return o;
        }

        public static Constant hasOwnProperty(Constant _this, Constant[] arguments, Agent agent) {
            // See: http://ecma-international.org/ecma-262/#sec-object.prototype.hasownproperty

            var v = Tool.GetArgument(arguments, 0, "Object.hasOwnProperty");
            var p = Convert.ToPropertyKey(v, agent);

            var o = Convert.ToObject(_this, agent);

            return Boolean.Create(o.HasOwnProperty(p));
        }

        public static Constant isPrototypeOf(Constant _this, Constant[] arguments, Agent agent) {
            // See: http://ecma-international.org/ecma-262/#sec-object.prototype.isprototypeof

            var v = Tool.GetArgument(arguments, 0, "Object.isPrototypeOf");

            if (v is Object proto) {
                var o = Convert.ToObject(_this, agent);

                while (true) {
                    proto = proto.GetPrototypeOf();
                    if (proto == null) return Boolean.False;
                    if (Compare.SameValue(o, proto)) return Boolean.True;
                }
            } else {
                return Boolean.False;
            }
        }

        public static Constant toString(Constant _this, Constant[] arguments, Agent agent) {
            // Because this is actually useful
            return JSONAPI.stringify(null, new Constant[] { _this }, agent);

            // According to javascript specification
            // return new String("[object Object]");
        }

        public static Constant valueOf(Constant _this, Constant[] arguments, Agent agent) {
            // See: http://ecma-international.org/ecma-262/#sec-object.prototype.valueof

            return Convert.ToObject(_this, agent);
        }
    }
}
