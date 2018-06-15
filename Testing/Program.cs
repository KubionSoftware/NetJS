using System;
using System.Collections.Generic;
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
//                Program.NumSuccess++;
            }
            else {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Test failed - " + message);
//                Program.NumFailed++;
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
            }
            catch (Exception e) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(e);
            }

            return NetJS.Core.Static.Undefined;
        }
    }

    class Program{
        public static int NumFailed = 0;
        public static int NumSuccess = 0;
        private static List<Test> _allTests;
        private static int _testCount = 0;
        private static int _benchmarkRounds = 1;
        private static List<double> _benchmarkResults = new List<double>();

        static void Main(string[] args){
            try {
                NumFailed = 0;
                NumSuccess = 0;

                var recursiveFindWatch = new Stopwatch();
                recursiveFindWatch.Start();

                var root = new Directory("test", 1);
                root = root.Walkthrough(System.IO.Path.GetFullPath("../../test/src/262/test" +
//                                                                   "/language" +
//                                                                       "/white-space" +
                                                                   ""), null);
                Console.WriteLine("Got all files!");

                recursiveFindWatch.Stop();

                _allTests = root.Tests;
                var executeWatch = new Stopwatch();
                

                var rounds = 0;
                while (_benchmarkRounds > rounds) {
                    Console.WriteLine("Round: " + (rounds+1));
                    executeWatch.Restart();

                    var taskList = new List<Task>();
                    for (var i = 0; i < Environment.ProcessorCount; i++) {
                        taskList.Add(Task.Factory.StartNew(ExecuteWorker));
                    }

                    Task.WaitAll(taskList.ToArray());

                    executeWatch.Stop();
                    _testCount = 0;
                    _benchmarkResults.Add(executeWatch.Elapsed.TotalSeconds);
                    rounds++;
                    Console.WriteLine(
                        "Elapsed Creation & executionTime (mm:ss):" + executeWatch.Elapsed.ToString(@"mm\:ss"));
                }
                
                var watch = new Stopwatch();
                watch.Start();

                var output = root.ToCSV(root.GetHighestLevel(root) + 1);


                watch.Stop();
                output += "\nElapsed Retrieval Time(mm:ss):," + recursiveFindWatch.Elapsed.ToString(@"mm\:ss");
                output += "\nElapsed Creation & Execution Time (mm:ss):," + executeWatch.Elapsed.ToString(@"mm\:ss");
                output += "\nElapsed Formatting Time (mm:ss):," + watch.Elapsed.ToString(@"mm\:ss");

                Console.WriteLine("Elapsed Retrieval Time(mm:ss):" + recursiveFindWatch.Elapsed.ToString(@"mm\:ss"));
                Console.WriteLine("Elapsed Formatting Time (mm:ss):" + watch.Elapsed.ToString(@"mm\:ss"));
                System.IO.File.WriteAllText(@"../../test\test.csv",
                    output);

                var totalExecutionTime = 0.0;
                foreach (var result in _benchmarkResults) {
                    totalExecutionTime += result;
                }

                Console.WriteLine("Avarage Execution time: " + totalExecutionTime / _benchmarkResults.Count);
                Console.ReadLine();
            }
            catch (Exception e) {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("Error in NetJS: ");
                System.IO.File.WriteAllText(@"../../test\error.txt",
                    e.Message);
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
                Test myTest;

                lock (_allTests) {
                    if (_testCount < _allTests.Count) {
                        myTest = _allTests[_testCount];
                        _testCount++;
                        if(_testCount % 3000 == 0) {
                            Console.WriteLine("We are at number: " + _testCount);
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