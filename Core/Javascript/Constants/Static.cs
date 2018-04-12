using System.Text;

namespace NetJS.Core.Javascript {
    public class Static {
        public static Undefined Undefined = new Undefined();
        public static Null Null = new Null();
        public static NaN NaN = new NaN();
        public static Infinity Infinity = new Infinity();

        public static Boolean True = new Boolean(true);
        public static Boolean False = new Boolean(false);
    }

    public class Undefined : Constant {

        public override Constant Equals(Constant other, Scope scope) {
            return other is Undefined || other is Null ? Static.True : Static.False;
        }

        public override Constant StrictEquals(Constant other, Scope scope) {
            return other is Undefined ? Static.True : Static.False;
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
            return other is Null || other is Undefined ? Static.True : Static.False;
        }

        public override Constant StrictEquals(Constant other, Scope scope) {
            return other is Null ? Static.True : Static.False;
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
            return other is NaN ? Static.True : Static.False;
        }

        public override Constant StrictEquals(Constant other, Scope scope) {
            return other is NaN ? Static.True : Static.False;
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
            return other is Infinity ? Static.True : Static.False;
        }

        public override Constant StrictEquals(Constant other, Scope scope) {
            return other is Infinity ? Static.True : Static.False;
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.Infinity);
        }

        public override string ToDebugString() {
            return "Infinity";
        }
    }
}