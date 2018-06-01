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
                data += $"\"{args[i].Replace("-", "")}\": \"{args[i + 1]}\"";
            }
            data += "}";

            var application = new JSApplication();
            var service = new JSService();
            var session = new JSSession();

            application.Engine.RegisterClass(typeof(API.Console));

            service.RunTemplate(application.Settings.Startup, data, ref application, ref session);

            try {
                var result = service.RunTemplate(application.Settings.Entry, data, ref application, ref session);
                System.Console.WriteLine(result);
                System.Console.Read();
            } catch (Exception e) {
                System.Console.WriteLine(e);
                System.Console.Read();
            }
        }
    }
}
