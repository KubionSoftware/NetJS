using NetJS.Core.Javascript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetJS.API {
    class Async {

        private static Task[] CreateTasks(Constant[] arguments, Scope scope, string name) {
            var tasks = new Task[arguments.Length];

            for (var i = 0; i < arguments.Length; i++) {
                var function = Core.Tool.GetArgument<Core.Javascript.Function>(arguments, i, name);
                var task = new Task(() => {
                    var functionArguments = new ArgumentList() {
                        Arguments = new List<Expression>() { }
                    };
                    function.Call(functionArguments, Static.Undefined, scope);
                });
                task.Start();
                tasks[i] = task;
            }

            return tasks;
        }

        /// <summary>Runs multiple functions parallel and continues when all are done.</summary>
        /// <param name="function1">A function to execute</param>
        /// <param name="function2">A function to execute</param>
        /// <param name="functionN">A function to execute</param>
        /// <example><code lang="javascript">Async.waitAll(
        ///     () => SQL.execute("query1"),
        ///     () => SQL.execute("query2"),
        ///     () => IO.writeText("big file")
        /// );</code></example>
        public static Constant waitAll(Constant _this, Constant[] arguments, Scope scope) {
            Task.WaitAll(CreateTasks(arguments, scope, "Async.waitAll"));
            return Static.Undefined;
        }

        /// <summary>Runs multiple functions parallel and continues when one of them is done.</summary>
        /// <param name="function1">A function to execute</param>
        /// <param name="function2">A function to execute</param>
        /// <param name="functionN">A function to execute</param>
        /// <example><code lang="javascript">Async.waitAny(
        ///     () => task1(),
        ///     () => task2(),
        ///     () => task3()
        /// );</code></example>
        public static Constant waitAny(Constant _this, Constant[] arguments, Scope scope) {
            Task.WaitAny(CreateTasks(arguments, scope, "Async.waitAny"));
            return Static.Undefined;
        }

        /// <summary>Runs multiple functions parallel and continues immediately. It doesn't wait for them to be done.</summary>
        /// <param name="function1">A function to execute</param>
        /// <param name="function2">A function to execute</param>
        /// <param name="functionN">A function to execute</param>
        /// <example><code lang="javascript">Async.run(
        ///     () => SQL.execute("query1"),
        ///     () => SQL.execute("query2"),
        ///     () => IO.writeText("big file")
        /// );</code></example>
        public static Constant run(Constant _this, Constant[] arguments, Scope scope) {
            CreateTasks(arguments, scope, "Async.run");
            return Static.Undefined;
        }
    }
}
