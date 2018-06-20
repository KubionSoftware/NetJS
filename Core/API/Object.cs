using NetJS.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetJS.Core.API {
    class ObjectAPI {

        public static Constant constructor(Constant _this, Constant[] arguments, Agent agent) {
            var thisObject = (Object)_this;

            return Static.Undefined;
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
                array.Add(o.Get(key, agent));
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

            return obj.GetOwnProperty(key).ToObject(agent);
        }

        [StaticFunction]
        public static Constant getOwnPropertyDescriptors(Constant _this, Constant[] arguments, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-object.getownpropertydescriptors

            var o = Tool.GetArgument(arguments, 0, "Object.getOwnPropertyDescriptors");
            var obj = Convert.ToObject(o, agent);

            var descriptors = Tool.Construct("Object", agent);
            foreach (var key in obj.OwnPropertyKeys()) {
                descriptors.Set(key, obj.GetOwnProperty(key).ToObject(agent));
            }

            return descriptors;
        }

        [StaticFunction]
        public static Constant create(Constant _this, Constant[] arguments, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-object.create

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
        public static Constant defineProperties(Constant _this, Constant[] arguments, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-objectdefineproperties

            var o = Tool.GetArgument<Object>(arguments, 0, "Object.defineProperties");
            var properties = Tool.GetArgument(arguments, 1, "Object.defineProperties");

            return ObjectDefineProperties(o, properties, agent);
        }

        private static Constant ObjectDefineProperties(Object o, Constant properties, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-objectdefineproperties

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

        public static Constant hasOwnProperty(Constant _this, Constant[] arguments, Agent agent) {
            var o = _this as Object;
            var key = Tool.GetArgument(arguments, 0, "Object.hasOwnProperty");
            return Boolean.Create(o.HasOwnProperty(key));
        }

        public static Constant toString(Constant _this, Constant[] arguments, Agent agent) {
            // Because this is actually useful
            return JSONAPI.stringify(null, new Constant[] { _this }, agent);

            // According to javascript specification
            // return new String("[object Object]");
        }
    }
}
