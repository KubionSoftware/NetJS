using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Fast
{
    public class Dict<T> {

        public struct Entry {
            public string Key;
            public T Value;

            public override string ToString() {
                return "{" + Key + ", " + Value.ToString() + "}";
            }
        }

        private uint _length;
        private Entry[][] _values;
        private string[] _keys;
        private int _keyIndex = 0;

        public Dict(uint length = 31) {
            _length = length;
            _values = new Entry[length][];
            _keys = new string[length];
        }

        private IEnumerable<A> All<A>(Func<Entry, A> action) {
            var items = new A[_keyIndex];
            for(var i = 0; i < _keyIndex; i++) {
                items[i] = action(GetEntry(_keys[i]));
            }
            return items;
        }

        public IEnumerable<Entry> Items {
            get {
                return All(item => item);
            }
        }
        
        public IEnumerable<string> Keys {
            get {
                return All(item => item.Key);
            }
        }
        
        public IEnumerable<T> Values {
            get {
                return All(item => item.Value);
            }
        }

        public Dictionary<string, N> ToDictionary<N>(Func<Entry, string> keyFunc, Func<Entry, N> valueFunc) {
            var dict = new Dictionary<string, N>();

            foreach(var key in Keys) {
                var entry = GetEntry(key);
                dict[keyFunc(entry)] = valueFunc(entry);
            }

            return dict;
        }

        public IEnumerable<N> Select<N>(Func<Entry, N> valueFunc) {
            var list = new List<N>();

            foreach (var key in Keys) {
                var entry = GetEntry(key);
                list.Add(valueFunc(entry));
            }

            return list;
        }

        public void Set(string key, T value) {
            var hash = Hash(key);
            var row = _values[hash];

            if(_keyIndex >= _keys.Length) {
                // Resize keys array if not big enough
                var newKeys = new string[_keys.Length * 2];
                for (var i = 0; i < _keys.Length; i++) newKeys[i] = _keys[i];
                _keys = newKeys;
            }
            // Add key to keys array
            _keys[_keyIndex] = key;
            _keyIndex++;

            if (row == null) {
                row = new Entry[10];
                row[0] = new Entry() { Key = key, Value = value };
                _values[hash] = row;
                return;
            }

            for (var i = 0; i < row.Length; i++) {
                if (row[i].Key == null) {
                    row[i] = new Entry() { Key = key, Value = value };
                    return;
                }
                if (row[i].Key == key) {
                    row[i].Value = value;
                    return;
                }
            }

            var newRow = new Entry[row.Length * 2];
            for(var i = 0; i < row.Length; i++) {
                newRow[i] = row[i];
            }
            newRow[row.Length] = new Entry() { Key = key, Value = value };
        }

        public bool ContainsKey(string key) {
            T value = default(T);
            return TryGetValue(key, ref value);
        }

        public T Get(string key) {
            T value = default(T);
            TryGetValue(key, ref value);
            return value;
        }

        public bool TryGetValue(string key, ref T value) {
            var hash = Hash(key);
            var row = _values[hash];
            if (row == null) return false;

            for (var i = 0; i < row.Length; i++) {
                if (row[i].Key == null) {
                    return false;
                }
                if (row[i].Key == key) {
                    var r = row[i];
                    value = r.Value;
                    return true;
                }
            }
            
            return false;
        }

        private Entry GetEntry(string key) {
            var hash = Hash(key);
            var row = _values[hash];

            if (row == null) {
                return default(Entry);
            }

            for (var i = 0; i < row.Length; i++) {
                if (row[i].Key == null) {
                    break;
                } else {
                    if (row[i].Key == key) {
                        return row[i];
                    }
                }
            }
            
            return default(Entry);
        }

        public void Remove(string key) {
            var hash = Hash(key);
            var row = _values[hash];
            if (row == null) return;

            // Remove the key
            var foundKey = false;
            for(var i = 0; i < _keyIndex; i++) {
                if(_keys[i] == key) {
                    _keyIndex--;
                    foundKey = true;
                }
                if (foundKey) _keys[i] = _keys[i + 1];
            }

            // Remove the value
            bool found = false;
            for (var i = 0; i < row.Length; i++) {
                if (found) row[i - 1] = row[i];

                if (row[i].Key == null) {
                    return;
                } else {
                    if (row[i].Key == key) {
                        row[i] = default(Entry);
                        found = true;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint Hash(string key) {
            uint value = 0;
            foreach (var c in key) {
                value = (value + c) * 68041;
            }
            return value % _length;
        }
    }
}
