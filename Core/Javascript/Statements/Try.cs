using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Try : Statement {
        public Block TryBody;
        public string CatchVariable;
        public Block CatchBody;
        public Block FinallyBody;

        public override Result Execute(Scope scope) {
            Result result;

            try {
                result = TryBody.Execute(scope);
                if (result.Type != ResultType.Throw) {
                    return result;
                }
            } catch (Exception e) {
                result = new Result(ResultType.Throw, new String(e.ToString()));
            }

            if (CatchBody != null) {
                if (CatchVariable != null) scope.DeclareVariable(CatchVariable, DeclarationScope.Block, false, result.Constant);
                var catchResult = CatchBody.Execute(scope);
                if (FinallyBody == null || catchResult.Type == ResultType.Return || catchResult.Type == ResultType.Throw) {
                    return catchResult;
                }
            }

            if (FinallyBody != null) {
                return FinallyBody.Execute(scope);
            }

            return new Result(ResultType.None);
        }
    }
}
