using Microsoft.ClearScript;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NetJS.API {
    /// <summary>Functions class contain functions that are injected directly into the engine.</summary>
    public class Functions {

        private static object includeLoad(string template, dynamic arguments, bool returnVar) {
            var parts = template.Split('.');
            if (parts.Length == 1) {
                template += ".js";
            }

            var application = State.Application;
            var resource = application.Cache.GetResource(template, returnVar, application);

            var oldBuffer = State.Buffer;
            var buffer = returnVar ? new StringBuilder() : oldBuffer;
            State.Buffer = buffer;
            resource(arguments);
            State.Buffer = oldBuffer;

            if (returnVar) {
                return buffer.ToString();
            } else {
                return null;
            }
        }
        
        /// <summary>include  takes a file, runs the code in the file and writes the result to the output buffer.
        /// If an object is given as second parameter, those variables will be set in the code before execution.
        /// Default filetype is ".js".</summary>
        /// <param name="file">The file to include</param>
        /// <param name="variables">An object with variables to setup the file before execution</param>
        /// <example><code lang="javascript">include("renderLayout.js", {loggedIn: true});</code></example>
        public static void include(string template, dynamic arguments = null) {
            includeLoad(template, arguments, false);
        }

        /// <summary>load takes a file, runs the code in the file and returns the value.
        /// If an object is given as second parameter, those variables will be set in the code before execution.
        /// Default filetype is ".js".</summary>
        /// <param name="file">The file to load</param>
        /// <param name="variables">An object with variables to setup the file before execution</param>
        /// <returns>Returns the output of the template.</returns>
        /// <example><code lang="javascript">var output = load("renderLayout.js", {loggedIn: true});</code></example>
        public static dynamic load(string template, dynamic arguments = null) {
            return includeLoad(template, arguments, true);
        }

        /// <summary>out writes a string to the output buffer</summary>
        /// <param name="value">The string to write</param>
        /// <example><code lang="javascript">out(JSON.stringify(data));</code></example>
        public static void @out(string value) {
            State.Buffer.Append(value);
        }

        /// <summary>outLine writes a string to the output buffer and appends a newline</summary>
        /// <param name="value">The string to write</param>
        /// <example><code lang="javascript">outLine(JSON.stringify(data));</code></example>
        public static void @outLine(string value) {
            State.Buffer.Append(value + "\n");
        }

        /// <summary>end returns the value to the caller</summary>
        /// <param name="value">The output to return</param>
        /// <example><code lang="javascript">end("<html></html>");</code></example>
        public static void end(object value = null) {
            State.Request.ResultCallback(value);
        }

        /// <summary>Schedules a function to be called after a certain amount of time</summary>
        /// <param name="function">The function to call</param>
        /// <param name="time">The time in milliseconds</param>
        /// <example><code lang="javascript">setTimeout(() => {
        ///     Log.write("It's time!");
        /// }, 1500);</code></example>
        public static void setTimeout(dynamic function, int time) {
            State.Application.AddTimeOut(time, function, State.Get());
        }

        /// <summary>import takes a file and runs the code in the file with the current agent.
        /// This way functions and variables can be imported.
        /// Default filetype is ".js".</summary>
        /// <param name="file">The file to import</param>
        /// <example><code lang="javascript">import("date");
        /// FormatDate(new Date());</code></example>
        public static void require(string template) {
            var application = State.Application;
            application.Require(template);
        }
        
        /// <summary>Includes a namespace from the .NET environment</summary>
        /// <param name="namespace">The namespace to include</param>
        /// <example><code lang="javascript">includeNamespace("System.Text");</code></example>
        public static void includeNamespace(string ns) {
            State.Application.AddHostObject("NetJS", new HostTypeCollection(ns));
            var root = ns.Split('.')[0];
            State.Application.Evaluate($"var {root} = NetJS.{root};");
        }

        /// <summary>Includes a dll</summary>
        /// <param name="file">The dll file to include</param>
        /// <example><code lang="javascript">includeDLL("ADFS.dll");</code></example>
        public static void includeDLL(string file) {
            
        }
    }
}