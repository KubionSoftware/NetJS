using System;
using NetJS.Core.Javascript;

namespace NetJS.External
{
    /// <summary>IO class contains methods for file manipulation.</summary>
    /// <remarks>IO can execute read, write and delete actions on a file.</remarks>
    /// <example>IO can read, write and delete a file:
    /// <code lang="javascript">var file = "example.txt";
    /// IO.write(file, "Hello World!");
    /// console.log(IO.read(file); //prints: Hello World!
    /// IO.delete(file);</code></example>
    public class IO {


        /// <summary>Writes content into a file.</summary>
        /// <param name = "file">A filename</param>
        /// <param name = "content">The content to be written</param>
        /// <example><code lang="javascript">IO.write("data.json", { name: "Hello World!");</code></example>
        /// <exception cref = "InternalError">Thrown when no application can be found in the application scope.</exception>
            public static Constant write(Constant _this, Constant[] arguments, Scope scope) {
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "IO.write");
            var content = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 1, "IO.write");

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");
            // TODO: handle errors
            System.IO.File.WriteAllText(application.Settings.Root + name.Value, content.Value);

            return Static.Undefined;
        }

        /// <summary>Reads and returns content of a file.</summary>
        /// <param name= "file">A filename to read from </param>
        /// <returns>The content of the file.</returns>
        /// <example><code lang="javascript">var content = IO.read("data.json");</code></example>
        /// <exception cref = "InternalError">Thrown when no application can be found in the application scope.</exception>
        public static Constant read(Constant _this, Constant[] arguments, Scope scope) {
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "IO.read");

            // TODO: determine return when error
            try {
                var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
                if (application == null) throw new InternalError("No application");
                return new Core.Javascript.String(System.IO.File.ReadAllText(application.Settings.Root + name.Value));
            }catch(Exception) {
                return Static.Undefined;
            }
        }
        
        /// <summary>Deletes a file.</summary>
        /// <param name= "file">A filename to delete</param>
        /// <example><code lang="javascript">IO.delete("data.json");</code></example>
        /// <exception cref = "InternalError">Thrown when no application can be found in the application scope.</exception>
        public static Constant delete(Constant _this, Constant[] arguments, Scope scope) {
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "IO.delete");

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");

            // TODO: handle errors
            System.IO.File.Delete(application.Settings.Root + name.Value);
            return Static.Undefined;
        }
    }
}