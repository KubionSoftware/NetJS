using System;
using System.Collections.Generic;
using NetJS.Core;
using System.Text;

namespace NetJS.Core.API {
    class ArrayAPI {

        public static Constant constructor(Constant _this, Constant[] arguments, Agent agent) {
            var length = arguments.Length == 1 ? (int)Tool.GetArgument<Number>(arguments, 0, "Array constructor").Value : 0;
            return new Array(length, agent);
        }

        private static Array GetArray(Constant _this) {
            return (Array)_this;
        }

        [StaticFunction]
        public static Constant isArray(Constant _this, Constant[] arguments, Agent agent) {
            var o = Tool.GetArgument(arguments, 0, "Array.isArray");
            return Boolean.Create(o is Array);
        }

        public static Constant forEach(Constant _this, Constant[] arguments, Agent agent) {
            var array = GetArray(_this);

            if (arguments.Length != 1) {
                throw new TypeError("Array.forEach takes one argument");
            }

            var callback = (Function)arguments[0];

            for (int i = 0; i < array.List.Count; i++) {
                var callbackArguments = new Constant[] {
                    array.List[i],
                    new Number(i),
                    array
                };
                callback.Call(Static.Undefined, agent, callbackArguments);
            }

            return Static.Undefined;
        }

        public static Constant push(Constant _this, Constant[] arguments, Agent agent) {
            var array = GetArray(_this);

            array.AddRange(arguments, agent);
            array.Set("length", new Number(array.List.Count), agent);

            return Static.Undefined;
        }

        public static Constant pop(Constant _this, Constant[] arguments, Agent agent) {
            var array = GetArray(_this);
            
            if (array.List.Count == 0) return Static.Undefined;

            var element = array.List[array.List.Count - 1];
            array.RemoveAt(array.List.Count - 1, agent);

            return element;
        }

        public static Constant shift(Constant _this, Constant[] arguments, Agent agent) {
            var array = GetArray(_this);

            if (array.List.Count == 0) return Static.Undefined;

            var element = array.List[0];
            array.RemoveAt(0, agent);

            return element;
        }

        public static Constant unshift(Constant _this, Constant[] arguments, Agent agent) {
            var array = GetArray(_this);

            array.InsertRange(0, arguments, agent);

            return Static.Undefined;
        }

        public static Constant indexOf(Constant _this, Constant[] arguments, Agent agent) {
            var array = GetArray(_this);

            var start = arguments.Length > 1 ? (int)((Number)arguments[1]).Value : 0;
            
            for(var i = start; i < array.List.Count; i++) {
                var element = array.List[i];
                var equals = Compare.StrictEqualityComparison(element, arguments[0]);
                if (equals) {
                    return new Number(i);
                }
            }

            return new Number(-1);
        }

        public static Constant splice(Constant _this, Constant[] arguments, Agent agent) {
            var array = GetArray(_this);
            
            var start = arguments.Length > 0 ? (int)Tool.GetArgument<Number>(arguments, 0, "Array.splice").Value : 0;
            var count = (int)(arguments.Length > 1 ? ((Number)arguments[1]).Value : array.List.Count - start);

            // Remove items
            var result = new Array(0, agent);
            result.AddRange(array.GetRange(start, count), agent);
            array.RemoveRange(start, count, agent);

            // Add items
            var addCount = arguments.Length <= 2 ? 0 : arguments.Length - 2;
            for (var i = 0; i < addCount; i++) {
                array.Insert(start + i, arguments[i + 2], agent);
            }

            return result;
        }

        public static Constant slice(Constant _this, Constant[] arguments, Agent agent) {
            var array = GetArray(_this);
            
            var begin = (int)(arguments.Length > 0 ? ((Number)arguments[0]).Value : 0);
            var end = (int)(arguments.Length > 1 ? ((Number)arguments[1]).Value : array.List.Count);

            var result = new Array(0, agent);
            for (int i = begin; i < end; i++) {
                result.Add(array.List[i], agent);
            }
            
            return result;
        }

        public static Constant map(Constant _this, Constant[] arguments, Agent agent) {
            var array = GetArray(_this);

            if (arguments.Length != 1) {
                throw new TypeError("Array.map takes one argument");
            }

            var callback = (Function)arguments[0];

            var result = new Array(0, agent);
            for (int i = 0; i < array.List.Count; i++) {
                var callbackArguments = new Constant[] {
                    array.List[i],
                    new Number(i),
                    array
                };
                var value = callback.Call(Static.Undefined, agent, callbackArguments);
                result.Add(value, agent);
            }

            return result;
        }

        public static Constant filter(Constant _this, Constant[] arguments, Agent agent) {
            var array = GetArray(_this);

            if (arguments.Length == 0 || arguments.Length > 2) {
                throw new TypeError("Array.filter takes one or two argument");
            }

            var callback = (Function)arguments[0];

            var result = new Array(0, agent);
            for (int i = 0; i < array.List.Count; i++) {
                var element = array.List[i];

                var callbackArguments = new Constant[] {
                    element,
                    new Number(i),
                    array
                };

                var value = callback.Call(arguments.Length == 1 ? Static.Undefined : arguments[1], agent, callbackArguments);
                if (value is Boolean) {
                    if (((Boolean)value).Value) {
                        result.Add(element, agent);
                    }
                }
            }

            return result;
        }

        public static Constant reduce(Constant _this, Constant[] arguments, Agent agent) {
            var array = GetArray(_this);

            if (arguments.Length < 1) {
                throw new TypeError("Array.reduce takes at least one argument");
            }

            var callback = (Function)arguments[0];

            Constant accumulator;
            if(arguments.Length > 1) {
                accumulator = arguments[1];
            } else {
                if (array.List.Count == 0) throw new TypeError("Array.reduce: No initial value is supplied and array is empty");
                accumulator = array.List[0];
            }
            
            for (int i = arguments.Length > 1 ? 0 : 1; i < array.List.Count; i++) {
                var callbackArguments = new Constant[] {
                    accumulator,
                    array.List[i],
                    new Number(i),
                    array
                };
                accumulator = callback.Call(Static.Undefined, agent, callbackArguments);
            }

            return accumulator;
        }

        private static Constant checkAll(Constant _this, Constant[] arguments, Agent agent, string name, Func<bool, Constant, int, Constant> resultFunc, Constant final) {
            var array = GetArray(_this);

            if (arguments.Length == 0 || arguments.Length > 2) {
                throw new TypeError(name + " takes one or two argument");
            }

            var callback = (Function)arguments[0];

            for (int i = 0; i < array.List.Count; i++) {
                var element = array.List[i];

                var callbackArguments = new Constant[] {
                    element,
                    new Number(i),
                    array
                };

                var value = callback.Call(arguments.Length == 1 ? Static.Undefined : arguments[1], agent, callbackArguments);
                if (value is Boolean) {
                    var boolValue = ((Boolean)value).Value;
                    var result = resultFunc(boolValue, element, i);
                    if (result != null) return result;
                }
            }

            return final;
        }

        public static Constant some(Constant _this, Constant[] arguments, Agent agent) {
            return checkAll(
                _this, 
                arguments, 
                agent,
                "Array.some",
                (value, element, index) => value ? Boolean.True : null, 
                Boolean.False
            );
        }

        public static Constant every(Constant _this, Constant[] arguments, Agent agent) {
            return checkAll(
                _this, 
                arguments, 
                agent,
                "Array.every",
                (value, element, index) => !value ? Boolean.False : null,
                Boolean.True
            );
        }

        public static Constant find(Constant _this, Constant[] arguments, Agent agent) {
            return checkAll(
                _this,
                arguments,
                agent,
                "Array.find",
                (value, element, index) => value ? element : null,
                Static.Undefined
            );
        }

        public static Constant findIndex(Constant _this, Constant[] arguments, Agent agent) {
            return checkAll(
                _this,
                arguments,
                agent,
                "Array.findIndex",
                (value, element, index) => value ? new Number(index) : null,
                Static.Undefined
            );
        }

        public static Constant includes(Constant _this, Constant[] arguments, Agent agent) {
            var array = GetArray(_this);

            if (arguments.Length == 0 || arguments.Length > 2) {
                throw new TypeError("Array.includes takes one or two argument");
            }

            var reference = arguments[0];
            var startIndex = (int)(arguments.Length > 1 ? Tool.GetArgument<Number>(arguments, 1, "Array.includes").Value : 0);

            for (int i = startIndex; i < array.List.Count; i++) {
                var equals = Compare.AbstractEqualityComparison(array.List[i], reference, agent);
                if (equals) {
                    return Boolean.True;
                }
            }

            return Boolean.False;
        }

        public static Constant sort(Constant _this, Constant[] arguments, Agent agent) {
            var array = GetArray(_this);

            if (arguments.Length != 1) {
                throw new TypeError("Array.sort takes one argument");
            }

            var callback = (Function)arguments[0];

            var n = array.List.Count;
            while (true) {
                var swapped = false;

                for(var i = 1; i < n; i++) {
                    var aElement = array.List[i - 1];
                    var bElement = array.List[i];

                    var callbackArguments = new Constant[] { aElement, bElement };
                    var value = (Number)callback.Call(Static.Undefined, agent, callbackArguments);

                    if(value.Value > 0) {
                        array.Set(i - 1, bElement);
                        array.Set(i, aElement);
                        swapped = true;
                    }
                }

                n--;

                if (!swapped) break;
            }

            return array;
        }

        public static Constant join(Constant _this, Constant[] arguments, Agent agent) {
            var array = GetArray(_this);

            var seperator = arguments.Length > 0 ? ((String)arguments[0]).Value : ",";

            var result = new StringBuilder();
            for (int i = 0; i < array.List.Count; i++) {
                if(i > 0) {
                    result.Append(seperator);
                }

                result.Append(Convert.ToString(array.List[i], agent));
            }

            return new String(result.ToString());
        }

        public static Constant reverse(Constant _this, Constant[] arguments, Agent agent) {
            var array = GetArray(_this);

            array.Reverse();

            return array;
        }

        public static Constant fill(Constant _this, Constant[] arguments, Agent agent) {
            var array = GetArray(_this);

            var value = Tool.GetArgument(arguments, 0, "Array.fill");
            var start = Tool.GetArgument<Number>(arguments, 1, "Array.fill", false) ?? new Number(0);
            var end = Tool.GetArgument<Number>(arguments, 2, "Array.fill", false) ?? new Number(array.List.Count);

            for(var i = (int)start.Value; i < (int)end.Value; i++) {
                array.Set(i, value);
            }

            return array;
        }

        public static Constant toString(Constant _this, Constant[] arguments, Agent agent) {
            // Because this is actually useful
            return JSONAPI.stringify(Static.Undefined, new Constant[] { _this }, agent);

            // According to javascript specification
            // return join(_this, arguments, agent);
        }
    }
}
