using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetJS.Core.Javascript;
using System.Diagnostics;
using NetJS;

namespace Testing {
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
        public static List<Test> AllTests;
        public static int TestCount = 0;


        static void Main(string[] args){
            try {
                var service = new JSService();
                var application = new JSApplication("../../test/");

                NumFailed = 0;
                NumSuccess = 0;

                var recursiveFindWatch = new Stopwatch();
                recursiveFindWatch.Start();

                Directory root = new Directory("test", 1);
                root = root.Walkthrough(System.IO.Path.GetFullPath("../../test/src/262/test" +
//                                                                   "/language" +
//                                                                       "/white-space" +
                                                                   ""), null);
                Console.WriteLine("Got all files!");

                recursiveFindWatch.Stop();
//                Console.WriteLine("tests collected in(s): " + recursiveFindWatch.Elapsed.TotalSeconds);

                var session = new JSSession();

                AllTests = root.Tests;

                var executeWatch = new Stopwatch();
                executeWatch.Start();

                var taskList = new List<Task>();
                for (var i = 0; i < System.Environment.ProcessorCount; i++) {
                    taskList.Add(Task.Factory.StartNew(ExecuteWorker));
                }

                Task.WaitAll(taskList.ToArray());

                executeWatch.Stop();
                var watch = new Stopwatch();
                watch.Start();

                var output = root.ToCSV(root.GetHighestLevel(root) + 1);
//                Console.WriteLine(output);


                watch.Stop();
                output += "\nElapsed Retrieval Time(mm:ss):," + recursiveFindWatch.Elapsed.ToString(@"mm\:ss");
                output += "\nElapsed Creation & Execution Time (mm:ss):," + executeWatch.Elapsed.ToString(@"mm\:ss");
                output += "\nElapsed Formatting Time (mm:ss):," + watch.Elapsed.ToString(@"mm\:ss");

                Console.WriteLine("Elapsed Retrieval Time(mm:ss):" + recursiveFindWatch.Elapsed.ToString(@"mm\:ss"));
                Console.WriteLine(
                    "Elapsed Creation & executionTime (mm:ss):" + executeWatch.Elapsed.ToString(@"mm\:ss"));
                Console.WriteLine("Elapsed Formatting Time (mm:ss):" + watch.Elapsed.ToString(@"mm\:ss"));
                System.IO.File.WriteAllText(@"C:\Users\Mitch\ProjectWorkspace\Kubion\NetJS\Testing\test\test.csv",
                    output);


                Console.ReadLine();
            }
            catch (Exception e) {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("Error in NetJS: ");
                System.IO.File.WriteAllText(@"C:\Users\Mitch\ProjectWorkspace\Kubion\NetJS\Testing\test\error.txt",
                    e.Message.ToString());
                Console.WriteLine(e);
            }

            Console.ReadLine();
        }

        private static void ExecuteWorker(){
            var service = new JSService();
            var session = new JSSession();
            var application = new JSApplication("../../test/");

            service.RunTemplate("262/harness/sta.js", "{}", ref application, ref session);
            service.RunTemplate("262/harness/assert.js", "{}", ref application, ref session);

            while (true) {
                Test myTest = null;

                lock (AllTests) {
                    if (TestCount < AllTests.Count) {
                        myTest = AllTests[TestCount];
                        TestCount++;
                        if(TestCount % 1000 == 0) {
                            Console.WriteLine("We are at number: " + TestCount);
                        }
                    }
                    else {
                        return;
                    }
                }

                if (myTest == null) continue;
                myTest.Initialize();
                myTest.Execute(application, service, session);
                
            }
        }
    }
}