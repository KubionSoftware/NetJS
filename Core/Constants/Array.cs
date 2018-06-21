using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class Array : Object {

        private List<Constant> _list;
        public IReadOnlyList<Constant> List { get { return _list.AsReadOnly(); } }

        public Array(int length, Agent agent) : base(Tool.Prototype("Array", agent)) {
            _list = new List<Constant>(length);

            for (var i = 0; i < length; i++) {
                _list.Add(Static.Undefined);
            }

            Set("length", new Number(_list.Count), agent);
        }

        public void Add(Constant item, Agent agent) {
            _list.Add(item);
            Set("length", new Number(_list.Count), agent);
        }

        public void AddRange(IEnumerable<Constant> items, Agent agent) {
            _list.AddRange(items);
            Set("length", new Number(_list.Count), agent);
        }

        public void Insert(int index, Constant item, Agent agent) {
            _list.Insert(index, item);
            Set("length", new Number(_list.Count), agent);
        }

        public void InsertRange(int index, IEnumerable<Constant> item, Agent agent) {
            _list.InsertRange(index, item);
            Set("length", new Number(_list.Count), agent);
        }

        public void RemoveAt(int index, Agent agent) {
            _list.RemoveAt(index);
            Set("length", new Number(_list.Count), agent);
        }

        public void RemoveRange(int index, int count, Agent agent) {
            _list.RemoveRange(index, count);
            Set("length", new Number(_list.Count), agent);
        }

        public void Reverse() {
            _list.Reverse();
        }

        public List<Constant> GetRange(int index, int count) {
            return _list.GetRange(index, count);
        }

        public Constant Get(int index) {
            if (index >= 0 && index < _list.Count) {
                return _list[index];
            } else {
                return Static.Undefined;
            }
        }

        public void Set(int index, Constant value) {
            if (index >= 0 && index < _list.Count) {
                _list[index] = value;
            }
        }

        public override Constant Get(Constant p, Agent agent = null, Constant receiver = null, int depth = 0) {
            Assert.IsPropertyKey(p);

            var keyString = p.ToString();

            int index;
            if (int.TryParse(keyString, NumberStyles.Integer, CultureInfo.InvariantCulture, out index)) {
                return Get(index);
            }

            return base.Get(p, agent, receiver, depth);
        }

        public override bool Set(Constant p, Constant v, Agent agent = null, Constant receiver = null, int depth = 0) {
            Assert.IsPropertyKey(p);

            var keyString = p.ToString();

            int index;
            if (int.TryParse(keyString, NumberStyles.Integer, CultureInfo.InvariantCulture, out index)) {
                if (index >= 0 && index < _list.Count) {
                    Set(index, v);
                    return true;
                }
            }

            return base.Set(p, v, agent, receiver, depth);
        }

        public override string ToString() {
            return "[ x" + _list.Count + " ]";
        }

        public override string ToDebugString() {
            return $"[{string.Join(", ", _list.Select(item => item.ToDebugString()))}]";
        }
    }
}
