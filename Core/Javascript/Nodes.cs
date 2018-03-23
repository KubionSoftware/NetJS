using System.Collections.Generic;
using System.Text;

namespace NetJS.Core.Javascript {
    
    public abstract class Expression : Node {

        public virtual Constant Execute(Scope scope, bool getValue = true) {
            return Static.Undefined;
        }

        public bool IsTrue(Scope scope) {
            var result = Execute(scope);

            if (result is Boolean b) {
                return b.Value;
            } else if(result is Number n) {
                return n.Value != 0;
            } else if(result is String s) {
                return s.Value.Length > 0;
            } else if(result is Object) {
                return true;
            }

            // undefined, null, NaN
            return false;
        }

        public virtual Constant GetValue(Scope scope) {
            return Static.Undefined;
        }
        
        public abstract string ToDebugString();
    }

    public class ParameterList : Node {
        public IList<Variable> Parameters = new List<Variable>();

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append("(");

            for(var i = 0; i < Parameters.Count; i++) {
                if (i > 0) builder.Append(", ");
                Parameters[i].Uneval(builder, depth);
            }

            builder.Append(")");
        }
    }

    public abstract class Statement : Node {
        
        public virtual Result Execute(Scope scope) {
            return new Result(ResultType.None);
        }
    }

    public abstract class Node {

#if debug_enabled
        public int Id = -1;

        public void RegisterDebug(Debug.Location location) {
            Id = Debug.AddNode(location);
        }
#endif

        public abstract void Uneval(StringBuilder builder, int depth);
        public static void NewLine(StringBuilder builder, int depth) {
            builder.Append("\n");
            for(var i = 0; i < depth; i++) {
                builder.Append("\t");
            }
        }
    }
}