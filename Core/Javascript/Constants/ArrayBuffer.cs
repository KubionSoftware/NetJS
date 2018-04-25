using NetJS.Core.Javascript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class ArrayBuffer : Constant {
        public byte[] Data;

        public ArrayBuffer(int length) {
            Data = new byte[length];
        }

        public ArrayBuffer(byte[] data) {
            Data = data;
        }

        public override string ToString() {
            return "[object ArrayBuffer]";
        }

        public override Constant TypeOf(Scope scope) {
            return new String("object");
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append($"new ArrayBuffer({Data.Length})");
        }

        public override string ToDebugString() {
            return $"ArrayBuffer[x{Data.Length}]";
        }
    }
}
