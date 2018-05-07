using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util;

namespace NetJS.Core.Javascript {

    public enum ScopeType {
        Engine,
        Global,
        Function,
        Block
    }

    public class Scope {

        private class ScopeVariable {
            public Constant Value;
            public bool IsConstant;
            public string Type;

            public ScopeVariable(Constant value, bool isConstant, string type) {
                Value = value;
                IsConstant = isConstant;
                Type = type;
            }
        }

        private const int MAX_DEPTH = 1000;

        private Fast.Dict<ScopeVariable> _variables;

        public Scope ScopeParent { get; }
        public Scope StackParent { get; }
        public Node EntryNode { get; }

        public Engine Engine { get; }

        public ScopeType Type;

        public StringBuilder Buffer { get; }

        public Scope(Engine engine, StringBuilder buffer) {
            Engine = engine;
            Type = ScopeType.Engine;
            _variables = new Fast.Dict<ScopeVariable>();
            Buffer = buffer;

            if (Depth > MAX_DEPTH) {
                // Stackoverflow
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
                throw new InternalError($"No scope found with type '{type.ToString()}'");
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

                var value = Convert.ValueToJson(_variables.Get(key).Value, this);
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

        // Returns a variable, going up through the scopes
        public Constant GetVariable(string name) {
            ScopeVariable variable = null;
            if(_variables.TryGetValue(name, ref variable)) {
                return variable.Value;
            }

            if(ScopeParent != null) {
                return ScopeParent.GetVariable(name);
            }
            
            return Static.Undefined;
        }

        // Returns a variable, going up through the stack
        public Constant GetStackVariable(string name) {
            ScopeVariable variable = null;
            if (_variables.TryGetValue(name, ref variable)) {
                return variable.Value;
            }

            if (StackParent != null) {
                return StackParent.GetStackVariable(name);
            }

            return Static.Undefined;
        }

        // Sets a variable
        public void SetVariable(string name, Constant value) {
            if (!_variables.ContainsKey(name)) {
                if (ScopeParent != null) {
                    ScopeParent.SetVariable(name, value);
                    return;
                } else {
                    // The variable is not declared, so it can't be assigned to
                    throw new TypeError($"Assignment to undeclared variable '{name}'");
                }
            }

            var variable = _variables.Get(name);
            if (variable.IsConstant) {
                // Constant variables can't be reassigned
                throw new TypeError($"Assignment to constant variable '{name}'");
            }

            // TODO: optimize?
            // Check the type
            if (!Tool.CheckType(value, variable.Type)) {
                throw new TypeError($"Cannot assign value with type '{value.GetType()}' to static type '{variable.Type}'");
            }

            variable.Value = value;
        }

        public IEnumerable<string> Variables {
            get {
                var variables = _variables.Keys.ToList();
                if (ScopeParent != null) variables.AddRange(ScopeParent.Variables);
                return variables;
            }
        }

        public void DeclareVariable(string name, DeclarationScope declarationScope, bool isConstant, Constant value, string type = "any") {
            var scope = this;

            if (declarationScope == DeclarationScope.Function) {
                scope = GetScope(ScopeType.Function);
            } else if (declarationScope == DeclarationScope.Global) {
                scope = GetScope(ScopeType.Global);
            } else if (declarationScope == DeclarationScope.Engine) {
                scope = GetScope(ScopeType.Engine);
            } else if (declarationScope == DeclarationScope.Block) {
                // Check if the variable is not declared yet
                if (scope._variables.ContainsKey(name)) {
                    throw new SyntaxError($"Variable '{name}' has already been declared");
                }
            }

            // Check type
            if (!Tool.CheckType(value, type)) {
                throw new TypeError($"Cannot assign value with type '{value.GetType()}' to static type '{type}'");
            }

            scope._variables.Set(name, new ScopeVariable(value, isConstant, type));
        }

        public void DeclareVariable(Variable variable, DeclarationScope declarationScope, Constant value) {
            DeclareVariable(variable.Name, declarationScope, variable.Constant, value, variable.Type);
        }

        public void Set(string key, Constant value) {
            _variables.Set(key, new ScopeVariable(value, true, "any"));
        }

        public void Remove(string key) {
            _variables.Remove(key);
        }

        private int _depth = -1;
        public int Depth {
            get {
                // Check if the depth is already calculated
                if (_depth != -1) return _depth;

                var depth = StackParent != null ? StackParent.Depth + 1 : 0;
                
                // Store the depth so it doesn't have to be calculated every time
                _depth = depth;
                return depth;
            }
        }
    }
}
