using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetJS.Core.Javascript;
using NetJS.Core;
using System.Diagnostics;
using System.IO;
using NetJS;

namespace Testing{
    class TestFunctions{
        public static Constant assert(Constant _this, Constant[] arguments, Scope scope){
            Constant value = Static.Undefined;
            string message = "Could not read message";

            try {
                var function =
                    NetJS.Core.Tool.GetArgument<NetJS.Core.Javascript.InternalFunction>(arguments, 0, "assert");
                message = NetJS.Core.Tool.GetArgument<NetJS.Core.Javascript.String>(arguments, 1, "assert").Value;

                value = function.Call(new ArgumentList(), null, scope);
            }
            catch (Exception e) {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Error.WriteLine(e);
            }

            if (value is NetJS.Core.Javascript.Boolean b && b.Value) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Test successful - " + message);
                Program.NumSuccess++;
            }
            else {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Test failed - " + message);
                Program.NumFailed++;
            }

            return NetJS.Core.Javascript.Static.Undefined;
        }

        public static Constant time(Constant _this, Constant[] arguments, Scope scope){
            string message = "Could not read message";

            try {
                var function =
                    NetJS.Core.Tool.GetArgument<NetJS.Core.Javascript.InternalFunction>(arguments, 0, "assert");
                message = NetJS.Core.Tool.GetArgument<NetJS.Core.Javascript.String>(arguments, 1, "assert").Value;

                var watch = new Stopwatch();
                watch.Start();
                function.Call(new ArgumentList(), null, scope);
                watch.Stop();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{message} took {watch.ElapsedMilliseconds}ms");
            }
            catch (Exception e) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(e);
            }

            return NetJS.Core.Javascript.Static.Undefined;
        }
    }

    class Program{
        public static int NumFailed = 0;
        public static int NumSuccess = 0;

        static void Main(string[] args){
            try {
                var service = new JSService();
                var application = new JSApplication("../../test/");

//                application.Engine.RegisterFunctions(typeof(TestFunctions));


                while (true) {
                    NumFailed = 0;
                    NumSuccess = 0;

                    var recursiveFindWatch = new Stopwatch();
                    recursiveFindWatch.Start();
                    
                    Directory root = new Directory("test", 1);
                    root = root.Walkthrough(System.IO.Path.GetFullPath("../../test/src/262/test" +
//                                                                       "/language" +
//                                                                       "/white-space" +
                                                                       ""));

                    recursiveFindWatch.Stop();
                    Console.WriteLine("tests collected in(s): " + recursiveFindWatch.Elapsed.TotalSeconds);
                    
                    var watch = new Stopwatch();
                    watch.Start();

                    var session = new JSSession();
                    service.RunTemplate("262/harness/sta.js", "{}", ref application, ref session);
                    service.RunTemplate("262/harness/assert.js", "{}", ref application, ref session);

                    var output = root.ToCSV(System.IO.Path.GetFullPath("../../test/src/262/test" +
//                                                                       "/language" +
//                                                                       "/white-space" +
                                                                       ""), root.GetHighestLevel(root) + 1, service,
                        application, session);


                    watch.Stop();
                    var time = watch.Elapsed;
                    output += "\nElapsed Retrieval Time(mm:ss):," + recursiveFindWatch.Elapsed.ToString(@"mm\:ss");
                    output += "\nElapsed Execution & Formatting Time (mm:ss):," + time.ToString(@"mm\:ss");
//                    Console.WriteLine(output);
                    Console.WriteLine("Elapsed Retrieval Time(mm:ss):" + recursiveFindWatch.Elapsed.ToString(@"mm\:ss"));
                    Console.WriteLine("Elapsed Execution & Formatting Time (mm:ss):" + time.ToString(@"mm\:ss"));
                    System.IO.File.WriteAllText(@"C:\Users\Mitch\ProjectWorkspace\Kubion\NetJS\Testing\test\test.csv",
                        output);


                    Console.ReadLine();
                }
            }
            catch (Exception e) {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("Error in NetJS: ");
                Console.WriteLine(e);
            }

            Console.ReadLine();
        }
    }
}