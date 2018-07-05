using Microsoft.ClearScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using Microsoft.ClearScript.V8;

namespace NetJS {

    public class NetJSObject : DynamicObject {
        // The inner dictionary to store field names and values.
        Dictionary<string, object> dictionary = new Dictionary<string, object>();

        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            return dictionary.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value) {
            dictionary[binder.Name] = value;
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value) {
            string index = indexes[0].ToString();

            if (dictionary.ContainsKey(index)) {
                dictionary[index] = value;
            } else {
                dictionary.Add(index, value);
            }

            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result) {
            string index = indexes[0].ToString();
            return dictionary.TryGetValue(index, out result);
        }
    }

    public class Tool {

        public static bool IsUndefined(dynamic value) {
            return value is Undefined;
        }

        public static bool IsObject(dynamic value) {
            return value is ScriptObject;
        }

        public static bool GetObject(dynamic value, out ScriptObject obj) {
            if (value is ScriptObject o) {
                obj = o;
                return true;
            }

            obj = null;
            return false;
        }

        public static bool Get<T>(dynamic value, out T obj) {
            if (value is T o) {
                obj = o;
                return true;
            }

            obj = default(T);
            return false;
        }

        public static object GetValue(dynamic obj, string key) {
            return obj[key];
        }

        private static dynamic _createJsArray;
        private static dynamic _createJsByteArray;
        private static dynamic _createJSPromise;
        private static dynamic _createJSObject;
        private static dynamic _getMemberNames;
        private static dynamic _getStack;

        public static void Init(V8ScriptEngine engine) {
            _createJsArray = engine.Evaluate(@"
                (function (list) {
                    var array = [];
                    for (var i = 0; i < list.Length; i++){
                        array.push(list[i]);
                    }
                    return array;
                }).valueOf()
            ");

            _createJsByteArray = engine.Evaluate(@"
                (function (list) {
                    var array = new Uint8Array(list.Length);
                    for (var i = 0; i < list.Length; i++){
                        array[i] = list[i];
                    }
                    return array;
                }).valueOf()
            ");

            _createJSPromise = engine.Evaluate(@"
                (function (action) {
                    return new Promise((resolve, reject) => {
                        action(resolve, reject);
                    });
                }).valueOf()
            ");

            _createJSObject = engine.Evaluate(@"
                (function (list) {
                    var obj = {};
                    for (var i = 0; i < list.Count; i++){
                        obj[list[i].Key] = list[i].Value;
                    }
                    return obj;
                }).valueOf()
            ");

            _getStack = engine.Evaluate(@"
                (function() {
                  // Save original Error.prepareStackTrace
                  var origPrepareStackTrace = Error.prepareStackTrace

                  // Override with function that just returns `stack`
                  Error.prepareStackTrace = function (_, stack) {
                    return stack
                  }

                  // Create a new `Error`, which automatically gets `stack`
                  var err = new Error()

                  // Evaluate `err.stack`, which calls our new `Error.prepareStackTrace`
                  var stack = err.stack

                  // Restore original `Error.prepareStackTrace`
                  Error.prepareStackTrace = origPrepareStackTrace

                  // Remove superfluous function call on stack
                  stack.shift() // getStack --> Error

                  return stack
                }).valueOf()
            ");
        }

        public static dynamic ToArray(object[] list) {
            var array = _createJsArray(list);
            return array;
        }

        public static dynamic ToByteArray(byte[] list) {
            var array = _createJsByteArray(list.Select(b => (object)b).ToArray());
            return array;
        }

        public static dynamic ToObject(Dictionary<string, object> dict) {
            var array = _createJSObject(dict.ToList());
            return array;
        }

        public static dynamic CreatePromise(Action<dynamic, dynamic> a) {
            Action<dynamic, dynamic> task = (resolve, reject) => {
                new Task(() => {
                    a(resolve, reject);
                }).Start();
            };

            return _createJSPromise(task);
        }

        public static dynamic GetStack() {
            if (_getStack == null) return null;
            return _getStack();
        }
    }
}
