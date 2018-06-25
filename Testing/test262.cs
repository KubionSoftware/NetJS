using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NetJS;
using YamlDotNet.RepresentationModel;

namespace NetJS.Testing {
    public class Directory {
        private List<Directory> _directories;
        private readonly int _level;
        public List<Test> Tests = new List<Test>();

        public Directory(string s, int level) {
            this._level = level;
            _directories = new List<Directory>();
        }

        public Directory Walkthrough(string path, Directory root) {
            var myRoot = root ?? this;
            var allFiles = System.IO.Directory.GetFiles(path);
            foreach (var file in allFiles) {
                var test = new Test(file);
                myRoot.Tests.Add(test);
            }

            if (allFiles.Length > 0) {
                myRoot.Tests.Add(null);
            }

            foreach (var dir in System.IO.Directory.GetDirectories(path)) {
                var dirPath = dir.Split('\\');
                var subDirectory = new Directory(dirPath[dirPath.Length - 1], _level + 1);
                _directories.Add(subDirectory.Walkthrough(dir, myRoot));
            }

            return this;
        }

        public void ExecuteTests(JSApplication application, JSService service, JSSession session) {
            foreach (var test in Tests) {
                if (test == null) continue;
                test.Initialize();
                test.Execute(application, service, session);
            }
        }

        public int GetHighestLevel(Directory d) {
            var level = this._level;
            if (d == null) return level;
            foreach (var directory in d._directories) {
                var newlevel = directory.GetHighestLevel(this);
                if (newlevel > level) {
                    level = newlevel;
                }
            }

            if (d._level > level) {
                level = d._level;
            }

            return level;
        }

        public string ToCSV(int level) {
            var countSuccesful = 0;
            var countTotal = 0;
            var countUndefined = 0;
            var countFailed = 0;
            var answer = new List<string> {
                "FOLDER" + new StringBuilder().Insert(0, ",SUBFOLDER", level - 2) + ", FILE, RESULT, TIME (ms), OUTPUT"
            };


            foreach (var test in Tests) {
                if (test != null) {
                    answer.Add(test.GetCsv(level));
                    if (test.Implemented.Count < 1) {
                        if (test.UseNonStrict) {
                            if (test.NonStrictResult) {
                                countSuccesful++;
                            } else {
                                countFailed++;
                            }
                        }
                        if (test.UseStrict) {
                            if (test.StrictResult) {
                                countSuccesful++;
                            } else {
                                countFailed++;
                            }
                        }
                    } else {
                        countUndefined++;
                    }

                    countTotal += test.TotalTests;
                } else {
                    answer.Add("");
                }
            }

            answer.Add("total tests:," + countTotal);
            answer.Add("succesful," + countSuccesful);
            answer.Add("not implemented," + countUndefined);
            answer.Add("failed," + countFailed);
            answer.Add("");
            var pSuccesful = Math.Round((double)countSuccesful * 100 / countTotal, 2);
            var pUndefined = Math.Round((double)countUndefined * 100 / countTotal, 2);
            var pFailed = Math.Round((double)countFailed * 100 / countTotal, 2);
            answer.Add("%succesful," + pSuccesful);
            answer.Add("%undefined," + pUndefined);
            answer.Add("%failed," + pFailed);

            return string.Join("\r\n", answer);
        }
    }

    public class Test {
        private readonly string _path;
        private string[] _splittedPath;
        private string _name;
        public bool UseStrict;
        public bool UseNonStrict;
        private List<string> _include;
        private string _strictOutput = "";
        private string _nonStrictOutput = "";
        private YamlNode _es5Id;
        private YamlNode _description;
        private YamlNode _negative;
        private YamlNode _negativePhase;
        private YamlNode _negativeType;
        private List<string> _flags;
        public List<string> Implemented = new List<string>();
        private string _nonStrictTime;
        private string _strictTime;
        public bool NonStrictResult = false;
        public bool StrictResult = false;
        private bool _useAsync = false;
        private string _code;
        public int TotalTests = 1;

        public Test(string filePath) {
            _path = filePath.Replace('\\', '/');
        }

        public void Initialize() {
            _splittedPath = _path.Split('/');
            _name = _splittedPath[_splittedPath.Length - 1];
            UseNonStrict = true;
            var excludeWords = new[] {
                "print(", "$262", "createRealm", "detachArrayBuffer", "evalScript", "global.", "IsHTMLDDA",
                "document.all", "agent"
            };

            var readText = File.ReadAllText(_path);
            _code = readText;
            foreach (var word in excludeWords) {
                if (readText.Contains(word)) {
                    Implemented.Add(word);
                }
            }

            if (Implemented.Count >= 1) return;
            var configRGX = new Regex(@"(?<=\/\*---)[\w\W]+(?=---\*\/)");
            var match = configRGX.Match(readText);
            if (match.Value == "") return;
            var yaml = new YamlStream();
            var yamlInput = new StringReader(match.Groups[0].Value);
            yaml.Load(yamlInput);

            if (yaml.Documents.Count <= 0) return;
            var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

            mapping.Children.TryGetValue("description", out _description);
            mapping.Children.TryGetValue("es5id", out _es5Id);
            mapping.Children.TryGetValue("negative", out _negative);
            mapping.Children.TryGetValue("includes", out var tempInclude);
            if (tempInclude != null) {
                _include = tempInclude.ToString().Split(' ').ToList();
            }

            mapping.Children.TryGetValue("flags", out var tempFlag);
            if (tempFlag != null) {
                _flags = tempFlag.ToString().Split(',').ToList();
            }

            if (_negative != null) {
                var into = "";
                foreach (var node in _negative.AllNodes) {
                    switch (@into) {
                        case "phase":
                            _negativePhase = node;
                            break;

                        case "type":
                            _negativeType = node;
                            break;
                    }

                    @into = node.ToString();
                }
            }

            if (_flags != null && _flags.Count > 0) {
                foreach (var flag in _flags) {
                    switch (flag) {
                        case "[ onlyStrict ]":
                            UseStrict = true;
                            Implemented.Add("onlyStrict");
                            break;
                        case "[ noStrict ]":
                            UseNonStrict = true;
                            break;
                        case "[ module ]":
                            UseNonStrict = true;
                            break;
                        case "[ raw ]":
                            UseNonStrict = true;
                            break;
                        case "[ async ]":
                            UseNonStrict = true;
                            _useAsync = true;
                            break;
                    }
                }
            } else {
                TotalTests = 2;
                UseNonStrict = true;
                UseStrict = true;
            }
        }


        public void Execute(JSApplication application, JSService service, JSSession session) {
            if (Implemented.Count >= 1) return;
            var watch = new Stopwatch();
            watch.Start();
            var preTestOutput = "";
            if (_include != null) {
                foreach (var preTest in _include) {
                    if (!preTest.Contains(".js")) continue;
                    var preTestPath = Path.GetFullPath(Program.Test262Root + "/../harness/" + preTest.Replace(",", ""))
                        .Replace('\\', '/');
                    preTestOutput += service.RunScript(preTestPath, application, session, true, false);
                }
            }

            var strictTestOutput = "";
            var strictWatch = new Stopwatch();
            if (UseStrict) {

                strictWatch.Start();
                strictTestOutput = service.RunCode("\"use strict\"\n" + _code, application, session, false, true);
                strictWatch.Stop();

            }


            var templateWatch = new Stopwatch();
            templateWatch.Start();
            var testOutput = service.RunCode(_code, application, session, false, true);
            templateWatch.Stop();

            watch.Stop();

            if (_negative != null && _negativeType != null && _negativeType.ToString().Length > 0) {
                var splitStrictOutput = testOutput.Split(' ');
                var splitOutput = testOutput.Split(' ');
                var comparewith = "";
                if (splitOutput.Length > 1) {
                    comparewith = splitOutput[0] + splitOutput[1][0].ToString().ToUpper() +
                                  splitOutput[1].Substring(1);
                }

                var compareStrictWith = "";
                if (splitStrictOutput.Length > 1) {
                    compareStrictWith = splitStrictOutput[0] + splitStrictOutput[1][0].ToString().ToUpper() +
                                        splitStrictOutput[1].Substring(1);
                }

                if (comparewith.Contains(_negativeType.ToString())) {
                    NonStrictResult = true;
                }

                if (compareStrictWith.Contains(_negativeType.ToString())) {
                    StrictResult = true;
                }
            } else {
                if (testOutput.Length < 1) {
                    NonStrictResult = true;
                }

                if (strictTestOutput.Length < 1) {
                    StrictResult = true;
                }
            }

            if (_useAsync && templateWatch.ElapsedMilliseconds > 75) {
                NonStrictResult = false;
                _nonStrictOutput += "Async timeout fail | ";
            }


            if (preTestOutput.Length > 0) {
                _nonStrictOutput += "preTest: " + preTestOutput + " | ";
                _strictOutput += "preTest: " + strictTestOutput + " | ";
            }


            _nonStrictOutput += testOutput;
            _strictOutput += strictTestOutput;

            var stringTime = Math.Round(templateWatch.Elapsed.TotalMilliseconds, 2).ToString().Split('.');
            var strictStringTime = Math.Round(strictWatch.Elapsed.TotalMilliseconds, 2).ToString().Split('.');
            var timeDecimal = "00";
            if (stringTime.Length == 2) {
                timeDecimal = (int.Parse(stringTime[1]) * 6 / 10).ToString();
            }

            var timeStrictDecimal = "00";
            if (strictStringTime.Length == 2) {
                timeStrictDecimal = (int.Parse(strictStringTime[1]) * 6 / 10).ToString();
            }

            _nonStrictTime = stringTime[0] + "." + timeDecimal;
            _strictTime = strictStringTime[0] + "." + timeStrictDecimal;
        }


        public string GetCsv(int level) {
            var answer = "";
            if (!UseNonStrict) return answer;
            var fullpath = Program.Test262Root;
            var splitpath = fullpath.Length + 1;
            var dirPathSplitted = fullpath.Split('\\');
            level = level + dirPathSplitted.Length;
            var infoString = "";

            var dirPath = string.Join(",", _splittedPath, 0, _splittedPath.Length - 1);
            infoString += dirPath.Substring(splitpath);

            if (level > _splittedPath.Length - 1) {
                infoString += new string(',', (level - _splittedPath.Length) + 1);
            } else {
                infoString += ",";
            }


            infoString += _name;
            if (Implemented.Count < 1) {
                var rgx4 = new Regex(@"([\r\n])+");
                if (UseNonStrict) {
                    var preOutput = "";
                    if (_negative != null && _negativeType != null) {
                        preOutput = "[negative:" + _negativeType + "] ";
                    }

                    answer += infoString + ",nonStrict modus," + NonStrictResult + "," + _nonStrictTime + "," + "\"" +
                              rgx4.Replace(preOutput.Replace('"', '\'') + _nonStrictOutput.Replace('"', '\''), " ") +
                              "\"," +
                              _path.Replace('/', '\\');
                    if (UseStrict) {
                        answer += "\n";
                    }
                }

                if (UseStrict) {
                    var preOutput = "";
                    if (_negative != null && _negativeType != null) {
                        preOutput = "[negative:" + _negativeType + "] ";

                    }

                    answer += infoString + ",strict modus," + StrictResult + "," + _strictTime + ",\"" +
                              rgx4.Replace(preOutput.Replace('"', '\'') + _strictOutput.Replace('"', '\''), " ") + "\"," + _path.Replace('/', '\\');
                }

            } else {
                answer += "," + "undefined" + ",," +
                          "\"the following features are not implemented: " + string.Join(",", Implemented.ToArray()) +
                          "\"," + _path.Replace('/', '\\');
            }

            return answer;
        }
    }
}