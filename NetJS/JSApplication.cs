using System;
using System.Reflection;
using System.Web;
using Microsoft.ClearScript.V8;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace NetJS {
    public class JSApplication : JSStorage {

        // Managers
        public Cache Cache { get; }
        public Connections Connections { get; }
        public Settings Settings { get; }
        public XDocServices.XDocService XDocService { get; }

        // Engine
        private V8ScriptEngine _engine;

        // Debug proxy
        private Proxy _proxy;
        private int _engineDebugPort;
        private const int DebugPortStart = 9223;
        private const int DebugPortEnd = 9323;

        // Event queues
        private ConcurrentQueue<Request> _requests = new ConcurrentQueue<Request>();
        private ConcurrentQueue<Callback> _callbacks = new ConcurrentQueue<Callback>();
        private List<TimeOut> _timeouts = new List<TimeOut>();
        private const int EventLimit = 10;

        // Callbacks
        private Action<JSApplication> _afterStart;
        private Action<Exception, ErrorStage> _onError;

        // Class to store required file
        private class File {
            public string Path;
            public DateTime LastChanged;
        }

        // List of all required source files
        private List<File> _required = new List<File>();

        public JSApplication(string rootDir, Action<JSApplication> afterStart, Action<Exception, ErrorStage> onError) {
            if (rootDir == null) {
                // Set the root directory to the application base directory if not specified
                rootDir = AppDomain.CurrentDomain.BaseDirectory;
            }

            // Create managers
            Settings = new Settings(rootDir);
            Cache = new Cache();
            Connections = new Connections(Settings);
            XDocService = new XDocServices.XDocService();

            // Store callbacks
            _afterStart = afterStart;
            _onError = onError;

            // Start the proxy
            _proxy = new Proxy(Settings.DebugPort, () => {
                return _engineDebugPort;
            });
            _proxy.Start();

            // Start the engine
            Restart();

            // Create thread for event loop
            var eventThread = new Thread(EventLoop);
            eventThread.Start();

            // Create thread for file update checking
            var fileThread = new Thread(FileLoop);
            fileThread.Start();
        }

        // Handles all events
        public void EventLoop() {
            while (true) {
                var events = 0;
                var eventLimitExceeded = false;

                // Check if timeouts should trigger and call then
                for (var i = 0; i < _timeouts.Count; i++) {
                    if (events >= EventLimit) {
                        eventLimitExceeded = true;
                        break;
                    }

                    var timeout = _timeouts[i];
                    if (timeout.ShouldTrigger(DateTime.Now)) {
                        _timeouts.RemoveAt(i);
                        i--;
                        timeout.Call();
                        events++;
                    }
                }

                events = 0;

                // Handle all queued callbacks
                while (_callbacks.TryDequeue(out Callback callback)) {
                    if (events >= EventLimit) {
                        eventLimitExceeded = true;
                        break;
                    }

                    callback.Call();
                }

                events = 0;

                // Handle all queued requests
                while (_requests.TryDequeue(out Request request)) {
                    if (events >= EventLimit) {
                        eventLimitExceeded = true;
                        break;
                    }

                    request.Call();
                }

                if (!eventLimitExceeded) {
                    // Sleep to keep from using all CPU
                    Thread.Sleep(1);
                }
            }
        }

        // Checks if a required source file has been changed and restarts when that happens
        public void FileLoop() {
            while (true) {
                try {
                    // Loop all required files
                    for (var i = 0; i < _required.Count; i++) {
                        var file = _required[i];

                        var lastChanged = System.IO.File.GetLastWriteTime(file.Path);
                        if (lastChanged > file.LastChanged) {
                            // File has been written to after last check
                            Restart();
                            file.LastChanged = lastChanged;
                            break;
                        }
                    }
                } catch (Exception e) {
                    API.Log.write("Error in FileLoop: " + e.ToString());
                }

                // Check every second
                Thread.Sleep(1000);
            }
        }

        public void Restart() {
            // Create a new engine
            if (_engine != null) _engine.Dispose();

            // Find available port
            _engineDebugPort = -1;
            for (var p = DebugPortStart; p <= DebugPortEnd; p++) {
                if (Proxy.IsPortAvailable(p)) {
                    _engineDebugPort = p;
                    break;
                }
            }

            if (_engineDebugPort != -1) {
                // Enable debugging on port "port"
                _engine = new V8ScriptEngine(V8ScriptEngineFlags.EnableDebugging, _engineDebugPort);
            } else {
                // Create engine without debugging because no port is open 
                // (this should never happen since there are a lot of ports in the range)
                _engine = new V8ScriptEngine();
            }

            // Clear the list of required files
            _required.Clear();

            // Define the NetJS API
            AddHostType(typeof(API.HTTP));
            AddHostType(typeof(API.SQL));
            AddHostType(typeof(API.IO));
            AddHostType(typeof(API.Log));
            AddHostType(typeof(API.Application));
            AddHostType(typeof(API.Session));
            AddHostType(typeof(API.XDoc));
            AddHostType(typeof(API.Base64));
            AddHostType(typeof(API.Buffer));
            AddHostType(typeof(API.Windows));
            AddHostType(typeof(API.DLL));
            AddHostType(typeof(API.XML));
            AddHostFunctions(typeof(API.Functions));

            // Initialize the tool functions
            Tool.Init(_engine);

            // Call the after start callback
            _afterStart(this);
        }

        // Calls the error callback
        public void Error(Exception e, ErrorStage stage) {
            _onError(e, stage);
        }

        // Processes a request with XDoc
        public void ProcessXDocRequest(HttpContext context) {
            XDocService.ProcessRequest(context);
        }

        // Add a C# object to the V8 engine
        public void AddHostObject(string name, object obj) {
            _engine.AddHostObject(name, obj);
        }

        // Add a C# type to the V8 engine
        public void AddHostType(Type type) {
            _engine.AddHostType(type);
        }

        // Add all functions defined in the type to the V8 engine
        public void AddHostFunctions(Type type) {
            // Temporarily add the type to the engine, so we can access its functions
            _engine.AddHostType(type);

            // Loop all public static functions that were declared in the type
            foreach(var member in type.GetMembers(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static)) {
                // Remove @, this is to allow functions with names that are C# keywords like "out"
                var name = member.Name.Replace("@", "");

                try {
                    // Move the function from the object to the global scope
                    var code = $"var {name} = {type.Name}.{name};";
                    _engine.Execute(code);
                }catch (Exception e) {
                    Console.Write("ERROR");
                }
            }
        }

        // Load the source from the template in the V8 engine
        public void Require(string template) {
            // Get the relative path for this project
            var path = Cache.GetPath(template, this, true);

            // If is already loaded, return
            if (_required.Any(file => file.Path == path)) return;
            
            // Get the script from the cache
            var script = Cache.GetScript(template, this);

            // Run the script
            try {
                Evaluate(script);
            } catch (Exception e) {
                Error(e, ErrorStage.Compilation);
            }

            // Add the script to the list of required files
            _required.Add(new File() {
                Path = path,
                LastChanged = System.IO.File.GetLastWriteTime(path)
            });
        }

        // Simple engine functions
        public V8Script Compile(string file, string code)   => _engine.Compile(file, code);
        public V8Script Compile(string code)                => Compile("", code);
        public dynamic Evaluate(string code)                => _engine.Evaluate("", code);
        public dynamic Evaluate(V8Script script)            => _engine.Evaluate(script);

        // Adds the request to the request queue
        public void AddRequest(Request request) {
            _requests.Enqueue(request);
        }

        // Adds the callback to the callback queue
        public void AddCallback(dynamic function, object argument0, State state, object argument1 = null, object argument2 = null, object argument3 = null) {
            _callbacks.Enqueue(new Callback(function, state, argument0, argument1, argument2, argument3));
        }

        // Adds the timeout to the timeout queue
        public void AddTimeOut(int time, dynamic function, State state) {
            _timeouts.Add(new TimeOut(time, function, state));
        }

        // Gets the current source file path from the stack trace
        public string GetCurrentLocation() {
            var stack = Tool.GetStack();
            if (stack == null) return "";
            var top = stack[0];
            if (Tool.IsUndefined(top)) return "";
            return top.getScriptNameOrSourceURL();
        }

        // Returns the global object
        public dynamic GetGlobal() => _engine.Script;
    }
}