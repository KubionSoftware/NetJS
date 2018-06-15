using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class Uint8Array : Object {

        public ArrayBuffer Buffer;

        public Uint8Array(int length, Agent agent) : base(Tool.Prototype("Uint8Array", agent)) {
            Buffer = new ArrayBuffer(length);
        }

        public Uint8Array(ArrayBuffer buffer, Agent agent) : base(Tool.Prototype("Uint8Array", agent)) {
            Buffer = buffer;
        }

        public Constant Get(int index) {
            if (index >= 0 && index < Buffer.Data.Length) {
                return new Number(Buffer.Data[index]);
            } else {
                return Static.Undefined;
            }
        }

        public void Set(int index, Constant value) {
            if (value is Number n && index >= 0 && index < Buffer.Data.Length) {
                Buffer.Data[index] = (byte)(int)n.Value;
            }
        }

        public override Constant Get(Constant p, Agent agent = null, Constant receiver = null) {
            Assert.IsPropertyKey(p);

            var keyString = p.ToString();

            int index;
            if (int.TryParse(keyString, NumberStyles.Integer, CultureInfo.InvariantCulture, out index)) {
                return Get(index);
            }

            return base.Get(p, agent, receiver);
        }

        public override bool Set(Constant p, Constant v, Agent agent = null, Constant receiver = null) {
            Assert.IsPropertyKey(p);

            var keyString = p.ToString();

            int index;
            if (int.TryParse(keyString, NumberStyles.Integer, CultureInfo.InvariantCulture, out index)) {
                if (index >= 0 && index < Buffer.Data.Length) {
                    Set(index, v);
                    return true;
                }
            }

            return base.Set(p, v, agent, receiver);
        }

        public override string ToString() {
            return "[object Uint8Array]";
        }

        public override string ToDebugString() {
            return $"Uint8Array[x{Buffer.Data.Length}]";
        }
    }
}
