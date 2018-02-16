using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetJS.Javascript;
using System.Diagnostics;
using NetJS;

namespace Testing {

    class TestFunctions {
        public static Constant assert(Constant _this, Constant[] arguments, Scope scope) {
            Constant value = Static.Undefined;
            string message = "Could not read message";

            try {
                var function = Tool.GetArgument<NetJS.Javascript.InternalFunction>(arguments, 0, "assert");
                message = Tool.GetArgument<NetJS.Javascript.String>(arguments, 1, "assert").Value;

                value = function.Call(new ArgumentList() { Arguments = new List<Expression>() }, null, scope);
            } catch (Exception e) {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Error.WriteLine(e);
            }

            if (value is NetJS.Javascript.Boolean && ((NetJS.Javascript.Boolean)value).Value) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Test successful - " + message);
                Program.NumSuccess++;
            } else {
                
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Test failed - " + message);
                Program.NumFailed++;
            }

            return NetJS.Javascript.Static.Undefined;
        }
    }

    class Program {

        public static int NumFailed = 0;
        public static int NumSuccess = 0;

        static void Main(string[] args) {
            var service = new JSService();
            var application = new JSApplication("../../../JSDoc/test/");

            application.Global.RegisterFunctions(typeof(TestFunctions));

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
        }
    }
}
