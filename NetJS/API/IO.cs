using System;
using NetJS.Core.Javascript;

namespace NetJS.API
{
    /// <summary>IO class contains methods for file and directory manipulation.</summary>
    /// <example>IO can read, write and delete a file:
    /// <code lang="javascript">var file = "example.txt";
    /// IO.write(file, "Hello World!");
    /// console.log(IO.read(file); //prints: Hello World!
    /// IO.deleteFile(file);</code></example>
    public class IO {

        private static string GetFile(JSApplication application, Core.Javascript.String name) {
            return application.Settings.Root + name.Value;
        }

        /// <summary>Writes content into a file.</summary>
        /// <param name = "file">A filename</param>
        /// <param name = "content">The content to be written</param>
        /// <example><code lang="javascript">IO.write("data.json", { name: "Hello World!");</code></example>
        /// <exception cref = "InternalError">Thrown when no application can be found in the application scope.</exception>
         public static Constant write(Constant _this, Constant[] arguments, Scope scope) {
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "IO.write");
            var content = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 1, "IO.write");

            var application = Tool.GetApplication(scope);
            var file = GetFile(application, name);

            try {
                System.IO.File.WriteAllText(file, content.Value);
            } catch {
                throw new IOError($"Could not write text to file '{file}'");
            }

            return Static.Undefined;
        }

        /// <summary>Reads and returns content of a file.</summary>
        /// <param name= "file">A filename to read from </param>
        /// <returns>The content of the file.</returns>
        /// <example><code lang="javascript">var content = IO.read("data.json");</code></example>
        /// <exception cref = "InternalError">Thrown when no application can be found in the application scope.</exception>
        public static Constant read(Constant _this, Constant[] arguments, Scope scope) {
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "IO.read");

            var application = Tool.GetApplication(scope);
            var file = GetFile(application, name);
            
            try {
                return new Core.Javascript.String(System.IO.File.ReadAllText(application.Settings.Root + name.Value));
            }catch {
                throw new IOError($"Could not read text from file '{file}'");
            }
        }
        
        /// <summary>Deletes a file.</summary>
        /// <param name= "file">A filename to delete</param>
        /// <example><code lang="javascript">IO.deleteFile("data.json");</code></example>
        /// <exception cref = "InternalError">Thrown when no application can be found in the application scope.</exception>
        public static Constant deleteFile(Constant _this, Constant[] arguments, Scope scope) {
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "IO.delete");

            var application = Tool.GetApplication(scope);
            var file = GetFile(application, name);

            try {
                System.IO.File.Delete(file);
                return Static.Undefined;
            } catch {
                throw new IOError($"Could not delete file '{file}'");
            }
        }

        /// <summary>Copies a file.</summary>
        /// <param name= "source">The file to copy</param>
        /// <param name= "destination">The file to copy to</param>
        /// <example><code lang="javascript">IO.copyFile("a.txt", "b.txt");</code></example>
        /// <exception cref = "InternalError">Thrown when no application can be found in the application scope.</exception>
        public static Constant copyFile(Constant _this, Constant[] arguments, Scope scope) {
            var a = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "IO.copy");
            var b = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 1, "IO.copy");

            var application = Tool.GetApplication(scope);
            var fileA = GetFile(application, a);
            var fileB = GetFile(application, b);

            try {
                System.IO.File.Copy(fileA, fileB);
                return Static.Undefined;
            } catch {
                throw new IOError($"Could not copy '{fileA}' to '{fileB}'");
            }
        }

        /// <summary>Moves/renames a file.</summary>
        /// <param name= "source">The source location</param>
        /// <param name= "destination">The destination</param>
        /// <example><code lang="javascript">IO.moveFile("a.txt", "files/b.txt");</code></example>
        /// <exception cref = "InternalError">Thrown when no application can be found in the application scope.</exception>
        public static Constant moveFile(Constant _this, Constant[] arguments, Scope scope) {
            var a = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "IO.moveFile");
            var b = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 1, "IO.moveFile");

            var application = Tool.GetApplication(scope);
            var fileA = GetFile(application, a);
            var fileB = GetFile(application, b);

            try {
                System.IO.File.Move(fileA, fileB);
                return Static.Undefined;
            } catch {
                throw new IOError($"Could not move '{fileA}' to '{fileB}'");
            }
        }

        /// <summary>Moves/renames a directory.</summary>
        /// <param name= "source">The source location</param>
        /// <param name= "destination">The destination</param>
        /// <example><code lang="javascript">IO.moveDirectory("files", "documents/files");</code></example>
        /// <exception cref = "InternalError">Thrown when no application can be found in the application scope.</exception>
        public static Constant moveDirectory(Constant _this, Constant[] arguments, Scope scope) {
            var a = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "IO.moveDirectory");
            var b = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 1, "IO.moveDirectory");

            var application = Tool.GetApplication(scope);
            var dirA = GetFile(application, a);
            var dirB = GetFile(application, b);

            try {
                System.IO.Directory.Move(dirA, dirB);
                return Static.Undefined;
            } catch {
                throw new IOError($"Could not move directory '{dirA}' to '{dirB}'");
            }
        }

        /// <summary>Get all files in a directory.</summary>
        /// <param name= "directory">The directory path</param>
        /// <example><code lang="javascript">var files = IO.getFiles("documents");</code></example>
        /// <exception cref = "InternalError">Thrown when no application can be found in the application scope.</exception>
        public static Constant getFiles(Constant _this, Constant[] arguments, Scope scope) {
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "IO.getFiles");

            var application = Tool.GetApplication(scope);
            var dir = GetFile(application, name);

            try {
                return Core.Tool.ToArray(System.IO.Directory.GetFiles(dir), scope);
            } catch {
                throw new IOError($"Could not get files from directory '{dir}'");
            }
        }

        /// <summary>Get all directories in a directory.</summary>
        /// <param name= "directory">The directory path</param>
        /// <example><code lang="javascript">var directories = IO.getDirectories("documents");</code></example>
        /// <exception cref = "InternalError">Thrown when no application can be found in the application scope.</exception>
        public static Constant getDirectories(Constant _this, Constant[] arguments, Scope scope) {
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "IO.getDirectories");

            var application = Tool.GetApplication(scope);
            var dir = GetFile(application, name);

            try {
                return Core.Tool.ToArray(System.IO.Directory.GetDirectories(dir), scope);
            } catch {
                throw new IOError($"Could not get directories from directory '{dir}'");
            }
        }

        /// <summary>Checks if the file exists.</summary>
        /// <param name= "file">The file path</param>
        /// <example><code lang="javascript">var exists = IO.fileExists("name.txt");</code></example>
        /// <exception cref = "InternalError">Thrown when no application can be found in the application scope.</exception>
        public static Constant fileExists(Constant _this, Constant[] arguments, Scope scope) {
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "IO.getFiles");

            var application = Tool.GetApplication(scope);
            var file = GetFile(application, name);

            try {
                return new Core.Javascript.Boolean(System.IO.File.Exists(file));
            } catch {
                throw new IOError($"Could not check if file '{file}' exists");
            }
        }

        /// <summary>Checks if the directory exists.</summary>
        /// <param name= "directory">The directory path</param>
        /// <example><code lang="javascript">var exists = IO.directoryExists("documents");</code></example>
        /// <exception cref = "InternalError">Thrown when no application can be found in the application scope.</exception>
        public static Constant directoryExists(Constant _this, Constant[] arguments, Scope scope) {
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "IO.getFiles");

            var application = Tool.GetApplication(scope);
            var dir = GetFile(application, name);

            try {
                return new Core.Javascript.Boolean(System.IO.Directory.Exists(dir));
            } catch {
                throw new IOError($"Could not check if directory '{dir}' exists");
            }
        }

        /// <summary>Creates a new directory.</summary>
        /// <param name= "directory">The directory path</param>
        /// <example><code lang="javascript">IO.createDirectory("documents");</code></example>
        /// <exception cref = "InternalError">Thrown when no application can be found in the application scope.</exception>
        public static Constant createDirectory(Constant _this, Constant[] arguments, Scope scope) {
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "IO.getFiles");

            var application = Tool.GetApplication(scope);
            var dir = GetFile(application, name);

            try {
                System.IO.Directory.CreateDirectory(dir);
                return Static.Undefined;
            } catch {
                throw new IOError($"Could not create directory '{dir}'");
            }
        }

        /// <summary>Deletes a directory.</summary>
        /// <param name= "directory">The directory path</param>
        /// <example><code lang="javascript">IO.deleteDirectory("documents");</code></example>
        /// <exception cref = "InternalError">Thrown when no application can be found in the application scope.</exception>
        public static Constant deleteDirectory(Constant _this, Constant[] arguments, Scope scope) {
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "IO.getFiles");

            var application = Tool.GetApplication(scope);
            var dir = GetFile(application, name);

            try {
                System.IO.Directory.Delete(dir);
                return Static.Undefined;
            } catch {
                throw new IOError($"Could not delete directory '{dir}'");
            }
        }
    }
}