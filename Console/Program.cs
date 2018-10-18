using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Console {
    class Program {
        static void Main(string[] args) {
            var application = new JSApplication("", app => {
                app.AddHostType(typeof(API.Console));
            }, (error, stage) => {
                System.Console.WriteLine(error);
            });
            var service = new JSService();
            var session = new JSSession();

            try {
                service.RunScriptSync(application.Settings.Startup, application, session, result => {
                    System.Console.WriteLine(result);
                    System.Console.ReadLine();
                });
            } catch (Exception e) {
                System.Console.WriteLine(e);
                System.Console.ReadLine();
            }
        }
    }
}
