using System;
using System.Collections.Generic;
using Util;

namespace NetJS.Javascript {
    public class Scope {

        private Fast.Dict<Constant> _variables;

        public Scope Parent { get; }
        public Node EntryNode { get; }

        public JSApplication Application { get; }
        public JSSession Session { get; }
        public XHTMLMerge.SVCache SVCache { get; }

        public Scope(JSApplication application, JSSession session, XHTMLMerge.SVCache svCache) {
            Application = application;
            Session = session;
            SVCache = svCache;

            _variables = new Fast.Dict<Constant>();
        }

        public Scope(Scope parent, Node entryNode) : this(parent.Application, parent.Session, parent.SVCache) {
            Parent = parent;
            EntryNode = entryNode;
        }

        public Scope(Scope parent, Node entryNode, JSSession session, XHTMLMerge.SVCache svCache) : this(parent.Application, session, svCache) {
            Parent = parent;
            EntryNode = entryNode;
        }

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

        public bool DeclareVariable(string variable, Constant value, bool create = true) {
            _variables.Set(variable, value);
            return true;
        }
    }
}
