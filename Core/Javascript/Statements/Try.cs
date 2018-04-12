using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Try : Statement {
        public Block TryBody;
        public Variable CatchVariable;
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
                result = new Result(ResultType.Throw, new String(e.Message));
            }

            if (CatchBody != null) {
                if (CatchVariable != null) CatchVariable.Assignment(result.Constant, scope);
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

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.Try + Tokens.BlockOpen);
            NewLine(builder, depth + 1);
            TryBody.Uneval(builder, depth + 1);
            NewLine(builder, depth);
            builder.Append(Tokens.BlockClose);

            if (CatchBody != null) {
                builder.Append(Tokens.Catch);

                if (CatchVariable != null) {
                    builder.Append(Tokens.GroupOpen);
                    CatchVariable.Uneval(builder, depth);
                    builder.Append(Tokens.GroupClose);
                }

                builder.Append(Tokens.BlockOpen);
                NewLine(builder, depth + 1);
                CatchBody.Uneval(builder, depth + 1);
                NewLine(builder, depth);
                builder.Append(Tokens.BlockClose);
            }

            if (FinallyBody != null) {
                builder.Append(Tokens.Finally + Tokens.BlockOpen);
                NewLine(builder, depth + 1);
                FinallyBody.Uneval(builder, depth + 1);
                NewLine(builder, depth);
                builder.Append(Tokens.BlockClose);
            }
        }
    }
}
