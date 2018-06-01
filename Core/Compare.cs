using NetJS.Core.Javascript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    class Compare {

        public enum RelationalComparisonResult {
            True,
            False,
            Undefined
        }

        public static RelationalComparisonResult AbstractRelationalComparison(Constant x, Constant y, bool leftFirst, Scope scope) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-abstract-relational-comparison

            // Control order of operations
            Constant px, py;
            if (leftFirst) {
                px = Convert.ToPrimitive(x, scope);
                py = Convert.ToPrimitive(y, scope);
            } else {
                py = Convert.ToPrimitive(y, scope);
                px = Convert.ToPrimitive(x, scope);
            }

            if(px is Javascript.String sx && py is Javascript.String sy) {
                if (sy.Value.StartsWith(sx.Value)) return RelationalComparisonResult.False;
                if (sx.Value.StartsWith(sy.Value)) return RelationalComparisonResult.True;

                // K is the first index where the strings are different
                int k;
                for (k = 0; sx.Value[k] == sy.Value[k]; k++);

                return sx.Value[k] < sy.Value[k] ? RelationalComparisonResult.True : RelationalComparisonResult.False;
            } else {
                var nx = Convert.ToNumber(px, scope);
                var ny = Convert.ToNumber(py, scope);

                // TODO: optimize?
                if (double.IsNaN(nx) || double.IsNaN(ny)) return RelationalComparisonResult.Undefined;
                if (nx == ny) return RelationalComparisonResult.False;
                if (double.IsPositiveInfinity(nx)) return RelationalComparisonResult.False;
                if (double.IsPositiveInfinity(ny)) return RelationalComparisonResult.True;
                if (double.IsNegativeInfinity(ny)) return RelationalComparisonResult.False;
                if (double.IsNegativeInfinity(nx)) return RelationalComparisonResult.True;

                return nx < ny ? RelationalComparisonResult.True : RelationalComparisonResult.False;
            }
        }

        public static bool AbstractEqualityComparison(Constant x, Constant y, Scope scope) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-abstract-equality-comparison

            if (x.GetType() == y.GetType()) return StrictEqualityComparison(x, y);

            if (x is Null && y is Undefined) return true;
            if (x is Undefined && y is Null) return true;

            if (x is Number nx && y is Javascript.String) {
                return AbstractEqualityComparison(nx, new Number(Convert.ToNumber(y, scope)), scope);
            }
            if (x is Javascript.String && y is Number ny) {
                return AbstractEqualityComparison(new Number(Convert.ToNumber(x, scope)), ny, scope);
            }

            if (x is Javascript.Boolean) {
                return AbstractEqualityComparison(new Number(Convert.ToNumber(x, scope)), y, scope);
            }
            if (y is Javascript.Boolean) {
                return AbstractEqualityComparison(x, new Number(Convert.ToNumber(y, scope)), scope);
            }

            if (y is Javascript.Object && (x is Javascript.String || x is Javascript.Number || x is Javascript.Symbol)) {
                return AbstractEqualityComparison(x, Convert.ToPrimitive(y, scope), scope);
            }
            if (x is Javascript.Object && (y is Javascript.String || y is Javascript.Number || y is Javascript.Symbol)) {
                return AbstractEqualityComparison(Convert.ToPrimitive(x, scope), y, scope);
            }

            return false;
        }

        public static bool StrictEqualityComparison(Constant x, Constant y) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-strict-equality-comparison

            if (x.GetType() != y.GetType()) return false;

            if (x is Number nx && y is Number ny) {
                if (double.IsNaN(nx.Value)) return false;
                if (double.IsNaN(ny.Value)) return false;

                if (nx.Value == ny.Value) return true;

                return false;
            }

            return SameValueNonNumber(x, y);
        }

        public static bool SameValueNonNumber(Constant x, Constant y) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-samevaluenonnumber

            // Assert Type(x) is not Number
            // Assert Type(x) is the same as Type(y)

            if (x is Undefined) return true;
            if (x is Null) return true;

            if (x is Javascript.String sx && y is Javascript.String sy) {
                return sx.Value == sy.Value;
            }

            if (x is Javascript.Boolean bx && y is Javascript.Boolean by) {
                return bx.Value == by.Value;
            }

            if (x is Javascript.Symbol syx && y is Javascript.Symbol syy) {
                return syx == syy;
            }

            return x == y;
        }
    }
}
