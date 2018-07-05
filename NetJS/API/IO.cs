using System;
using Microsoft.ClearScript.JavaScript;

namespace NetJS.API
{
    /// <summary>IO class contains methods for file and directory manipulation.</summary>
    /// <example>IO can read, write and delete a file:
    /// <code lang="javascript">var file = "example.txt";
    /// IO.write(file, "Hello World!");
    /// console.log(IO.read(file); //prints: Hello World!
    /// IO.deleteFile(file);</code></example>
    public class IO {

        private static string GetFile(string name) {
            return State.Application.Settings.Root + name;
        }

        /// <summary>Writes text into a file.</summary>
        /// <param name = "file">A filename (string)</param>
        /// <param name = "content">The text to be written (string)</param>
        /// <example><code lang="javascript">IO.writeText("data.json", "Hello World!");</code></example>
        public static dynamic writeText(string name, string content) {
            var application = State.Application;
            var state = State.Get();
            var file = GetFile(name);

            return Tool.CreatePromise((resolve, reject) => {
                try {
                    System.IO.File.WriteAllText(file, content);
                    application.AddCallback(resolve, true, state);
                } catch {
                    application.AddCallback(reject, $"Could not write text to file '{file}'", state);
                }
            });
        }

        /// <summary>Writes bytes into a file.</summary>
        /// <param name = "file">A filename</param>
        /// <param name = "content">The bytes to be written (Uint8Array)</param>
        /// <example><code lang="javascript">IO.writeBytes("image.png", bytes);</code></example>
        public static dynamic writeBytes(string name, dynamic bytes) {
            var application = State.Application;
            var state = State.Get();
            var file = GetFile(name);

            return Tool.CreatePromise((resolve, reject) => {
                var content = (IArrayBuffer)bytes.buffer;

                try {
                    System.IO.File.WriteAllBytes(file, content.GetBytes());
                    application.AddCallback(resolve, true, state);
                } catch {
                    application.AddCallback(reject, $"Could not write bytes to file '{file}'", state);
                }
            });
        }

        /// <summary>Reads and returns text content of a file.</summary>
        /// <param name= "file">A filename to read from (string)</param>
        /// <returns>The content of the file (string)</returns>
        /// <example><code lang="javascript">var text = IO.readText("data.json");</code></example>
        public static dynamic readText(string name) {
            var application = State.Application;
            var state = State.Get();
            var file = GetFile(name);

            return Tool.CreatePromise((resolve, reject) => {
                try {
                    application.AddCallback(resolve, System.IO.File.ReadAllText(file), state);
                } catch {
                    application.AddCallback(reject, $"Could not read text from file '{file}'", state);
                }
            });
        }

        /// <summary>Reads and returns binary content of a file.</summary>
        /// <param name= "file">A filename to read from (string)</param>
        /// <returns>The binary content of the file (Uint8Array)</returns>
        /// <example><code lang="javascript">var bytes = IO.readBytes("image.png");</code></example>
        public static dynamic readBytes(string name) {
            var application = State.Application;
            var state = State.Get();
            var file = GetFile(name);

            return Tool.CreatePromise((resolve, reject) => {
                try {
                    var bytes = System.IO.File.ReadAllBytes(file);
                    application.AddCallback(resolve, Tool.ToByteArray(bytes), state);
                } catch (Exception e) {
                    application.AddCallback(reject, $"Could not read bytes from file '{file}'", state);
                }
            });
        }

        /// <summary>Deletes a file.</summary>
        /// <param name= "file">A filename to delete</param>
        /// <example><code lang="javascript">IO.deleteFile("data.json");</code></example>
        public static dynamic deleteFile(string name) {
            var application = State.Application;
            var state = State.Get();
            var file = GetFile(name);

            return Tool.CreatePromise((resolve, reject) => {
                try {
                    System.IO.File.Delete(file);
                    application.AddCallback(resolve, true, state);
                } catch {
                    application.AddCallback(reject, $"Could not delete file '{file}'", state);
                }
            });
        }

        /// <summary>Copies a file.</summary>
        /// <param name= "source">The file to copy</param>
        /// <param name= "destination">The file to copy to</param>
        /// <example><code lang="javascript">IO.copyFile("a.txt", "b.txt");</code></example>
        public static dynamic copyFile(string source, string destination) {
            var application = State.Application;
            var state = State.Get();
            var fileA = GetFile(source);
            var fileB = GetFile(destination);

            return Tool.CreatePromise((resolve, reject) => {
                try {
                    System.IO.File.Copy(fileA, fileB);
                    application.AddCallback(resolve, true, state);
                } catch {
                    application.AddCallback(reject, $"Could not copy '{fileA}' to '{fileB}'", state);
                }
            });
        }

        /// <summary>Moves/renames a file.</summary>
        /// <param name= "source">The source location</param>
        /// <param name= "destination">The destination</param>
        /// <example><code lang="javascript">IO.moveFile("a.txt", "files/b.txt");</code></example>
        public static dynamic moveFile(string source, string destination) {
            var application = State.Application;
            var state = State.Get();
            var fileA = GetFile(source);
            var fileB = GetFile(destination);

            return Tool.CreatePromise((resolve, reject) => {
                try {
                    System.IO.File.Move(fileA, fileB);
                    application.AddCallback(resolve, true, state);
                } catch {
                    application.AddCallback(reject, $"Could not move '{fileA}' to '{fileB}'", state);
                }
            });
        }

        /// <summary>Moves/renames a directory.</summary>
        /// <param name= "source">The source location</param>
        /// <param name= "destination">The destination</param>
        /// <example><code lang="javascript">IO.moveDirectory("files", "documents/files");</code></example>
        public static dynamic moveDirectory(string source, string destination) {
            var application = State.Application;
            var state = State.Get();
            var dirA = GetFile(source);
            var dirB = GetFile(destination);

            return Tool.CreatePromise((resolve, reject) => {
                try {
                    System.IO.Directory.Move(dirA, dirB);
                    application.AddCallback(resolve, true, state);
                } catch {
                    application.AddCallback(reject, $"Could not move directory '{dirA}' to '{dirB}'", state);
                }
            });
        }

        /// <summary>Get all files in a directory.</summary>
        /// <param name= "directory">The directory path</param>
        /// <example><code lang="javascript">var files = IO.getFiles("documents");</code></example>
        public static dynamic getFiles(string directory) {
            var application = State.Application;
            var state = State.Get();
            var dir = GetFile(directory);

            return Tool.CreatePromise((resolve, reject) => {
                try {
                    application.AddCallback(resolve, Tool.ToArray(System.IO.Directory.GetFiles(dir)), state);
                } catch {
                    application.AddCallback(reject, $"Could not get files from directory '{dir}'", state);
                }
            });
        }

        /// <summary>Get all directories in a directory.</summary>
        /// <param name= "directory">The directory path</param>
        /// <example><code lang="javascript">var directories = IO.getDirectories("documents");</code></example>
        public static dynamic getDirectories(string directory) {
            var application = State.Application;
            var state = State.Get();
            var dir = GetFile(directory);

            return Tool.CreatePromise((resolve, reject) => {
                try {
                    application.AddCallback(resolve, Tool.ToArray(System.IO.Directory.GetDirectories(dir)), state);
                } catch {
                    application.AddCallback(reject, $"Could not get directories from directory '{dir}'", state);
                }
            });
        }

        /// <summary>Checks if the file exists.</summary>
        /// <param name= "file">The file path</param>
        /// <example><code lang="javascript">var exists = IO.fileExists("name.txt");</code></example>
        public static dynamic fileExists(string name) {
            var application = State.Application;
            var state = State.Get();
            var file = GetFile(name);

            return Tool.CreatePromise((resolve, reject) => {
                try {
                    application.AddCallback(resolve, System.IO.File.Exists(file), state);
                } catch {
                    application.AddCallback(reject, $"Could not check if file '{file}' exists", state);
                }
            });
        }

        /// <summary>Checks if the directory exists.</summary>
        /// <param name= "directory">The directory path</param>
        /// <example><code lang="javascript">var exists = IO.directoryExists("documents");</code></example>
        public static dynamic directoryExists(string name) {
            var application = State.Application;
            var state = State.Get();
            var dir = GetFile(name);

            return Tool.CreatePromise((resolve, reject) => {
                try {
                    application.AddCallback(resolve, System.IO.Directory.Exists(dir), state);
                } catch {
                    application.AddCallback(reject, $"Could not check if directory '{dir}' exists", state);
                }
            });
        }

        /// <summary>Creates a new directory.</summary>
        /// <param name= "directory">The directory path</param>
        /// <example><code lang="javascript">IO.createDirectory("documents");</code></example>
        public static dynamic createDirectory(string name) {
            var application = State.Application;
            var state = State.Get();
            var dir = GetFile(name);

            return Tool.CreatePromise((resolve, reject) => {
                try {
                    System.IO.Directory.CreateDirectory(dir);
                    application.AddCallback(resolve, true, state);
                } catch {
                    application.AddCallback(reject, $"Could not create directory '{dir}'", state);
                }
            });
        }

        /// <summary>Deletes a directory.</summary>
        /// <param name= "directory">The directory path</param>
        /// <example><code lang="javascript">IO.deleteDirectory("documents");</code></example>
        public static dynamic deleteDirectory(string name) {
            var application = State.Application;
            var state = State.Get();
            var dir = GetFile(name);

            return Tool.CreatePromise((resolve, reject) => {
                try {
                    System.IO.Directory.Delete(dir);
                    application.AddCallback(resolve, true, state);
                } catch {
                    application.AddCallback(reject, $"Could not delete directory '{dir}'", state);
                }
            });
        }
    }
}