using NetJS.Core.Javascript;
using System;

namespace NetJS.External {
    /// <summary>Log class contains methods for logging to a file.</summary>
    public class Log {

        /// <summary>Writes a log to the system configured log.</summary>
        /// <param name="log">The log that needs to be written</param>
        /// <example><code lang="javascript">Log.write("Hello world!");</code></example>
        /// <exception cref="InternalError">Thrown when there is no application in the application scope.</exception>
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