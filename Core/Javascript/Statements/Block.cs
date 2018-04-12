using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Block : Statement {
        public IList<Node> Nodes = new List<Node>();

        public override Result Execute(Scope scope) {
            var depth = scope.Depth;

            foreach (var node in Nodes) {
#if debug_enabled
                if (Debug.BreakpointNodes.Contains(node.Id)) {
                    Debug.SteppingLevel = scope.Depth;
                    Debug.Break(Debug.StopOnBreakpoint, scope.GetStackTrace(Debug.GetNodeLocation(node.Id)), scope.GetScopes());
                } else if (Debug.SteppingInto && depth > Debug.SteppingLevel) {
                    Debug.SteppingLevel++;
                    Debug.SteppingInto = false;
                    Debug.Break(Debug.StopOnBreakpoint, scope.GetStackTrace(Debug.GetNodeLocation(node.Id)), scope.GetScopes());
                } else if (Debug.SteppingOut && depth < Debug.SteppingLevel) {
                    Debug.SteppingLevel--;
                    Debug.SteppingOut = false;
                    Debug.Break(Debug.StopOnBreakpoint, scope.GetStackTrace(Debug.GetNodeLocation(node.Id)), scope.GetScopes());
                } else if (Debug.SteppingOver && depth <= Debug.SteppingLevel) {
                    Debug.SteppingLevel = depth;
                    Debug.SteppingOver = false;
                    Debug.Break(Debug.StopOnBreakpoint, scope.GetStackTrace(Debug.GetNodeLocation(node.Id)), scope.GetScopes());
                }
#endif

                try {
                    if (node is Statement statement) {
                        var result = statement.Execute(scope);
                        if (result.Type == ResultType.None) continue;

                        if (result.Type == ResultType.Return || result.Type == ResultType.Break || result.Type == ResultType.Throw || result.Type == ResultType.Continue) {
#if debug_enabled
                            if (Debug.SteppingOver && depth <= 1) {
                                Debug.Continue();
                            }
#endif

                            return result;
                        }
                    } else if (node is Expression expression) {
                        expression.Execute(scope);
                    }
                } catch (Error e) {
#if debug_enabled
                    Debug.SteppingLevel = scope.Depth;
                    var location = Debug.GetNodeLocation(node.Id);
                    Debug.Break(Debug.StopOnException, scope.GetStackTrace(location), scope.GetScopes());

                    e.AddStackTrace(location);
#endif

                    // Rethrow the error so it keeps traveling up
                    throw;
                }
            }

#if debug_enabled
            if (Debug.SteppingOver && depth <= 1) {
                Debug.Continue();
            }
#endif

            return new Result(ResultType.None);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            for (var i = 0; i < Nodes.Count; i++) {
                var node = Nodes[i];

                if (i > 0) NewLine(builder, depth);
                node.Uneval(builder, depth);

                if (!(node is If || node is For || node is ForOf || node is ForIn || node is While || node is Try)) {
                    builder.Append(Tokens.ExpressionEnd);
                }
            }
        }
    }
}
