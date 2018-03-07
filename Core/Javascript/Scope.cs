using System;
using System.Collections.Generic;
using Util;

namespace NetJS.Core.Javascript {

    public enum ResultType {
        None,
        String,
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

        private Fast.Dict<Constant> _variables;

        public Scope Parent { get; }
        public Node EntryNode { get; }

        public Engine Engine { get; }

        public ScopeType Type;

        public Scope(Engine engine) {
            Engine = engine;
            Type = ScopeType.Engine;
            _variables = new Fast.Dict<Constant>();
        }

        public Scope(Scope parent, Node entryNode, ScopeType type) : this(parent.Engine) {
            Parent = parent;
            EntryNode = entryNode;
            Type = type;
        }

        public Scope GetScope(ScopeType type) {
            if (Type == type) {
                return this;
            } else if (Parent != null) {
                return Parent.GetScope(type);
            } else {
                throw new InternalError($"No scope found with type '{Type.ToString()}'");
            }
        }

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
            while(scope.Parent != null && scope.EntryNode != null) {
                var entryLocation = Debug.GetNodeLocation(scope.EntryNode.Id);

                frames.Add(Parent.GetFrame(index, entryLocation));

                index++;
                scope = scope.Parent;
            }

            return frames;
        }

        public Debug.Scope GetScope(int index) {
            var localVariables = new Json();
            foreach (var key in _variables.Keys) {
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
            while (scope.Parent != null) {
                scope = scope.Parent;
                index++;

                scopes.Add(scope.GetScope(index));
            }

            return scopes;
        }

        public Constant GetVariable(string variable) {
            Constant value = null;
            if(_variables.TryGetValue(variable, ref value)) {
                return value;
            }

            if(Parent != null) {
                return Parent.GetVariable(variable);
            }
            
            return Static.Undefined;
        }

        public bool SetVariable(string variable, Constant value, bool create = true) {
            if (!_variables.ContainsKey(variable)) {
                if (Parent != null) {
                    var inParent = Parent.SetVariable(variable, value, false);
                    if (inParent) return true;
                }

                if (!create) return false;
            }

            _variables.Set(variable, value);
            return true;
        }

        public IEnumerable<string> Variables { get { return _variables.Keys; } }

        public bool DeclareVariable(string variable, Constant value, bool create = true) {
            _variables.Set(variable, value);
            return true;
        }
    }
}
