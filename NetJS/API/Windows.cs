using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.API {
    class Windows {

        public static string execute(string command) {
            var application = State.Application;

            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.WorkingDirectory = application.Settings.Root;
            cmd.StartInfo.Arguments = "/C " + command;
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            if (cmd.WaitForExit(1000)) {
                return cmd.StandardOutput.ReadToEnd();
            } else {
                throw new Error("Executing command took too long");
            }
        }
    }
}
