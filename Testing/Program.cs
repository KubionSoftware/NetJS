using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using NetJS;
using System.Linq;

namespace NetJS.Testing {
    
    public class Test {
        public static void assert(dynamic func, string message) {
            object value = null;

            try {
                value = func();
            } catch (Exception e) {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Error.WriteLine(e);
            }

            if (value is bool b && b) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Test successful - " + message);
                Program.NumSuccess++;
            }
            else {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Test failed - " + message);
                Program.NumFailed++;
            }
        }

        public static void time(dynamic func, string message) {
            try {
                var watch = new Stopwatch();
                watch.Start();
                func();
                watch.Stop();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{message} took {watch.ElapsedMilliseconds}ms");
            }
            catch (Exception e) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(e);
            }
        }
    }

    class Program{
        public static int NumFailed = 0;
        public static int NumSuccess = 0;
        //private static List<TestFile> _allTests;
        private static int _testCount = 0;
        private static int _benchmarkRounds = 1;
        private static List<double> _benchmarkResults = new List<double>();

        public static string Test262Root = System.IO.Path.GetFullPath("../../test262/test");

        static void Main(string[] args){
            try {
                FeatureTest();
                //Test262();
            }
            catch (Exception e) {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("Error in NetJS: ");
                System.IO.File.WriteAllText(@"error.txt", e.Message);
                Console.WriteLine(e);
            }
            
            Console.ReadLine();
        }

        static void FeatureTest() {
            while (true) {
                var service = new JSService();
                var application = new JSApplication("../../test/", app => {
                    app.AddHostType(typeof(Test));
                }, error => {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(error.ToString());
                });

                NumFailed = 0;
                NumSuccess = 0;

                var watch = new Stopwatch();
                watch.Start();

                var session = new JSSession();
                service.RunScript("main.js", application, session, output => {
                    watch.Stop();

                    Console.ForegroundColor = NumFailed == 0 ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.WriteLine($"Completed test with {NumFailed} failures and {NumSuccess} successes");

                    Console.ForegroundColor = ConsoleColor.White;

                    Console.WriteLine("Output: ");
                    Console.WriteLine(output);

                    Console.WriteLine("Time: " + watch.ElapsedMilliseconds + "ms");
                });

                Console.ReadLine();
            }
        }

        /*
        static void Test262() {
            var recursiveFindWatch = new Stopwatch();
            recursiveFindWatch.Start();

            var root = new Directory("test", 1);
            root = root.Walkthrough(Test262Root, null);
            Console.WriteLine("Got all files!");

            recursiveFindWatch.Stop();

            _allTests = root.Tests;

            var executeWatch = new Stopwatch();


            var rounds = 0;
            while (_benchmarkRounds > rounds) {
                Console.WriteLine("Round: " + (rounds + 1));
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

            while (true) {
                try {
                    System.IO.File.WriteAllText(@"test.csv", output);
                    break;
                } catch {
                    Console.Error.WriteLine("Close csv file. Then press enter");
                    Console.ReadLine();
                }
            }

            var totalExecutionTime = 0.0;
            foreach (var result in _benchmarkResults) {
                totalExecutionTime += result;
            }

            Console.WriteLine("Avarage Execution time: " + totalExecutionTime / _benchmarkResults.Count);
        }

        private static void ExecuteWorker(){
            var service = new JSService();
            var session = new JSSession();
            var application = new JSApplication(Test262Root);

            service.RunScript(Test262Root + "/../harness/sta.js", application, session, true, false);
            service.RunScript(Test262Root + "/../harness/assert.js", application, session, true, false);

            while (true) {
                TestFile myTest;

                lock (_allTests) {
                    if (_testCount < _allTests.Count) {
                        myTest = _allTests[_testCount];
                        _testCount++;
                        if(_testCount % 1000 == 0) {
                            Console.WriteLine("We are at number: " + _testCount);
                        }
                    }
                    else {
                        return;
                    }
                }

                if (myTest == null) continue;

                try {
                    myTest.Initialize();
                    myTest.Execute(application, service, session);
                }catch(Exception e) {
                    Console.Error.WriteLine(e);
                }
            }
        }
        */
    }
}