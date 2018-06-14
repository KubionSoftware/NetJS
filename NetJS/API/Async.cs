using NetJS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetJS.API {
    class Async {

        private static Task[] CreateTasks(Constant[] arguments, Agent agent, string name) {
            var tasks = new Task[arguments.Length];

            for (var i = 0; i < arguments.Length; i++) {
                var function = Core.Tool.GetArgument<Core.Function>(arguments, i, name);
                var task = new Task(() => {
                    try {
                        function.Call(Static.Undefined, agent, new Constant[] { });
                    } catch (Exception e) {
                        Core.Log.Write(e.ToString());
                    }
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
        public static Constant waitAll(Constant _this, Constant[] arguments, Agent agent) {
            Task.WaitAll(CreateTasks(arguments, agent, "Async.waitAll"));
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
        public static Constant waitAny(Constant _this, Constant[] arguments, Agent agent) {
            Task.WaitAny(CreateTasks(arguments, agent, "Async.waitAny"));
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
        public static Constant run(Constant _this, Constant[] arguments, Agent agent) {
            CreateTasks(arguments, agent, "Async.run");
            return Static.Undefined;
        }

        /// <summary>Sleeps for a given number of milliseconds.</summary>
        /// <param name="time">Milliseconds to sleep (number)</param>
        /// <example><code lang="javascript">Async.sleep(1000);</code></example>
        public static Constant sleep(Constant _this, Constant[] arguments, Agent agent) {
            var time = Core.Tool.GetArgument<Core.Number>(arguments, 0, "Async.sleep");
            Thread.Sleep((int)time.Value);
            return Static.Undefined;
        }
    }
}
