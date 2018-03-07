using System.Collections.Generic;
using System.Linq;

namespace Util {
    public class Json {

        private Dictionary<string, object> _value;

        public Dictionary<string, object> Value {
            get {
                return _value;
            }
        }

        public IEnumerable<string> Keys {
            get {
                return _value.Keys;
            }
        }

        public Json() {
            _value = new Dictionary<string, object>();
        }

        public Json(Dictionary<string, object> value) {
            _value = value;
        }

        public Json(string s) {
            _value = JsonParser.StringToJsonObject(s);
        }

        public bool Has(string key) {
            return _value.ContainsKey(key);
        }

        public object Get(string key) {
            return _value[key];
        }

        public T Get<T>(string key) {
            return (T)_value[key];
        }

        public string String(string key) {
            return (string)_value[key];
        }

        public double Double(string key) {
            return (double)_value[key];
        }

        public int Int(string key) {
            return (int)Double(key); 
        }

        public Json Object(string key) {
            return new Json((Dictionary<string, object>)_value[key]);
        }

        public IEnumerable<Json> Objects(string key) {
            return List(key).Select(o => new Json((Dictionary<string, object>)o));
        }

        public Dictionary<string, object> Dict(string key) {
            return (Dictionary<string, object>)_value[key];
        }

        public Dictionary<string, T> Dict<T>(string key) {
            return Dict(key).ToDictionary(pair => pair.Key, pair => (T)pair.Value);
        }

        public IList<object> List(string key) {
            return (List<object>)_value[key];
        }

        public IList<T> List<T>(string key) {
            return List(key).Select(item => (T)item).ToList();
        }

        public void Set(string key, string value) {
            _value[key] = value;
        }

        public void Set(string key, double value) {
            _value[key] = value;
        }

        public void Set(string key, bool value) {
            _value[key] = value;
        }

        public void Set(string key, IEnumerable<string> list) {
            _value[key] = list.Select(item => (object)item).ToList();
        }

        public void Set(string key, IEnumerable<double> list) {
            _value[key] = list.Select(item => (object)item).ToList();
        }

        public void Set(string key, IEnumerable<bool> list) {
            _value[key] = list.Select(item => (object)item).ToList();
        }

        public void Set(string key, IEnumerable<Json> objects) {
            _value[key] = objects.Select(o => (object)o._value).ToList();
        }

        public void Set<T>(string key, Dictionary<string, T> dict) {
            _value[key] = dict.ToDictionary(pair => pair.Key, pair => (object)pair.Value);
        }

        public void Set(string key, Json obj) {
            _value[key] = obj._value;
        }

        public void Remove(string key) {
            _value.Remove(key);
        }

        public string ToString(bool pretty = false) {
            return JsonParser.JsonToString(_value, pretty);
        }
    }
}