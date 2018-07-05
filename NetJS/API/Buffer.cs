namespace NetJS.API {
    public class Buffer {

        public static void set(string value) {
            State.Buffer.Clear();
            State.Buffer.Append(value);
        }

        public static string get() {
            return State.Buffer.ToString();
        }

        public static void write(string s) {
            State.Buffer.Append(s);
        }

        public static void clear() {
            State.Buffer.Clear();
        }
    }
}