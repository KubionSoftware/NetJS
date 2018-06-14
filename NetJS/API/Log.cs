using NetJS.Core;
using System;

namespace NetJS.API {
    /// <summary>Log class contains methods for logging to a file.</summary>
    public class Log {

        /// <summary>Writes a log to the system configured log.</summary>
        /// <param name="log">The log that needs to be written</param>
        /// <example><code lang="javascript">Log.write("Hello world!");</code></example>
        /// <exception cref="InternalError">Thrown when there is no application in the application lex.</exception>
        public static Constant write(Constant _this, Constant[] arguments, Agent agent) {
            var message = Core.Convert.ToString(arguments[0], agent);

            var application = (agent as NetJSAgent).Application;
            var file = application.Settings.Root + application.Settings.Log;

            try {
                var info = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss:fff");
                System.IO.File.AppendAllText(file, info + " - " + message + "\n");
            } catch { }

            return Static.Undefined;
        }
    }
}