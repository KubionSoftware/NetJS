using System.Text;

namespace NetJS.Javascript {
    public class Static {
        public static Undefined Undefined = new Undefined();
        public static Null Null = new Null();
        public static NaN NaN = new NaN();
        public static Infinity Infinity = new Infinity();
    }

    public class Undefined : Constant {

        public override Constant Equals(Constant other, Scope scope) {
            if (other is Undefined || other is Null) {
                return new Boolean(true);
            }

            return new Boolean(false);
        }

        public override Constant StrictEquals(Constant other, Scope scope) {
            if (other is Undefined) {
                return new Boolean(true);
            }

            return new Boolean(false);
        }

        public override Constant Add(Constant other, Scope scope) {
            return other;
        }

        public override string ToString() {
            return "";
        }

        public override Constant TypeOf(Scope scope) {
            return new String("undefined");
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.Undefined);
        }

        public override string ToDebugString() {
            return "undefined";
        }
    }

    public class Null : Constant {

        public override Constant Equals(Constant other, Scope scope) {
            if (other is Undefined || other is Null) {
                return new Boolean(true);
            }

            return new Boolean(false);
        }

        public override Constant StrictEquals(Constant other, Scope scope) {
            if (other is Null) {
                return new Boolean(true);
            }

            return new Boolean(false);
        }

        public override string ToString() {
            return "null";
        }

        public override Constant TypeOf(Scope scope) {
            return new String("object");
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.Null);
        }

        public override string ToDebugString() {
            return "null";
        }
    }

    public class NaN : Constant {

        public override Constant TypeOf(Scope scope) {
            return new String("number");
        }

        public override Constant Equals(Constant other, Scope scope) {
            if (other is NaN) {
                return new Boolean(true);
            }

            return new Boolean(false);
        }

        public override Constant StrictEquals(Constant other, Scope scope) {
            if (other is NaN) {
                return new Boolean(true);
            }

            return new Boolean(false);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.NotANumber);
        }

        public override string ToDebugString() {
            return "NaN";
        }
    }

    public class Infinity : Constant {

        public override Constant TypeOf(Scope scope) {
            return new String("number");
        }

        public override Constant Equals(Constant other, Scope scope) {
            if (other is Infinity) {
                return new Boolean(true);
            }

            return new Boolean(false);
        }

        public override Constant StrictEquals(Constant other, Scope scope) {
            if (other is Infinity) {
                return new Boolean(true);
            }

            return new Boolean(false);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.Infinity);
        }

        public override string ToDebugString() {
            return "Infinity";
        }
    }
}