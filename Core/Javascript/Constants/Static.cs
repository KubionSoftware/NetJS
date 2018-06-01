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
        
        public override string ToString() {
            return "";
        }

        public override Constant TypeOf(Scope scope) {
            return new String("undefined");
        }

        public override string ToDebugString() {
            return "undefined";
        }
    }

    public class Null : Constant {
        
        public override string ToString() {
            return "null";
        }

        public override Constant TypeOf(Scope scope) {
            return new String("object");
        }

        public override string ToDebugString() {
            return "null";
        }
    }

    public class NaN : Number {

        public NaN() : base(double.NaN) { }

        public override Constant TypeOf(Scope scope) {
            return new String("number");
        }
        
        public override string ToString() {
            return "NaN";
        }

        public override string ToDebugString() {
            return "NaN";
        }
    }

    public class Infinity : Number {

        public Infinity() : base(double.PositiveInfinity) { }

        public override Constant TypeOf(Scope scope) {
            return new String("number");
        }
        
        public override string ToString() {
            return "Infinity";
        }

        public override string ToDebugString() {
            return "Infinity";
        }
    }
}