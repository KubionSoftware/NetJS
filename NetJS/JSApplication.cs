using System;
using System.Reflection;
using System.Web;
using Microsoft.ClearScript.V8;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NetJS {
    public class JSApplication : JSStorage {

        public Cache Cache { get; }
        public Connections Connections { get; }
        public Settings Settings { get; }

        private V8ScriptEngine _engine;

        private ConcurrentQueue<Request> _requests = new ConcurrentQueue<Request>();
        private ConcurrentQueue<Callback> _callbacks = new ConcurrentQueue<Callback>();
        private List<TimeOut> _timeouts = new List<TimeOut>();

        public XDocServices.XDocService XDocService { get; }

        private Action<JSApplication> _afterStart;
        private Action<Exception> _onError;

        public JSApplication(string rootDir, Action<JSApplication> afterStart, Action<Exception> onError) {
            if (rootDir == null) {
                rootDir = AppDomain.CurrentDomain.BaseDirectory;
            }

            Settings = new Settings(rootDir);

            Cache = new Cache();

            _afterStart = afterStart;
            _onError = onError;
            Restart();

            Connections = new Connections(Settings);

            XDocService = new XDocServices.XDocService();

            var eventThread = new Thread(EventLoop);
            eventThread.Start();

            var fileThread = new Thread(FileLoop);
            fileThread.Start();

            Tool.Init(_engine);
        }

        public void EventLoop() {
            while (true) {
                for (var i = 0; i < _timeouts.Count; i++) {
                    var timeout = _timeouts[i];
                    if (timeout.ShouldTrigger(DateTime.Now)) {
                        _timeouts.RemoveAt(i);
                        i--;
                        timeout.Call();
                    }
                }

                while (_callbacks.TryDequeue(out Callback callback)) {
                    callback.Call();
                }

                while (_requests.TryDequeue(out Request request)) {
                    request.Call();
                }

                Thread.Sleep(1);
            }
        }

        public void FileLoop() {
            while (true) {
                foreach (var file in _required) {
                    var lastChanged = System.IO.File.GetLastWriteTime(file.Path);
                    if (lastChanged > file.LastChanged) {
                        Restart();
                        file.LastChanged = lastChanged;
                        break;
                    }
                }

                Thread.Sleep(1000);
            }
        }

        public void Restart() {
            _engine = new V8ScriptEngine(V8ScriptEngineFlags.EnableDebugging, 9222);
            _required.Clear();

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

            _afterStart(this);
        }

        public void Error(Exception e) {
            _onError(e);
        }

        public void ProcessXDocRequest(HttpContext context) {
            XDocService.ProcessRequest(context);
        }

        public void AddHostObject(string name, object obj) {
            _engine.AddHostObject(name, obj);
        }

        public void AddHostType(Type type) {
            _engine.AddHostType(type);
        }

        public void AddHostFunctions(Type type) {
            _engine.AddHostType(type);
            foreach(var member in type.GetMembers(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static)) {
                var name = member.Name.Replace("@", "");
                try {
                    var code = $"var {name} = {type.Name}.{name};";
                    _engine.Execute(code);
                }catch (Exception e) {
                    Console.Write("ERROR");
                }
            }
        }

        private class File {
            public string Path;
            public DateTime LastChanged;
        }

        private List<File> _required = new List<File>();

        public void Require(string template) {
            var path = Cache.GetPath(template, this, true);
            if (_required.Any(file => file.Path == path)) return;
            
            var script = Cache.GetScript(template, this);
            Evaluate(script);

            _required.Add(new File() {
                Path = path,
                LastChanged = System.IO.File.GetLastWriteTime(path)
            });
        }

        public V8Script Compile(string file, string code) {
            return _engine.Compile(file, code);
        }

        public V8Script Compile(string code) {
            return _engine.Compile("", code);
        }

        public dynamic Evaluate(string code) {
            return _engine.Evaluate("", code);
        }

        public dynamic Evaluate(V8Script script) {
            return _engine.Evaluate(script);
        }

        public void AddRequest(Request request) {
            _requests.Enqueue(request);
        }

        public void AddCallback(dynamic function, object argument0, State state, object argument1 = null, object argument2 = null, object argument3 = null) {
            _callbacks.Enqueue(new Callback(function, state, argument0, argument1, argument2, argument3));
        }

        public void AddTimeOut(int time, dynamic function, State state) {
            _timeouts.Add(new TimeOut(time, function, state));
        }

        public string GetCurrentLocation() {
            var stack = Tool.GetStack();
            if (stack == null) return "";
            var top = stack[0];
            if (Tool.IsUndefined(top)) return "";
            return top.getScriptNameOrSourceURL();
        }
    }
}