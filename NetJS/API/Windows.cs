using NetJS.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.API {
    class Windows {

        public static Constant execute(Constant _this, Constant[] arguments, Agent agent) {
            var command = Core.Tool.GetArgument<Core.String>(arguments, 0, "Windows.execute");

            var application = (agent as NetJSAgent).Application;

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
                return new Core.String(cmd.StandardOutput.ReadToEnd());
            } else {
                throw new InternalError("Executing command took too long");
            }
        }
    }
}
