using System;

namespace NetJS.API {
    /// <summary>Log class contains methods for logging to a file.</summary>
    public class Log {

        /// <summary>Writes a log to the system configured log.</summary>
        /// <param name="log">The log that needs to be written</param>
        /// <example><code lang="javascript">Log.write("Hello world!");</code></example>
        public static void write(string message) {
            var file = State.Application.Settings.Root + State.Application.Settings.Log;

            try {
                var info = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss:fff");
                System.IO.File.AppendAllText(file, info + " - " + message + "\n");
            } catch { }
        }
    }
}