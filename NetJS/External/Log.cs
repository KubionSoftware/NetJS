using NetJS.Javascript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetJS.External {
    public class Log {

        public static Constant write(Constant _this, Constant[] arguments, Scope scope) {
            var message = Tool.ToString(arguments[0], scope);

            // TODO: save base directory
            var file = scope.Application.Settings.Root + scope.Application.Settings.Log;

            try {
                System.IO.File.AppendAllText(file, message + "\n");
            } catch { }

            return Static.Undefined;
        }
    }
}