using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using NetJS;
using NetJS.Core.Javascript;
using YamlDotNet.RepresentationModel;
using Boolean = System.Boolean;
using String = System.String;

namespace Testing{
    public class Directory{
        public string name;
        public List<Directory> Directories;
        public int level;

        public List<File> Files;

        public Directory(string s, int level){
            this.name = s;
            this.level = level;
            this.Directories = new List<Directory>();
            this.Files = new List<File>();
        }

        public Directory Walkthrough(string path){
            foreach (var file in System.IO.Directory.GetFiles(path)) {
                var f = new File(file);
                this.Files.Add(f);
            }

            foreach (var dir in System.IO.Directory.GetDirectories(path)) {
                var dirPath = dir.Split('\\');
                var subDirectory = new Directory(dirPath[dirPath.Length - 1], this.level + 1);
                this.Directories.Add(subDirectory.Walkthrough(dir));
            }

            return this;
        }

        public String ToString(){
            var s = "name: " + name;
            foreach (var directory in Directories) {
                s += "\r\n" + directory.level + directory.ToString();
            }

            return s;
        }

        public int GetHighestLevel(Directory d){
            var level = this.level;
            if (d != null) {
                foreach (var directory in d.Directories) {
                    var newlevel = directory.GetHighestLevel(this);
                    if (newlevel > level) {
                        level = newlevel;
                    }
                }

                if (d.level > level) {
                    level = d.level;
                }
            }

            return level;
        }

        public String ToCSV(string basePath, int level, JSService service, JSApplication application,
            JSSession session){
            var countSuccesful = 0;
            var countTotal = 0;
            var countUndefined = 0;
            var countFailed = 0;
            List<string> answer = new List<string>();
            List<File> allFiles = GetFiles();
            
            answer.Add("FOLDER" + new StringBuilder().Insert(0, ",SUBFOLDER", level-2) + ", FILE, RESULT, TIME (ms), OUTPUT");

            foreach (var file in allFiles) {
                if (file != null) {
                    var myTest = new Test(file);
                    myTest.Execute(application, service, session);
                    answer.Add(myTest.GetCsv(level));
                    if (myTest.implemented.Count < 1) {
                        if (myTest.useNonStrict) {
                            if (myTest.nonStrictResult) {
                                countSuccesful++;
                            }
                            else {
                                countFailed++;
                            }
                        }
                    }
                    else {
                        countUndefined++;
                    }
                    countTotal++;
                }
                else {
                    answer.Add("");
                }
            }

            answer.Add("total tests:," + countTotal);
            answer.Add("succesful," + countSuccesful);
            answer.Add("not implemented," + countUndefined);
            answer.Add("failed," + countFailed);
            answer.Add("");
            double pSuccesful = Math.Round((double) countSuccesful * 100 / countTotal, 2);
            var pUndefined = Math.Round((double) countUndefined * 100 / countTotal, 2);
            var pFailed = Math.Round((double) countFailed * 100 / countTotal, 2);
            answer.Add("%succesful," + pSuccesful);
            answer.Add("%undefined," + pUndefined);
            answer.Add("%failed," + pFailed);
            
            return String.Join("\r\n", answer);
        }

        public List<File> GetFiles(){
            List<File> answer = new List<File>();
            foreach (var file in Files) {
                answer.Add(file);
            }

            if (Files.Count > 0 && Directories.Count > 0) {
                answer.Add(null);
            }

            foreach (var directory in Directories) {
                answer.AddRange(directory.GetFiles());
                answer.Add(null);
            }

            return answer;
        }
    }

    public class File{
        public string name;

        public File(string file){
            this.name = file;
        }
    }

    public class Test{
        private string path;
        private string[] splittedPath;
        private string name;
        public bool useStrict = false;
        public bool useNonStrict = false;
        private List<string> include;
        public string strictOutput;
        public string nonStrictOutput = "";
        private string information;
        private YamlNode es5id;
        private YamlNode description;
        private YamlNode negative;
        private YamlNode negativePhase;
        private YamlNode negativeType;
        private List<string> flags;
        public List<string> implemented = new List<string>();
        public string nonStrictTime;
        private static string test;
        public bool nonStrictResult = false;

        public Test(File file){
            this.path = file.name.Replace("\\", "/");
            splittedPath = path.Split('/');
            this.name = splittedPath[splittedPath.Length - 1];
            useNonStrict = true;
            var excludeWords = new[] {"onlyStrict", "async","print", "$262", "createRealm", "detachArrayBuffer", "evalScript", "global", "IsHTMLDDA", "document.all", "agent"};

            var readText = System.IO.File.ReadAllText(path);
            foreach (var word in excludeWords) {
                if (readText.Contains(word)) {
                    implemented.Add(word);
                }
            }

            if (implemented.Count < 1) {
                var configRGX = new Regex(@"(?<=\/\*---)[\w\W]+(?=---\*\/)");
                var match = configRGX.Match(readText);
                if (match.Value != "") {
                    var yaml = new YamlStream();
                    var yamlInput = new StringReader(match.Groups[0].Value);
                    yaml.Load(yamlInput);

                    YamlMappingNode mapping = null;

                    if (yaml.Documents.Count > 0) {
                        mapping = (YamlMappingNode) yaml.Documents[0].RootNode;

                        mapping.Children.TryGetValue("description", out description);
                        mapping.Children.TryGetValue("es5id", out es5id);
                        mapping.Children.TryGetValue("negative", out negative);
                        YamlNode tempInclude = null;
                        mapping.Children.TryGetValue("includes", out tempInclude);
                        if (tempInclude != null) {
//                            Console.WriteLine(tempInclude);
                            include = tempInclude.ToString().Split(new char[] {' '}).ToList();
                        }

                        YamlNode tempFlag = null;
                        mapping.Children.TryGetValue("flags", out tempFlag);
                        if (tempFlag != null) {
                            flags = tempFlag.ToString().Split(new char[] {','}).ToList();
                        }

                        if (flags != null) {
                            test = flags.ToString();
                        }

                        if (negative != null) {
                            var into = "";
                            foreach (var node in negative.AllNodes) {
                                switch (into) {
                                    case "phase":
                                        negativePhase = node;
                                        break;

                                    case "type":
                                        negativeType = node;
                                        break;
                                }

                                into = node.ToString();
                            }
                        }

                        if (flags != null && flags.Count > 0) {
                            foreach (var flag in flags) {
                                switch (flag) {
                                    case "onlyStrict":
                                        useStrict = true;
                                        break;
                                    case "noStrict":
                                        useNonStrict = true;
                                        break;
                                    case "module":
                                        useNonStrict = true;
                                        break;
                                    case "raw":
                                        useNonStrict = true;
                                        break;
                                }
                            }
                        }
                        else {
                            useNonStrict = true;
                            useStrict = true;
                        }
                    }
                    else {
//                    throw new Error("juist.." + path + "." +
//                                    match.Value);
                    }
                }
            }

//            Console.WriteLine(match.Value);
        }


        public void Execute(JSApplication application, JSService service, JSSession session){
            if (implemented.Count < 1) {
                Console.WriteLine("executing: " + path);
                var watch = new Stopwatch();
                watch.Start();
                var preTestOutput = "";
                if (include != null) {
                    foreach (var preTest in include) {
                        if (preTest.Contains(".js")) {
                            var preTestPath = System.IO.Path.GetFullPath("../../test/src/262/harness/" + preTest.Replace(",", "")).Replace('\\', '/');
                            Console.WriteLine("preTest: " + preTestPath);
                            preTestOutput += service.RunTemplate(preTestPath, "{}", ref application, ref session);
                        }
                    }
                }
                var testOutput = service.RunTemplate(path, "{}", ref application, ref session);
                watch.Stop();

                if (negative != null && negativeType != null && negativeType.ToString().Length > 0) {
                    var splitOutput = testOutput.Split(' ');
                    var comparewith = "";
                    if (splitOutput.Length > 1) {
                        comparewith = splitOutput[0] + splitOutput[1][0].ToString().ToUpper() +
                                      splitOutput[1].Substring(1);
//                            Console.WriteLine(comparewith);
                    }
                    if (comparewith.Contains(negativeType.ToString())) {
                        nonStrictResult = true;
                    }
                } else if (testOutput.Length < 1) {
                    nonStrictResult = true;
                }

                if (preTestOutput.Length > 0) {
                    nonStrictOutput += "preTest: " + preTestOutput + " | " ;
                }

                nonStrictOutput += testOutput;
                var stringTime = Math.Round(watch.Elapsed.TotalMilliseconds, 2).ToString().Split('.');
                var timeDecimal = "00";
                if (stringTime.Length == 2) {
                    timeDecimal = (Int32.Parse(stringTime[1]) * 6 / 10).ToString();
                }
                nonStrictTime = stringTime[0] + "." + timeDecimal;
//                Console.WriteLine(nonStrictTime);
            }
        }

        public string GetCsv(int level){
            string answer = "";
            if (useNonStrict) {
                var fullpath = System.IO.Path.GetFullPath("../../test/src/262/test");
                var splitpath = fullpath.Length + 1;
                string[] dirPathSplitted = fullpath.Split('\\');
                level = level + dirPathSplitted.Length;

                var dirPath = String.Join(",", splittedPath, 0, splittedPath.Length - 1);
                answer += dirPath.Substring(splitpath);

                if (level > splittedPath.Length - 1) {
                    answer += new string(',', (level - splittedPath.Length)+1);
                }
                else {
                    answer += ",";
                }


                answer += name;
                if (implemented.Count < 1) {
                    var preOutput = "";
                    if (negative != null&& negativeType != null) {
                        preOutput = "[negative:" + negativeType.ToString() + "] ";
                    }
                    
                    var rgx4 = new Regex(@"([\r\n])+");
                    
                    answer += "," + nonStrictResult + "," + nonStrictTime + "," + "\"" + rgx4.Replace(preOutput.Replace('"', '\'') + nonStrictOutput.Replace('"', '\''), " ") + "\"";
                }
                else {
                    answer += "," + "undefined" + ",," +
                              "\"the following features are not implemented: " + string.Join(",", implemented.ToArray()) + "\"";
                }
            }
            return answer;
        }
    }
}