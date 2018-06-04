using NetJS.Core.Javascript;
using System;

namespace NetJS.API {
    /// <summary>Log class contains methods for logging to a file.</summary>
    public class Log {

        /// <summary>Writes a log to the system configured log.</summary>
        /// <param name="log">The log that needs to be written</param>
        /// <example><code lang="javascript">Log.write("Hello world!");</code></example>
        /// <exception cref="InternalError">Thrown when there is no application in the application lex.</exception>
        public static Constant write(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var message = Core.Convert.ToString(arguments[0], lex);

            var application = Tool.GetFromScope<JSApplication>(lex, "__application__");
            if (application == null) throw new InternalError("No application");
            
            var file = application.Settings.Root + application.Settings.Log;

            try {
                var info = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss:fff");
                System.IO.File.AppendAllText(file, info + " - " + message + "\n");
            } catch { }

            return Static.Undefined;
        }
    }
}