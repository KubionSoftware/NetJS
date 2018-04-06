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

        /// <summary>Copies a file.</summary>
        /// <param name= "source">The file to copy</param>
        /// <param name= "destination">The file to copy to</param>
        /// <example><code lang="javascript">IO.copy("a.txt", "b.txt");</code></example>
        /// <exception cref = "InternalError">Thrown when no application can be found in the application scope.</exception>
        public static Constant copy(Constant _this, Constant[] arguments, Scope scope) {
            var a = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "IO.copy");
            var b = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 1, "IO.copy");

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");

            // TODO: handle errors
            System.IO.File.Copy(application.Settings.Root + a.Value, application.Settings.Root + b.Value);
            return Static.Undefined;
        }

        /// <summary>Moves/renames a file.</summary>
        /// <param name= "source">The source location</param>
        /// <param name= "destination">The destination</param>
        /// <example><code lang="javascript">IO.moveFile("a.txt", "files/b.txt");</code></example>
        /// <exception cref = "InternalError">Thrown when no application can be found in the application scope.</exception>
        public static Constant moveFile(Constant _this, Constant[] arguments, Scope scope) {
            var a = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "IO.moveFile");
            var b = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 1, "IO.moveFile");

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");

            // TODO: handle errors
            System.IO.File.Move(application.Settings.Root + a.Value, application.Settings.Root + b.Value);
            return Static.Undefined;
        }

        /// <summary>Moves/renames a directory.</summary>
        /// <param name= "source">The source location</param>
        /// <param name= "destination">The destination</param>
        /// <example><code lang="javascript">IO.moveDirectory("files", "documents/files");</code></example>
        /// <exception cref = "InternalError">Thrown when no application can be found in the application scope.</exception>
        public static Constant moveDirectory(Constant _this, Constant[] arguments, Scope scope) {
            var a = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "IO.moveDirectory");
            var b = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 1, "IO.moveDirectory");

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");

            // TODO: handle errors
            System.IO.Directory.Move(application.Settings.Root + a.Value, application.Settings.Root + b.Value);
            return Static.Undefined;
        }

        /// <summary>Get all files in a directory.</summary>
        /// <param name= "directory">The directory path</param>
        /// <example><code lang="javascript">var files = IO.getFiles("documents");</code></example>
        /// <exception cref = "InternalError">Thrown when no application can be found in the application scope.</exception>
        public static Constant getFiles(Constant _this, Constant[] arguments, Scope scope) {
            var dir = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "IO.getFiles");

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");

            // TODO: handle errors
            return Core.Tool.ToArray(System.IO.Directory.GetFiles(application.Settings.Root + dir), scope);
        }

        /// <summary>Get all directories in a directory.</summary>
        /// <param name= "directory">The directory path</param>
        /// <example><code lang="javascript">var directories = IO.getDirectories("documents");</code></example>
        /// <exception cref = "InternalError">Thrown when no application can be found in the application scope.</exception>
        public static Constant getDirectories(Constant _this, Constant[] arguments, Scope scope) {
            var dir = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "IO.getDirectories");

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");

            // TODO: handle errors
            return Core.Tool.ToArray(System.IO.Directory.GetDirectories(application.Settings.Root + dir), scope);
        }
    }
}