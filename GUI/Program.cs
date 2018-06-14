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

            var application = new JSApplication();
            var service = new JSService();
            var session = new JSSession();
            
            application.Realm.RegisterType(typeof(API.Window), "Window");
            application.Realm.RegisterType(typeof(API.Graphics), "Graphics");

            //service.RunTemplate(application.Settings.Startup, "{}", ref application, ref session);
            try {
                var result = service.RunTemplate(application.Settings.Entry, "{}", ref application, ref session);

                if (result.Length > 0) {
                    Core.Log.Write(result);
                }
            } catch(Exception e) {
                Core.Log.Write(e.ToString());
            }
        }
    }
}
