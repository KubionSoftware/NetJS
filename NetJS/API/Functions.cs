using System.Text;
using System.Web;
using NetJS.Core;

namespace NetJS.API {
    /// <summary>Functions class contain functions that are injected directly into the engine.</summary>
    public class Functions {

        private static Constant includeLoad(Constant[] arguments, bool returnVar, Agent agent) {
            var name = Core.Tool.GetArgument<Core.String>(arguments, 0, "include");

            var parts = name.Value.Split('.');
            if (parts.Length == 1) {
                name.Value += ".js";
            }

            var application = (agent as NetJSAgent).Application;
            if (application == null) throw new InternalError("No application");
            var script = application.Cache.GetScript(name.Value, application);

            // Execute template
            var buffer = returnVar ? new StringBuilder() : agent.Running.Buffer;
            Object parameters = arguments.Length > 1 ? (Object)arguments[1] : null;
            var result = script.Evaluate(agent, true, buffer, parameters).Value;

            if (returnVar) {
                return result is Undefined ? new String(buffer.ToString()) : result;
            } else {
                buffer.Append(Convert.ToString(result, agent));
                return Static.Undefined;
            }
        }
        
        /// <summary>include  takes a file, runs the code in the file and writes the result to the output buffer.
        /// If an object is given as second parameter, those variables will be set in the code before execution.
        /// Default filetype is ".js".</summary>
        /// <param name="file">The file to include</param>
        /// <param name="variables">An object with variables to setup the file before execution</param>
        /// <example><code lang="javascript">include("renderLayout.js", {loggedIn: true});</code></example>
        public static Constant include(Constant _this, Constant[] arguments, Agent agent) {
            return includeLoad(arguments, false, agent);
        }

        /// <summary>load takes a file, runs the code in the file and returns the value.
        /// If an object is given as second parameter, those variables will be set in the code before execution.
        /// Default filetype is ".js".</summary>
        /// <param name="file">The file to load</param>
        /// <param name="variables">An object with variables to setup the file before execution</param>
        /// <returns>Returns the output of the template.</returns>
        /// <example><code lang="javascript">var output = load("renderLayout.js", {loggedIn: true});</code></example>
        public static Constant load(Constant _this, Constant[] arguments, Agent agent) {
            return includeLoad(arguments, true, agent);
        }

        /// <summary>out writes a string to the output buffer</summary>
        /// <param name="value">The string to write</param>
        /// <example><code lang="javascript">out(JSON.stringify(data));</code></example>
        public static Constant @out(Constant _this, Constant[] arguments, Agent agent) {
            var value = Core.Tool.GetArgument(arguments, 0, "out");
            agent.Running.Buffer.Append(Convert.ToString(value, agent));
            return Static.Undefined;
        }

        /// <summary>outLine writes a string to the output buffer and appends a newline</summary>
        /// <param name="value">The string to write</param>
        /// <example><code lang="javascript">outLine(JSON.stringify(data));</code></example>
        public static Constant @outLine(Constant _this, Constant[] arguments, Agent agent) {
            var value = Core.Tool.GetArgument(arguments, 0, "outLine");
            agent.Running.Buffer.Append(Convert.ToString(value, agent) + "\n");
            return Static.Undefined;
        }

        /// <summary>import takes a file and runs the code in the file with the current agent.
        /// This way functions and variables can be imported.
        /// Default filetype is ".js".</summary>
        /// <param name="file">The file to import</param>
        /// <example><code lang="javascript">import("date");
        /// FormatDate(new Date());</code></example>
        public static Constant import(Constant _this, Constant[] arguments, Agent agent) {
            var name = Core.Tool.GetArgument<Core.String>(arguments, 0, "import");

            var parts = name.Value.Split('.');
            if (parts.Length == 1) {
                name.Value += ".js";
            }

            var application = (agent as NetJSAgent).Application;
            var node = application.Cache.GetScript(name.Value, application);

            // Pop created function environment so import runs in callee environment
            var oldEnv = agent.Pop();
            var result = node.Evaluate(agent, false).Value;
            agent.Push(oldEnv);

            return result;
        }
        
        /// <summary>redirect takes an url and redirects a HttpResponse to the given url.</summary>
        /// <param name="url">A url to redirect to</param>
        /// <example><code lang="javascript">redirect("https://google.com/search?q=hello+world");</code></example>
        public static Constant redirect(Constant _this, Constant[] arguments, Agent agent) {
            var url = Core.Tool.GetArgument<Core.String>(arguments, 0, "redirect");
            var context = HttpContext.Current;
            if (context != null) {
                context.Response.Redirect(url.Value);
            }
            return Static.Undefined;
        }

        /// <summary>Runs unsafe code (loops)</summary>
        /// <param name="function">The function to execute</param>
        /// <example><code lang="javascript">unsafe(function(){
        ///     while(true){}
        /// });</code></example>
        public static Constant @unsafe(Constant _this, Constant[] arguments, Agent agent) {
            var function = Core.Tool.GetArgument<Core.Function>(arguments, 0, "unsafe");

            var netJSAgent = agent as NetJSAgent;
            netJSAgent.Unsafe = true;
            function.Call(Static.Undefined, agent, new Constant[] { });
            netJSAgent.Unsafe = false;

            return Static.Undefined;
        }
    }
}