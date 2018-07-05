using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS {
    public abstract class JSStorage {

        private ConcurrentDictionary<string, object> _dict;

        public JSStorage() {
            _dict = new ConcurrentDictionary<string, object>();
        }

        public void Set(string key, object value) {
            _dict[key] = value;
        }

        public object Get(string key) {
            if (_dict.ContainsKey(key)) {
                return _dict[key];
            } else {
                return null;
            }
        }

        public void Remove(string key) {
            if (_dict.ContainsKey(key)) {
                _dict.TryRemove(key, out object removed);
            }
        }

        public void Clear() {
            _dict.Clear();
        }

        public dynamic GetObject() {
            dynamic obj = new NetJSObject();

            foreach(var pair in _dict) {
                obj[pair.Key] = pair.Value;
            }

            return obj;
        }

        public Dictionary<string, string> GetStrings() {
            return _dict.ToDictionary(pair => pair.Key, pair => pair.Value.ToString());
        }
    }
}
