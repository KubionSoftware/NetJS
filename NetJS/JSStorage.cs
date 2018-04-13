using NetJS.Core.Javascript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS {
    public abstract class JSStorage {

        private Dictionary<string, Constant> _dict;

        public JSStorage() {
            _dict = new Dictionary<string, Constant>();
        }

        public void Set(string key, Constant value) {
            _dict[key] = value;
        }

        public Constant Get(string key) {
            if (_dict.ContainsKey(key)) {
                return _dict[key];
            } else {
                return Static.Undefined;
            }
        }

        public void Remove(string key) {
            if (_dict.ContainsKey(key)) {
                _dict.Remove(key);
            }
        }

        public void Clear() {
            _dict.Clear();
        }

        public Dictionary<string, string> GetStrings() {
            return _dict.ToDictionary(pair => pair.Key, pair => pair.Value.ToString());
        }
    }
}
