using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetJS {
    public class JSSession {

        private Dictionary<string, Javascript.Constant> _dict;

        public JSSession() {
            _dict = new Dictionary<string, Javascript.Constant>();
        }

        public void Set(string key, Javascript.Constant value) {
            _dict[key] = value;
        }

        public Javascript.Constant Get(string key) {
            if (_dict.ContainsKey(key)) {
                return _dict[key];
            } else {
                return Javascript.Static.Undefined;
            }
        }

        public void Remove(string key) {
            if (_dict.ContainsKey(key)) {
                _dict.Remove(key);
            }
        }

        public Dictionary<string, string> GetStrings() {
            return _dict.ToDictionary(pair => pair.Key, pair => pair.Value.ToString());
        }
    }
}