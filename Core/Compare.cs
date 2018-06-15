using NetJS.Core;
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

        public static RelationalComparisonResult AbstractRelationalComparison(Constant x, Constant y, bool leftFirst, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-abstract-relational-comparison

            // Control order of operations
            Constant px, py;
            if (leftFirst) {
                px = Convert.ToPrimitive(x, agent);
                py = Convert.ToPrimitive(y, agent);
            } else {
                py = Convert.ToPrimitive(y, agent);
                px = Convert.ToPrimitive(x, agent);
            }

            if(px is String sx && py is String sy) {
                if (sy.Value.StartsWith(sx.Value)) return RelationalComparisonResult.False;
                if (sx.Value.StartsWith(sy.Value)) return RelationalComparisonResult.True;

                // K is the first index where the strings are different
                int k;
                for (k = 0; sx.Value[k] == sy.Value[k]; k++);

                return sx.Value[k] < sy.Value[k] ? RelationalComparisonResult.True : RelationalComparisonResult.False;
            } else {
                var nx = Convert.ToNumber(px, agent);
                var ny = Convert.ToNumber(py, agent);

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

        public static bool AbstractEqualityComparison(Constant x, Constant y, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-abstract-equality-comparison

            if (x.GetType() == y.GetType()) return StrictEqualityComparison(x, y);

            if (x is Null && y is Undefined) return true;
            if (x is Undefined && y is Null) return true;

            if (x is Number nx && y is String) {
                return AbstractEqualityComparison(nx, new Number(Convert.ToNumber(y, agent)), agent);
            }
            if (x is String && y is Number ny) {
                return AbstractEqualityComparison(new Number(Convert.ToNumber(x, agent)), ny, agent);
            }

            if (x is Boolean) {
                return AbstractEqualityComparison(new Number(Convert.ToNumber(x, agent)), y, agent);
            }
            if (y is Boolean) {
                return AbstractEqualityComparison(x, new Number(Convert.ToNumber(y, agent)), agent);
            }

            if (y is Object && (x is String || x is Number || x is Symbol)) {
                return AbstractEqualityComparison(x, Convert.ToPrimitive(y, agent), agent);
            }
            if (x is Object && (y is String || y is Number || y is Symbol)) {
                return AbstractEqualityComparison(Convert.ToPrimitive(x, agent), y, agent);
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

            if (x is String sx && y is String sy) {
                return sx.Value == sy.Value;
            }

            if (x is Boolean bx && y is Boolean by) {
                return bx.Value == by.Value;
            }

            if (x is Symbol syx && y is Symbol syy) {
                return syx == syy;
            }

            return x == y;
        }

        public static bool SameValue(Constant x, Constant y) {
            if (x.GetType() != y.GetType()) return false;
            if (x is Number nx && y is Number ny) {
                if (double.IsNaN(nx.Value) && double.IsNaN(ny.Value)) return true;
                if (nx.Value == ny.Value) return true;
                return false;
            }
            return SameValueNonNumber(x, y);
        }
    }
}
