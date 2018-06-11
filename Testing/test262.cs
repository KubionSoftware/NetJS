using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NetJS;
using YamlDotNet.RepresentationModel;

namespace Testing{
    public class Directory{
        private string name;
        private List<Directory> Directories;
        private int level;

        private List<Test> Files;

        public Directory(string s, int level){
            name = s;
            this.level = level;
            Directories = new List<Directory>();
            Files = new List<Test>();
        }

        public Directory Walkthrough(string path){
            foreach (var file in System.IO.Directory.GetFiles(path)) {
                
                var f = new Test(file);
                Console.WriteLine("found test: " + f.path);
                Files.Add(f);
            }

            foreach (var dir in System.IO.Directory.GetDirectories(path)) {
                var dirPath = dir.Split('\\');
                var subDirectory = new Directory(dirPath[dirPath.Length - 1], level + 1);
                Directories.Add(subDirectory.Walkthrough(dir));
            }

            return this;
        }

        public int GetHighestLevel(Directory d){
            var level = this.level;
            if (d == null) return level;
            foreach (var directory in d.Directories) {
                var newlevel = directory.GetHighestLevel(this);
                if (newlevel > level) {
                    level = newlevel;
                }
            }

            if (d.level > level) {
                level = d.level;
            }

            return level;
        }

        public string ToCSV(string basePath, int level, JSService service, JSApplication application,
            JSSession session){
            var countSuccesful = 0;
            var countTotal = 0;
            var countUndefined = 0;
            var countFailed = 0;
            var answer = new List<string>();
            var allFiles = GetFiles();
            
            answer.Add("FOLDER" + new StringBuilder().Insert(0, ",SUBFOLDER", level-2) + ", FILE, RESULT, TIME (ms), OUTPUT");

            foreach (var file in allFiles) {
                if (file != null) {
                    file.Execute(application, service, session);
                    answer.Add(file.GetCsv(level));
                    if (file.implemented.Count < 1) {
                        if (file.useNonStrict) {
                            if (file.nonStrictResult) {
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
            var pSuccesful = Math.Round((double) countSuccesful * 100 / countTotal, 2);
            var pUndefined = Math.Round((double) countUndefined * 100 / countTotal, 2);
            var pFailed = Math.Round((double) countFailed * 100 / countTotal, 2);
            answer.Add("%succesful," + pSuccesful);
            answer.Add("%undefined," + pUndefined);
            answer.Add("%failed," + pFailed);
            
            return string.Join("\r\n", answer);
        }

        private IEnumerable<Test> GetFiles(){
            var answer = new List<Test>();
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

    public class Test{
        public string path;
        private string[] splittedPath;
        private string name;
        private bool useStrict;
        public bool useNonStrict;
        private List<string> include;
        public string strictOutput;
        private string nonStrictOutput = "";
        private string information;
        private YamlNode es5id;
        private YamlNode description;
        private YamlNode negative;
        private YamlNode negativePhase;
        private YamlNode negativeType;
        private List<string> flags;
        public List<string> implemented = new List<string>();
        private string nonStrictTime;
        public bool nonStrictResult;

        public Test(string filePath){
            path = filePath.Replace('\\', '/');
            splittedPath = path.Split('/');
            name = splittedPath[splittedPath.Length - 1];
            useNonStrict = true;
            var excludeWords = new[] {"onlyStrict", "async","print", "$262", "createRealm", "detachArrayBuffer", "evalScript", "global.", "IsHTMLDDA", "document.all", "agent"};

            var readText = File.ReadAllText(path);
            foreach (var word in excludeWords) {
                if (readText.Contains(word)) {
                    implemented.Add(word);
                }
            }

            if (implemented.Count >= 1) return;
            var configRGX = new Regex(@"(?<=\/\*---)[\w\W]+(?=---\*\/)");
            var match = configRGX.Match(readText);
            if (match.Value == "") return;
            var yaml = new YamlStream();
            var yamlInput = new StringReader(match.Groups[0].Value);
            yaml.Load(yamlInput);

            if (yaml.Documents.Count <= 0) return;
            var mapping = (YamlMappingNode) yaml.Documents[0].RootNode;

            mapping.Children.TryGetValue("description", out description);
            mapping.Children.TryGetValue("es5id", out es5id);
            mapping.Children.TryGetValue("negative", out negative);
            mapping.Children.TryGetValue("includes", out var tempInclude);
            if (tempInclude != null) {
                include = tempInclude.ToString().Split(' ').ToList();
            }

            mapping.Children.TryGetValue("flags", out var tempFlag);
            if (tempFlag != null) {
                flags = tempFlag.ToString().Split(',').ToList();
            }

            if (negative != null) {
                var into = "";
                foreach (var node in negative.AllNodes) {
                    switch (@into) {
                        case "phase":
                            negativePhase = node;
                            break;

                        case "type":
                            negativeType = node;
                            break;
                    }

                    @into = node.ToString();
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


        public void Execute(JSApplication application, JSService service, JSSession session){
            if (implemented.Count >= 1) return;
            Console.WriteLine("executing: " + path);
            var watch = new Stopwatch();
            watch.Start();
            var preTestOutput = "";
            if (include != null) {
                foreach (var preTest in include) {
                    if (!preTest.Contains(".js")) continue;
                    var preTestPath = Path.GetFullPath("../../test/src/262/harness/" + preTest.Replace(",", "")).Replace('\\', '/');
                    Console.WriteLine("preTest: " + preTestPath);
                    preTestOutput += service.RunTemplate(preTestPath, "{}", ref application, ref session);
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
                timeDecimal = (int.Parse(stringTime[1]) * 6 / 10).ToString();
            }
            nonStrictTime = stringTime[0] + "." + timeDecimal;
        }

        public string GetCsv(int level){
            var answer = "";
            if (!useNonStrict) return answer;
            var fullpath = Path.GetFullPath("../../test/src/262/test");
            var splitpath = fullpath.Length + 1;
            var dirPathSplitted = fullpath.Split('\\');
            level = level + dirPathSplitted.Length;

            var dirPath = string.Join(",", splittedPath, 0, splittedPath.Length - 1);
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
                    preOutput = "[negative:" + negativeType + "] ";
                }
                    
                var rgx4 = new Regex(@"([\r\n])+");
                    
                answer += "," + nonStrictResult + "," + nonStrictTime + "," + "\"" + rgx4.Replace(preOutput.Replace('"', '\'') + nonStrictOutput.Replace('"', '\''), " ") + "\"," + path.Replace('/', '\\');
            }
            else {
                answer += "," + "undefined" + ",," +
                          "\"the following features are not implemented: " + string.Join(",", implemented.ToArray()) + "\"," + path.Replace('/', '\\');
            }
            return answer;
        }
    }
}