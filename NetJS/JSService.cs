using Microsoft.ClearScript;
using System;
using System.Collections.Generic;

namespace NetJS { 
    public class JSService {

        public void RunCode(string code, JSApplication application, JSSession session, Action<object> resultCallback) {
            var request = new ScriptRequest(
                application.Compile(code), 
                application, 
                resultCallback, 
                session
            );
            application.AddRequest(request);
        }

        public void RunCodeSync(string code, JSApplication application, JSSession session, Action<object> resultCallback) {
            var request = new ScriptRequest(
                application.Compile(code),
                application,
                resultCallback,
                session
            );
            request.Call();
        }

        public void RunScript(string template, JSApplication application, JSSession session, Action<object> resultCallback) {
            var request = new ScriptRequest(
                application.Cache.GetScript(template, application), 
                application, 
                resultCallback, 
                session
            );
            application.AddRequest(request);
        }

        public void RunScriptSync(string template, JSApplication application, JSSession session, Action<object> resultCallback) {
            var request = new ScriptRequest(
                application.Cache.GetScript(template, application),
                application,
                resultCallback,
                session
            );
            request.Call();
        }

        // Every template is executed via this function
        public string RunTemplate(string template, Dictionary<string, object> arguments, ref JSApplication application, ref JSSession session, ref XHTMLMerge.SVCache svCache) {
            try {
                if (arguments == null) {
                    arguments = new Dictionary<string, object>();
                }

                NetJS.API.XDoc.SetXDocInfo(new API.XDoc.XDocInfo() { AppCache = null, SVCache = svCache });

                // TODO: better way to forward session
                dynamic args = new NetJSObject();
                foreach (var pair in arguments) {
                    args[pair.Key] = pair.Value;
                }

                var result = API.Functions.include(template, args);

                return result.ToString();
            } catch (ScriptEngineException se) {
                return se.ErrorDetails;
            } catch (Error e) {
                return e.ToString();
            } catch (Exception e) {
                return "System error - " + e.ToString();
            }
        }

        public string RunTemplate(string template, string data, ref JSApplication application, ref JSSession session) {
            var svCache = new XHTMLMerge.SVCache();
            return RunTemplate(template, data, ref application, ref session, ref svCache);
        }

        public string RunTemplate(string template, string data, ref JSApplication application, ref JSSession session, ref XHTMLMerge.SVCache svCache) {
            try {
                dynamic arguments = new NetJSObject();
                return RunTemplate(template, arguments, ref application, ref session, ref svCache);
            } catch { }
            
            return "invalid arguments (must be valid json)";
        }
    }
}
