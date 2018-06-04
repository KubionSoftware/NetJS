using System;
using System.Collections.Generic;
using NetJS.Core.Javascript;
using System.Text;

namespace NetJS.Core.API {
    class Array {

        public static Constant constructor(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var length = arguments.Length == 1 ? (int)Tool.GetArgument<Javascript.Number>(arguments, 0, "Array constructor").Value : 0;

            return new Javascript.Array(length);
        }

        [StaticFunction]
        public static Constant isArray(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var o = Tool.GetArgument(arguments, 0, "Array.isArray");
            return new Javascript.Boolean(o is Javascript.Array);
        }

        public static Constant forEach(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var array = (Javascript.Array)_this;

            if (arguments.Length != 1) {
                throw new Javascript.TypeError("Array.forEach takes one argument");
            }

            var callback = (Javascript.Function)arguments[0];

            for (int i = 0; i < array.List.Count; i++) {
                var callbackArguments = new Constant[] {
                    array.List[i],
                    new Javascript.Number(i),
                    array
                };
                callback.Call(callbackArguments, Static.Undefined, lex);
            }

            return Static.Undefined;
        }

        public static Constant push(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var array = (Javascript.Array)_this;

            array.List.AddRange(arguments);

            return Static.Undefined;
        }

        public static Constant pop(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var array = (Javascript.Array)_this;
            
            if (array.List.Count == 0) return Static.Undefined;

            var element = array.List[array.List.Count - 1];
            array.List.RemoveAt(array.List.Count - 1);

            return element;
        }

        public static Constant shift(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var array = (Javascript.Array)_this;

            if (array.List.Count == 0) return Static.Undefined;

            var element = array.List[0];
            array.List.RemoveAt(0);

            return element;
        }

        public static Constant unshift(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var array = (Javascript.Array)_this;

            array.List.InsertRange(0, arguments);

            return Static.Undefined;
        }

        public static Constant indexOf(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var array = (Javascript.Array)_this;

            var start = arguments.Length > 1 ? (int)((Javascript.Number)arguments[1]).Value : 0;
            
            for(var i = start; i < array.List.Count; i++) {
                var element = array.List[i];
                var equals = Compare.StrictEqualityComparison(element, arguments[0]);
                if (equals) {
                    return new Javascript.Number(i);
                }
            }

            return new Javascript.Number(-1);
        }

        public static Constant splice(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var array = (Javascript.Array)_this;
            
            var start = arguments.Length > 0 ? (int)Tool.GetArgument<Javascript.Number>(arguments, 0, "Array.splice").Value : 0;
            var count = (int)(arguments.Length > 1 ? ((Javascript.Number)arguments[1]).Value : array.List.Count - start);

            // Remove items
            var result = new Javascript.Array();
            result.List.AddRange(array.List.GetRange(start, count));
            array.List.RemoveRange(start, count);

            // Add items
            var addCount = arguments.Length <= 2 ? 0 : arguments.Length - 2;
            for (var i = 0; i < addCount; i++) {
                array.List.Insert(start + i, arguments[i + 2]);
            }
            
            return result;
        }

        public static Constant slice(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var array = (Javascript.Array)_this;
            
            var begin = (int)(arguments.Length > 0 ? ((Javascript.Number)arguments[0]).Value : 0);
            var end = (int)(arguments.Length > 1 ? ((Javascript.Number)arguments[1]).Value : array.List.Count);

            var result = new Javascript.Array();
            for (int i = begin; i < end; i++) {
                result.List.Add(array.List[i]);
            }
            
            return result;
        }

        public static Constant map(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var array = (Javascript.Array)_this;

            if (arguments.Length != 1) {
                throw new Javascript.TypeError("Array.map takes one argument");
            }

            var callback = (Javascript.Function)arguments[0];

            var result = new Javascript.Array();
            for (int i = 0; i < array.List.Count; i++) {
                var callbackArguments = new Constant[] {
                    array.List[i],
                    new Javascript.Number(i),
                    array
                };
                var value = callback.Call(callbackArguments, Static.Undefined, lex);
                result.List.Add(value);
            }

            return result;
        }

        public static Constant filter(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var array = (Javascript.Array)_this;

            if (arguments.Length == 0 || arguments.Length > 2) {
                throw new Javascript.TypeError("Array.filter takes one or two argument");
            }

            var callback = (Javascript.Function)arguments[0];

            var result = new Javascript.Array();
            for (int i = 0; i < array.List.Count; i++) {
                var element = array.List[i];

                var callbackArguments = new Constant[] {
                    element,
                    new Javascript.Number(i),
                    array
                };

                var value = callback.Call(callbackArguments, arguments.Length == 1 ? Static.Undefined : arguments[1], lex);
                if (value is Javascript.Boolean) {
                    if (((Javascript.Boolean)value).Value) {
                        result.List.Add(element);
                    }
                }
            }

            return result;
        }

        public static Constant reduce(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var array = (Javascript.Array)_this;

            if (arguments.Length < 1) {
                throw new Javascript.TypeError("Array.reduce takes at least one argument");
            }

            var callback = (Javascript.Function)arguments[0];

            Constant accumulator;
            if(arguments.Length > 1) {
                accumulator = arguments[1];
            } else {
                if (array.List.Count == 0) throw new Javascript.TypeError("Array.reduce: No initial value is supplied and array is empty");
                accumulator = array.List[0];
            }
            
            for (int i = arguments.Length > 1 ? 0 : 1; i < array.List.Count; i++) {
                var callbackArguments = new Constant[] {
                    accumulator,
                    array.List[i],
                    new Javascript.Number(i),
                    array
                };
                accumulator = callback.Call(callbackArguments, Static.Undefined, lex);
            }

            return accumulator;
        }

        private static Constant checkAll(Constant _this, Constant[] arguments, LexicalEnvironment lex, string name, Func<bool, Constant, int, Constant> resultFunc, Constant final) {
            var array = (Javascript.Array)_this;

            if (arguments.Length == 0 || arguments.Length > 2) {
                throw new Javascript.TypeError(name + " takes one or two argument");
            }

            var callback = (Javascript.Function)arguments[0];

            for (int i = 0; i < array.List.Count; i++) {
                var element = array.List[i];

                var callbackArguments = new Constant[] {
                    element,
                    new Javascript.Number(i),
                    array
                };

                var value = callback.Call(callbackArguments, arguments.Length == 1 ? Static.Undefined : arguments[1], lex);
                if (value is Javascript.Boolean) {
                    var boolValue = ((Javascript.Boolean)value).Value;
                    var result = resultFunc(boolValue, element, i);
                    if (result != null) return result;
                }
            }

            return final;
        }

        public static Constant some(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            return checkAll(
                _this, 
                arguments, 
                lex,
                "Array.some",
                (value, element, index) => value ? new Javascript.Boolean(true) : null, 
                new Javascript.Boolean(false)
            );
        }

        public static Constant every(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            return checkAll(
                _this, 
                arguments, 
                lex,
                "Array.every",
                (value, element, index) => !value ? new Javascript.Boolean(false) : null,
                new Javascript.Boolean(true)
            );
        }

        public static Constant find(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            return checkAll(
                _this,
                arguments,
                lex,
                "Array.find",
                (value, element, index) => value ? element : null,
                Javascript.Static.Undefined
            );
        }

        public static Constant findIndex(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            return checkAll(
                _this,
                arguments,
                lex,
                "Array.findIndex",
                (value, element, index) => value ? new Javascript.Number(index) : null,
                Javascript.Static.Undefined
            );
        }

        public static Constant includes(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var array = (Javascript.Array)_this;

            if (arguments.Length == 0 || arguments.Length > 2) {
                throw new Javascript.TypeError("Array.includes takes one or two argument");
            }

            var reference = arguments[0];
            var startIndex = (int)(arguments.Length > 1 ? Tool.GetArgument<Javascript.Number>(arguments, 1, "Array.includes").Value : 0);

            for (int i = startIndex; i < array.List.Count; i++) {
                var equals = Compare.AbstractEqualityComparison(array.List[i], reference, lex);
                if (equals) {
                    return new Javascript.Boolean(true);
                }
            }

            return new Javascript.Boolean(false);
        }

        public static Constant sort(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var array = (Javascript.Array)_this;

            if (arguments.Length != 1) {
                throw new Javascript.TypeError("Array.sort takes one argument");
            }

            var callback = (Javascript.Function)arguments[0];

            var n = array.List.Count;
            while (true) {
                var swapped = false;

                for(var i = 1; i < n; i++) {
                    var aElement = array.List[i - 1];
                    var bElement = array.List[i];

                    var callbackArguments = new Constant[] { aElement, bElement };
                    var value = (Javascript.Number)callback.Call(callbackArguments, Static.Undefined, lex);

                    if(value.Value > 0) {
                        array.List[i - 1] = bElement;
                        array.List[i] = aElement;
                        swapped = true;
                    }
                }

                n--;

                if (!swapped) break;
            }

            return array;
        }

        public static Constant join(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var array = (Javascript.Array)_this;

            var seperator = arguments.Length > 0 ? ((Javascript.String)arguments[0]).Value : ",";

            var result = new StringBuilder();
            for (int i = 0; i < array.List.Count; i++) {
                if(i > 0) {
                    result.Append(seperator);
                }

                result.Append(Convert.ToString(array.List[i], lex));
            }

            return new Javascript.String(result.ToString());
        }

        public static Constant reverse(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var array = (Javascript.Array)_this;

            array.List.Reverse();

            return array;
        }

        public static Constant fill(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var array = (Javascript.Array)_this;

            var value = Tool.GetArgument(arguments, 0, "Array.fill");
            var start = Tool.GetArgument<Javascript.Number>(arguments, 1, "Array.fill", false) ?? new Javascript.Number(0);
            var end = Tool.GetArgument<Javascript.Number>(arguments, 2, "Array.fill", false) ?? new Javascript.Number(array.List.Count);

            for(var i = (int)start.Value; i < (int)end.Value; i++) {
                array.List[i] = value;
            }

            return array;
        }

        public static Constant toString(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            // Because this is actually useful
            return JSON.stringify(Static.Undefined, new Constant[] { _this }, lex);

            // According to javascript specification
            // return join(_this, arguments, lex);
        }
    }
}
