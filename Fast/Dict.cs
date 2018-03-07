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

        public Dict(uint length = 31) {
            _length = length;
            _values = new Entry[length][];
        }

        public IEnumerable<Entry> Items {
            get {
                var items = new List<Entry>();
                for (var r = 0; r < _values.Length; r++) {
                    var row = _values[r];
                    if (row != null) {
                        for (var i = 0; i < row.Length; i++) {
                            var item = row[i];
                            if (item.Key == null) break;
                            items.Add(item);
                        }
                    }
                }
                return items;
            }
        }

        // TODO: remove duplicate code
        public IEnumerable<string> Keys {
            get {
                var items = new List<string>();
                for (var r = 0; r < _values.Length; r++) {
                    var row = _values[r];
                    if (row != null) {
                        for (var i = 0; i < row.Length; i++) {
                            var item = row[i];
                            if (item.Key == null) break;
                            items.Add(item.Key);
                        }
                    }
                }
                return items;
            }
        }

        // TODO: remove duplicate code
        public IEnumerable<T> Values {
            get {
                var items = new List<T>();
                for (var r = 0; r < _values.Length; r++) {
                    var row = _values[r];
                    if (row != null) {
                        for (var i = 0; i < row.Length; i++) {
                            var item = row[i];
                            if (item.Key == null) break;
                            items.Add(item.Value);
                        }
                    }
                }
                return items;
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
