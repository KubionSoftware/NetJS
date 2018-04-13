using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Console {
    class Program {
        static void Main(string[] args) {
            var data = "{";
            for (var i = 1; i < args.Length - 1; i++) {
                if (i > 1) data += ",";

                if (!args[i].StartsWith("-")) {
                    System.Console.WriteLine("The parameter name should start with -");
                    System.Console.ReadLine();
                    return;
                }
                data += args[i].Replace("-", "") + ": \"" + args[i + 1] + "\"";
            }
            data += "}";

            var application = new JSApplication();
            var service = new JSService();

            System.Console.WriteLine("Type 'quit' to exit the application");

            while (true) {
                try {
                    var result = service.RunTemplate("main.js", data);
                    System.Console.WriteLine(result);
                } catch (Exception e) {
                    System.Console.WriteLine(e);
                }

                if (System.Console.ReadLine().ToLower() == "quit") Environment.Exit(0);
            }
        }
    }
}
