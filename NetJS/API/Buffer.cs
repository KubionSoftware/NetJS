namespace NetJS.API {
    public class Buffer {

        public static void set(string value) {
            State.Buffer.Clear();
            State.Buffer.Append(value);
        }

        public static string get() {
            return State.Buffer.ToString();
        }

        public static void write(object o) {
            if (o is string s) {
                State.Buffer.Append(s);
            } else if (o is int i) {
                State.Buffer.Append(i.ToString());
            } else if (o is double d) {
                State.Buffer.Append(d.ToString());
            } else if (o is bool b) {
                State.Buffer.Append(b ? "true" : "false");
            }
        }

        public static void clear() {
            State.Buffer.Clear();
        }
    }
}