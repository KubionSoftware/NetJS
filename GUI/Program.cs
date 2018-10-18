using Microsoft.ClearScript;
using NetJS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetJS.GUI {
    static class Program {

        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var service = new JSService();
            var session = new JSSession();

            var application = new JSApplication(null, app => {
                app.AddHostType(typeof(API.Window));
                app.AddHostType(typeof(API.Graphics));
            }, (exception, stage) => {
                if (exception is ScriptEngineException se) {
                    NetJS.API.Log.write(se.ErrorDetails);
                } else {
                    NetJS.API.Log.write(exception.ToString());
                }
            });
            
            service.RunScript(application.Settings.Startup, application, session, result => { });
        }
    }
}
