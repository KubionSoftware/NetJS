using NetJS;
using NetJS.Javascript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDocConvert {
    class Program {

        static string InRoot;
        static string OutRoot;

        static void Main(string[] args) {
            var converter = new Converter();
            //Global.Init();

            InRoot = System.IO.Path.GetFullPath("../../xdoc_client");
            OutRoot = System.IO.Path.GetFullPath("../../out/");

            TestAll(converter, false);
            // TestSingle(converter, "../../xdoc_client/Apps.xbinjs");
        }

        static void TestAll(Converter converter, bool parse) {
            ForeachFile(InRoot, file => {
                ConvertFile(converter, file, parse);
            });
        }

        static void TestSingle(Converter converter, string file) {
            ConvertFile(converter, file, true);
            Environment.Exit(0);
        }

        static void ForeachFile(string folder, Action<string> action) {
            foreach (string file in System.IO.Directory.GetFiles(folder)) {
                action(file);
            }

            foreach (string directory in System.IO.Directory.GetDirectories(folder)) {
                ForeachFile(directory, action);
            }
        }

        static void ConvertFile(Converter converter, string file, bool parse = true) {
            var extension = System.IO.Path.GetExtension(file);
            if (extension != ".xbinjs") return;

            var path = System.IO.Path.GetFullPath(file);
            var jsPath = path.Replace(InRoot, OutRoot).Replace(".xbinjs", ".js");

            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(jsPath));

            var name = System.IO.Path.GetFileNameWithoutExtension(file);
            if (name.Contains(".bak")) return;

            var xdoc = System.IO.File.ReadAllText(file);
            var jsdoc = converter.XDocToJSDoc(xdoc);

            System.IO.File.WriteAllText(jsPath, jsdoc);

            if (parse) {
                // Try parse
                try {
                    var tokens = Lexer.Lex(jsdoc);
                    var parser = new Parser(System.IO.Path.GetFullPath(jsPath), tokens);
                    var node = parser.Parse();
                    // Console.WriteLine("Parsed " + jsFile);
                }catch(Exception e) {
                    Console.WriteLine("Failed " + jsPath);
                    Console.WriteLine(e.Message + "\n");
                }
            } else {
                Console.WriteLine(name);
            }
        }
    }
}
