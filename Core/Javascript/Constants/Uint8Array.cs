using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Uint8Array : Constant {
        public ArrayBuffer Buffer;

        public Uint8Array(int length) {
            Buffer = new ArrayBuffer(length);
        }

        public Uint8Array(ArrayBuffer buffer) {
            Buffer = buffer;
        }

        public Constant Get(int index) {
            if (index >= 0 && index < Buffer.Data.Length) {
                return new Number(Buffer.Data[index]);
            } else {
                return Static.Undefined;
            }
        }

        public override Constant GetProperty(Constant key, Scope scope) {
            if (key is Number n) {
                return Get((int)n.Value);
            }

            var keyString = key.ToString();

            int index;
            if (int.TryParse(keyString, NumberStyles.Integer, CultureInfo.InvariantCulture, out index)) {
                return Get(index);
            }

            if (keyString == "length") {
                return new Number(Buffer.Data.Length);
            } else if(keyString == "buffer") {
                return Buffer;
            }

            return Static.Undefined;
        }

        public override void SetProperty(Constant key, Constant value, Scope scope) {
            if (key is Number n && value is Number v) {
                var index = (int)n.Value;
                if (index >= 0 && index < Buffer.Data.Length) {
                    Buffer.Data[index] = (byte)((uint)v.Value);
                }
            }
        }

        public override string ToString() {
            return "[object Uint8Array]";
        }

        public override Constant TypeOf(Scope scope) {
            return new String("object");
        }

        public override string ToDebugString() {
            return $"Uint8Array[x{Buffer.Data.Length}]";
        }
    }
}
