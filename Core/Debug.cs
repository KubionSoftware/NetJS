using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using Util;

namespace NetJS.Core {
#if debug_enabled
    public class Debug {

        public struct Location {
            public int FileId;
            public int LineNr;

            public Location(int fileId, int lineNr) {
                FileId = fileId;
                LineNr = lineNr;
            }

            public bool IsSame(Location other) {
                return FileId == other.FileId && LineNr == other.LineNr;
            }
        }

        public class Frame {
            public int Index;
            public string Name;
            public string File;
            public int Line;
        }

        public class Scope {
            public string Name;
            public Json Variables;
        }

        public class Breakpoint {
            public Location Location;
            public List<int> Nodes = new List<int>();

            public Breakpoint(Location location) {
                Location = location;
            }
        }

        public const string StopOnBreakpoint = "stopOnBreakpoint";
        public const string StopOnException = "stopOnException";

        private static Dictionary<string, int> Files = new Dictionary<string, int>();

        private static ConcurrentDictionary<int, ConcurrentDictionary<int, List<int>>> Nodes = new ConcurrentDictionary<int, ConcurrentDictionary<int, List<int>>>();
        private static List<WebSocket> Sockets = new List<WebSocket>();

        private static List<Breakpoint> Breakpoints = new List<Breakpoint>();
        public static HashSet<int> BreakpointNodes = new HashSet<int>();

        private static AutoResetEvent BreakHandle = new AutoResetEvent(true);

        private static int LastId = 0;

        public static bool SteppingInto = false;
        public static bool SteppingOut = false;
        public static bool SteppingOver = false;
        public static int SteppingLevel = -1;

        public static int GetFileId(string path) {
            var normalized = Tool.NormalizePath(path);

            foreach (var key in Files.Keys) {
                if (key == normalized) {
                    return Files[key];
                }
            }

            Files[normalized] = Files.Count + 1;
            return Files[normalized];
        }

        public static string GetFileName(int id) {
            foreach (var pair in Files) {
                if (pair.Value == id) {
                    return pair.Key;
                }
            }

            return "unknown";
        }

        public static int AddNode(Location location) {
            var id = LastId++;
            if (!Nodes.ContainsKey(location.FileId)) {
                Nodes[location.FileId] = new ConcurrentDictionary<int, List<int>>();
            }
            if (!Nodes[location.FileId].ContainsKey(location.LineNr)) {
                Nodes[location.FileId][location.LineNr] = new List<int>();
            }

            Nodes[location.FileId][location.LineNr].Add(id);

            // if there is a breakpoint on this location add the node to it
            var breakpoint = GetBreakpoint(location);
            if (breakpoint != null) {
                BreakpointNodes.Add(id);
                breakpoint.Nodes.Add(id);
            }
            
            return id;
        }

        public static void RemoveNodes(int fileId) {
            if (Nodes.ContainsKey(fileId)) {
                // TODO: make type for this
                ConcurrentDictionary<int, List<int>> removedValue;
                Nodes.TryRemove(fileId, out removedValue);
            }
        }

        public static Location GetNodeLocation(int id) {
            // can be slow because it only happens on an error, which should be almost never :)

            
            foreach (var fileId in Nodes.Keys) {
                var fileNodes = Nodes[fileId];
                foreach (var lineNr in fileNodes.Keys) {
                    if (fileNodes[lineNr].Contains(id)) {
                        return new Location(fileId, lineNr);
                    }
                }
            }
           

            return new Location(-1, -1);
        }

        public static string Message(Javascript.Node node, string message) {
            // can be slow because it only happens on an error, which should be almost never :)
            var location = GetNodeLocation(node.Id);
            var file = GetFileName(location.FileId);

            return file + " (" + location.LineNr + ") - " + message;
        }

        public static bool AddBreakpoint(Location location) {
            var breakpoint = GetBreakpoint(location);

            if (breakpoint == null) {
                breakpoint = new Breakpoint(location);
                Breakpoints.Add(breakpoint);
            }

            // find the nodes that the breakpoint is set on and add them
            foreach (var fileId in Nodes.Keys) {
                foreach (var lineNr in Nodes[fileId].Keys) {
                    if (fileId == location.FileId && lineNr == location.LineNr) {
                        foreach (var id in Nodes[fileId][lineNr]) {
                            BreakpointNodes.Add(id);
                            breakpoint.Nodes.Add(id);
                        }
                    }
                }
            }

            return true;
        }

        public static void RemoveBreakpoint(Location location) {
            var breakpoint = GetBreakpoint(location);
            foreach(var node in breakpoint.Nodes) {
                BreakpointNodes.Remove(node);
            }

            Breakpoints.Remove(breakpoint);
        }

        public static void RemoveBreakpoints(int fileId) {
            foreach(var breakpoint in Breakpoints) {
                if(breakpoint.Location.FileId == fileId) {
                    RemoveBreakpoint(breakpoint.Location);
                }
            }
        }

        public static Breakpoint GetBreakpoint(Location location) {
            foreach(var breakpoint in Breakpoints) {
                if(breakpoint.Location.IsSame(location)) {
                    return breakpoint;
                }
            }
            
            return null;
        }

        public static void AddSocket(WebSocket socket) {
            Sockets.Add(socket);
        }

        public static void RemoveSocket(WebSocket socket) {
            Sockets.Remove(socket);
            Continue();
        }

        public static void Break(string eventName, List<Frame> frames, List<Scope> scopes) {
            if (Sockets.Count == 0) return;

            var json = new Json();
            json.Set("event", eventName);

            var stack = new Json();
            stack.Set("frames", frames.Select(frame => {
                var frameJson = new Json();
                frameJson.Set("index", frame.Index);
                frameJson.Set("name", frame.Name);
                frameJson.Set("file", frame.File);
                frameJson.Set("line", frame.Line);
                return frameJson;
            }));
            stack.Set("count", frames.Count);
            json.Set("stack", stack);

            json.Set("scopes", scopes.Select(scope => {
                var scopeJson = new Json();
                scopeJson.Set("name", scope.Name);
                scopeJson.Set("variables", scope.Variables);
                return scopeJson;
            }));

            SendMessage(json.ToString());

            BreakHandle.Reset();
            BreakHandle.WaitOne();
        }

        public static void HandleMessage(string message) {
            try {
                var json = new Json(message);
                var command = json.String("command");

                if (command == "start") {

                } else if (command == "setBreakpoint") {
                    var fileId = GetFileId(json.String("file"));
                    var lineNr = json.Int("line");

                    if (AddBreakpoint(new Location(fileId, lineNr))) {
                        var validatedJson = new Json();
                        validatedJson.Set("event", "breakpointValidated");

                        var breakpointJson = new Json();
                        breakpointJson.Set("id", json.Int("id"));
                        breakpointJson.Set("line", lineNr);
                        breakpointJson.Set("verified", true);

                        validatedJson.Set("breakpoint", breakpointJson);

                        SendMessage(validatedJson.ToString());
                    }
                } else if (command == "clearBreakpoint") {
                    var fileId = GetFileId(json.String("file"));
                    var lineNr = json.Int("line");
                    RemoveBreakpoint(new Location(fileId, lineNr));
                } else if (command == "clearBreakpoints") {
                    var fileId = GetFileId(json.String("file"));
                    RemoveBreakpoints(fileId);
                } else if (command == "continue") {
                    Continue();
                } else if (command == "stepInto") {
                    StepInto();
                } else if (command == "stepOver") {
                    StepOver();
                } else if (command == "stepOut") {
                    StepOut();
                }
            } catch (Exception e) {
                Log.Write("Error while handling debug message - " + e);
            }
        }

        public static void Continue() {
            SteppingLevel = -1;
            SteppingInto = SteppingOver = SteppingOut = false;
            BreakHandle.Set();
        }

        public static void StepInto() {
            SteppingInto = true;
            BreakHandle.Set();
        }

        public static void StepOver() {
            SteppingOver = true;
            BreakHandle.Set();
        }

        public static void StepOut() {
            SteppingOut = true;
            BreakHandle.Set();
        }

        public static void SendMessage(string message) {
            var cancellationToken = new CancellationToken();

            foreach (var socket in Sockets) {
                try {
                    var responseBytes = Encoding.UTF8.GetBytes(message);
                    socket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, cancellationToken);
                }catch(Exception e) {
                    Log.Write("Error while sending debug message - " + e);
                }
            }
        }
    }
#endif
}