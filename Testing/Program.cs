using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetJS.Core;
using System.Diagnostics;
using NetJS;

namespace Testing {

    class TestFunctions {
        public static Constant assert(Constant _this, Constant[] arguments, Agent agent) {
            Constant value = Static.Undefined;
            string message = "Could not read message";

            try {
                var function = NetJS.Core.Tool.GetArgument<NetJS.Core.InternalFunction>(arguments, 0, "assert");
                message = NetJS.Core.Tool.GetArgument<NetJS.Core.String>(arguments, 1, "assert").Value;

                value = function.Call(Static.Undefined, agent, new Constant[] { });
            } catch (Exception e) {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Error.WriteLine(e);
            }

            if (value is NetJS.Core.Boolean b && b.Value) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Test successful - " + message);
                Program.NumSuccess++;
            } else {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Test failed - " + message);
                Program.NumFailed++;
            }

            return NetJS.Core.Static.Undefined;
        }

        public static Constant time(Constant _this, Constant[] arguments, Agent agent) {
            string message = "Could not read message";

            try {
                var function = NetJS.Core.Tool.GetArgument<NetJS.Core.InternalFunction>(arguments, 0, "assert");
                message = NetJS.Core.Tool.GetArgument<NetJS.Core.String>(arguments, 1, "assert").Value;

                var watch = new Stopwatch();
                watch.Start();
                function.Call(Static.Undefined, agent, new Constant[] { });
                watch.Stop();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{message} took {watch.ElapsedMilliseconds}ms");
            } catch (Exception e) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(e);
            }

            return NetJS.Core.Static.Undefined;
        }
    }

    class Program {

        public static int NumFailed = 0;
        public static int NumSuccess = 0;

        static void Main(string[] args) {
            try {
                var service = new JSService();
                var application = new JSApplication("../../test/");

                application.Realm.RegisterFunctions(typeof(TestFunctions));

                while (true) {
                    NumFailed = 0;
                    NumSuccess = 0;

                    var watch = new Stopwatch();
                    watch.Start();

                    var session = new JSSession();
                    var output = service.RunTemplate("main.js", "{}", ref application, ref session);

                    watch.Stop();

                    Console.ForegroundColor = NumFailed == 0 ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.WriteLine($"Completed test with {NumFailed} failures and {NumSuccess} successes");

                    Console.ForegroundColor = ConsoleColor.White;

                    Console.WriteLine("Output: ");
                    Console.WriteLine(output);

                    Console.WriteLine("Time: " + watch.ElapsedMilliseconds + "ms");

                    Console.ReadLine();
                }
            } catch(Exception e) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error in NetJS: ");
                Console.WriteLine(e);
            }

            Console.ReadLine();
        }
    }
}
