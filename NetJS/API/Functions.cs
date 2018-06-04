using System.Text;
using System.Web;
using NetJS.Core.Javascript;
using System.Collections.Generic;
using NetJS.Core;

namespace NetJS.API {
    /// <summary>Functions class contain functions that are injected directly into the engine.</summary>
    public class Functions {

        private static Constant includeLoad(Constant[] arguments, bool returnVar, LexicalEnvironment lex) {
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "include");

            var parts = name.Value.Split('.');
            if (parts.Length == 1) {
                name.Value += ".js";
            }

            var application = Tool.GetFromScope<JSApplication>(lex, "__application__");
            if (application == null) throw new InternalError("No application");
            var node = application.Cache.GetFile(name.Value, application);

            // Create lex
            var buffer = returnVar ? new StringBuilder() : lex.Buffer;
            var templateScope = new LexicalEnvironment(application.Engine.EngineScope, lex, node, EnvironmentType.Function, buffer);

            // Pass arguments
            if (arguments.Length > 1) {
                var parameters = (Object)arguments[1];
                foreach (var key in parameters.GetKeys()) {
                    templateScope.DeclareVariable(key, Core.Javascript.DeclarationScope.Function, false, parameters.Get(key));
                }
            }

            // Execute template
            var result = node.Execute(templateScope).Value;

            if (returnVar) {
                return result is Undefined ? new String(buffer.ToString()) : result;
            } else {
                buffer.Append(Convert.ToString(result, lex));
                return Static.Undefined;
            }
        }
        
        /// <summary>include  takes a file, runs the code in the file and writes the result to the output buffer.
        /// If an object is given as second parameter, those variables will be set in the code before execution.
        /// Default filetype is ".js".</summary>
        /// <param name="file">The file to include</param>
        /// <param name="variables">An object with variables to setup the file before execution</param>
        /// <example><code lang="javascript">include("renderLayout.js", {loggedIn: true});</code></example>
        public static Constant include(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            return includeLoad(arguments, false, lex);
        }

        /// <summary>load takes a file, runs the code in the file and returns the value.
        /// If an object is given as second parameter, those variables will be set in the code before execution.
        /// Default filetype is ".js".</summary>
        /// <param name="file">The file to load</param>
        /// <param name="variables">An object with variables to setup the file before execution</param>
        /// <returns>Returns the output of the template.</returns>
        /// <example><code lang="javascript">var output = load("renderLayout.js", {loggedIn: true});</code></example>
        public static Constant load(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            return includeLoad(arguments, true, lex);
        }

        /// <summary>out writes a string to the output buffer</summary>
        /// <param name="value">The string to write</param>
        /// <example><code lang="javascript">out(JSON.stringify(data));</code></example>
        public static Constant @out(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var value = Core.Tool.GetArgument(arguments, 0, "out");
            lex.Buffer.Append(Convert.ToString(value, lex));
            return Static.Undefined;
        }

        /// <summary>outLine writes a string to the output buffer and appends a newline</summary>
        /// <param name="value">The string to write</param>
        /// <example><code lang="javascript">outLine(JSON.stringify(data));</code></example>
        public static Constant @outLine(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var value = Core.Tool.GetArgument(arguments, 0, "outLine");
            lex.Buffer.Append(Convert.ToString(value, lex) + "\n");
            return Static.Undefined;
        }

        /// <summary>import takes a file and runs the code in the file with the current lex.
        /// This way functions and variables can be imported.
        /// Default filetype is ".js".</summary>
        /// <param name="file">The file to import</param>
        /// <example><code lang="javascript">import("date");
        /// FormatDate(new Date());</code></example>
        public static Constant import(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "import");

            var parts = name.Value.Split('.');
            if (parts.Length == 1) {
                name.Value += ".js";
            }

            var application = Tool.GetFromScope<JSApplication>(lex, "__application__");
            if (application == null) throw new InternalError("No application");
            var node = application.Cache.GetFile(name.Value, application);

            if (lex.StackParent == null) throw new InternalError("No lex to import code in");
            return node.Execute(lex.StackParent).Value;
        }
        
        /// <summary>redirect takes an url and redirects a HttpResponse to the given url.</summary>
        /// <param name="url">A url to redirect to</param>
        /// <example><code lang="javascript">redirect("https://google.com/search?q=hello+world");</code></example>
        public static Constant redirect(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var url = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "redirect");
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
        public static Constant @unsafe(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var function = Core.Tool.GetArgument<Core.Javascript.Function>(arguments, 0, "unsafe");

            lex.Set("__unsafe__", new Core.Javascript.Boolean(true));
            function.Call(new Constant[] { }, Static.Undefined, lex);
            lex.Remove("__unsafe__");

            return Static.Undefined;
        }
    }
}