using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Console {
    class Program {
        static void Main(string[] args) {
            var application = new JSApplication();
            var service = new JSService();

            System.Console.WriteLine("Type 'quit' to exit the application");

            while (true) {
                try {
                    var result = service.RunTemplate("main.js");
                    System.Console.WriteLine(result);
                } catch (Exception e) {
                    System.Console.WriteLine(e);
                }

                if (System.Console.ReadLine().ToLower() == "quit") Environment.Exit(0);
            }
        }
    }
}
