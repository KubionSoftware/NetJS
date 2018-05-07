using System;
using System.Linq;
using System.Text;
using System.Web;
using NetJS.Core.Javascript;
using NetJS.Core.API;
using NetJS.Core;

namespace NetJS { 
    public class JSService {

        // Every template is executed via this function
        public string RunTemplate(string template, Core.Javascript.Object arguments, ref JSApplication application, ref JSSession session, ref XHTMLMerge.SVCache svCache) {
            try {
                if (arguments == null) {
                    arguments = Core.Tool.Construct("Object", application.Engine.GlobalScope);
                }

                var scope = new Scope(application.Engine.GlobalScope, null, null, ScopeType.Function, new StringBuilder());
                scope.DeclareVariable("__application__", Core.Javascript.DeclarationScope.Function, true, new Foreign(application));
                scope.DeclareVariable("__session__", Core.Javascript.DeclarationScope.Function, true, new Foreign(session));

                NetJS.API.XDoc.SetXDocInfo(new API.XDoc.XDocInfo() { AppCache = null, SVCache = svCache }, scope);

                // TODO: better way to forward session
                var result = API.Functions.include(
                    Static.Undefined,
                    new Constant[] { new Core.Javascript.String(template), arguments },
                    scope
                );

                if (result is Core.Javascript.String s) {
                    return s.Value;
                } else {
                    return scope.Buffer.ToString();
                }
            } catch (Error e) {
                return e.ToString();
            } catch (Exception e) {
                return "System error - " + e.ToString();
            }
        }

        public string RunTemplate(string template) {
            return RunTemplate(template, "");
        }

        public string RunTemplate(string template, string data) {
            var application = new JSApplication();
            var session = new JSSession();
            var svCache = new XHTMLMerge.SVCache();

            return RunTemplate(template, data, ref application, ref session, ref svCache);
        }

        public string RunTemplate(string template, string data, ref XHTMLMerge.SVCache svCache) {
            var application = new JSApplication();
            var session = new JSSession();

            return RunTemplate(template, data, ref application, ref session, ref svCache);
        }

        public string RunTemplate(string template, string data, ref JSApplication application, ref JSSession session) {
            var svCache = new XHTMLMerge.SVCache();
            return RunTemplate(template, data, ref application, ref session, ref svCache);
        }

        public string RunTemplate(string template, string data, ref JSApplication application, ref JSSession session, ref XHTMLMerge.SVCache svCache) {
            try {
                Core.Javascript.Constant arguments;
                if (data.Length == 0) {
                    arguments = Core.Tool.Construct("Object", application.Engine.GlobalScope);
                } else {
                    arguments = JSON.parse(null, new[] { new Core.Javascript.String(data) }, application.Engine.GlobalScope);
                }

                if (arguments is Core.Javascript.Object a) {
                    return RunTemplate(template, a, ref application, ref session, ref svCache);
                }
            } catch { }
            
            return "invalid arguments (must be valid json)";
        }

        public string RunTemplate(string template, Core.Javascript.Object arguments, ref JSApplication application) {
            if(application == null) {
                application = new JSApplication();
            }

            var session = new JSSession();

            return RunTemplate(template, arguments, ref application, ref session);
        }

        public string RunTemplate(string template, Core.Javascript.Object arguments, ref JSApplication application, ref JSSession session) {
            if (application == null) {
                application = new JSApplication();
            }
            if (session == null) {
                session = new JSSession();
            }

            var svCache = new XHTMLMerge.SVCache();

            return RunTemplate(template, arguments, ref application, ref session, ref svCache);
        }
    }
}
