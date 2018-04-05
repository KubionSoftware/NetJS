using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util;

namespace NetJS.Core.Javascript {

    public enum ResultType {
        None,
        Buffer,
        Return,
        Break,
        Continue,
        Throw
    }

    public class Result {
        public ResultType Type;
        public Constant Constant;

        public Result(ResultType type) {
            Type = type;
            Constant = Static.Undefined;
        }

        public Result(ResultType type, Constant constant) {
            Type = type;
            Constant = constant;
        }
    }

    public enum ScopeType {
        Engine,
        Session,
        Template,
        Function,
        Block
    }

    public class Scope {

        private const int MAX_DEPTH = 1000;

        private Fast.Dict<Constant> _variables;

        public Scope ScopeParent { get; }
        public Scope StackParent { get; }
        public Node EntryNode { get; }

        public Engine Engine { get; }

        public ScopeType Type;

        public StringBuilder Buffer { get; }

        public Scope(Engine engine, StringBuilder buffer) {
            Engine = engine;
            Type = ScopeType.Engine;
            _variables = new Fast.Dict<Constant>();
            Buffer = buffer;

            if (Depth > 1000) {
                throw new RangeError("Maximum call stack size exceeded");
            }
        }

        public Scope(Scope scopeParent, Scope stackParent, Node entryNode, ScopeType type, StringBuilder buffer) : this(scopeParent.Engine, buffer) {
            ScopeParent = scopeParent;
            StackParent = stackParent;
            EntryNode = entryNode;
            Type = type;
        }

        public Scope GetScope(ScopeType type) {
            if (Type == type) {
                return this;
            } else if (ScopeParent != null) {
                return ScopeParent.GetScope(type);
            } else {
                throw new InternalError($"No scope found with type '{Type.ToString()}'");
            }
        }

#if debug_enabled
        public Debug.Frame GetFrame(int index, Debug.Location location) {
            return new Debug.Frame() {
                Index = 1,
                Name = "Frame Name",
                File = Debug.GetFileName(location.FileId),
                Line = location.LineNr
            };
        }

        public List<Debug.Frame> GetStackTrace(Debug.Location location) {
            var frames = new List<Debug.Frame>();
            frames.Add(GetFrame(1, location));

            var scope = this;
            var index = 2;
            while(scope.StackParent != null && scope.EntryNode != null) {
                var entryLocation = Debug.GetNodeLocation(scope.EntryNode.Id);

                frames.Add(StackParent.GetFrame(index, entryLocation));

                index++;
                scope = scope.StackParent;
            }

            return frames;
        }

        public Debug.Scope GetScope(int index) {
            var localVariables = new Json();
            foreach (var key in _variables.Keys) {
                // TODO: Remove this hack
                if (key.StartsWith("__") && key.EndsWith("__")) continue;

                var value = Convert.ValueToJson(_variables.Get(key), this);
                localVariables.Value[key] = value;
            }

            return new Debug.Scope() {
                Name = "Scope name " + index,
                Variables = localVariables
            };
        }

        public List<Debug.Scope> GetScopes() {
            var scopes = new List<Debug.Scope>();
            scopes.Add(GetScope(1));

            var scope = this;
            var index = 1;
            while (scope.ScopeParent != null) {
                scope = scope.ScopeParent;
                index++;

                scopes.Add(scope.GetScope(index));
            }

            return scopes;
        }
#endif

        public Constant GetVariable(string variable) {
            Constant value = null;
            if(_variables.TryGetValue(variable, ref value)) {
                return value;
            }

            if(ScopeParent != null) {
                return ScopeParent.GetVariable(variable);
            }
            
            return Static.Undefined;
        }

        public bool SetVariable(string variable, Constant value, bool create = true) {
            if (!_variables.ContainsKey(variable)) {
                if (ScopeParent != null) {
                    var inParent = ScopeParent.SetVariable(variable, value, false);
                    if (inParent) return true;
                }

                if (!create) return false;
            }

            _variables.Set(variable, value);
            return true;
        }

        public IEnumerable<string> Variables {
            get {
                var variables = _variables.Keys.ToList();
                if (ScopeParent != null) variables.AddRange(ScopeParent.Variables);
                return variables;
            }
        }

        public bool DeclareVariable(string variable, Constant value, bool create = true) {
            _variables.Set(variable, value);
            return true;
        }

        private int _depth = -1;
        public int Depth {
            get {
                if (_depth != -1) return _depth;
                var depth = StackParent != null ? StackParent.Depth + 1 : 0;
                _depth = depth;
                return depth;
            }
        }
    }
}
