using NetJS.Core.Javascript;
using System;

namespace NetJS.External {
    public class Log {

        public static Constant write(Constant _this, Constant[] arguments, Scope scope) {
            var message = Core.Tool.ToString(arguments[0], scope);

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");

            // TODO: save base directory
            var file = application.Settings.Root + application.Settings.Log;

            try {
                var info = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss:fff");
                System.IO.File.AppendAllText(file, info + " - " + message + "\n");
            } catch { }

            return Static.Undefined;
        }
    }
}