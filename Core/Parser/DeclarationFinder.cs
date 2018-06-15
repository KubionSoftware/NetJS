using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    class DeclarationFinder {

        public static void FindVarDeclarations(Statement statement, Action<Constant, bool> action) {
            var declaredVarNames = new List<Constant>();

            Walker.Walk(statement, node => {
                if (node is FunctionLiteral || node is ClassLiteral) return null;

                if (node is VariableDeclaration d && d.Scope != DeclarationScope.Block) {
                    foreach (var dn in d.GetBoundNames()) {
                        if (declaredVarNames.Contains(dn)) continue;
                        declaredVarNames.Add(dn);

                        action(dn, d.IsConstant);
                    }
                }

                return node;
            });
        }
    }
}
