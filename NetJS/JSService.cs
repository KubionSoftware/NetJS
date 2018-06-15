using System;
using System.Linq;
using System.Text;
using System.Web;
using NetJS.Core;
using NetJS.Core.API;

namespace NetJS { 
    public class JSService {

        public string RunCode(string code, ref JSApplication application, ref JSSession session) {
            try {
                var agent = new NetJSAgent(application.Realm) {
                    Application = application,
                    Session = session
                };

                var script = ScriptRecord.ParseScript(code, application.Realm, -1);
                var result = script.Evaluate(agent);

                if (result.Value is Core.String s) {
                    return s.Value;
                } else {
                    return agent.Running.Buffer.ToString();
                }
            } catch (Error e) {
                return e.ToString();
            } catch (Exception e) {
                return "System error - " + e.ToString();
            }
        }

        // Every template is executed via this function
        public string RunTemplate(string template, Core.Object arguments, ref JSApplication application, ref JSSession session, ref XHTMLMerge.SVCache svCache) {
            try {
                var agent = new NetJSAgent(application.Realm) {
                    Application = application,
                    Session = session
                };

                if (arguments == null) {
                    arguments = Core.Tool.Construct("Object", agent);
                }

                NetJS.API.XDoc.SetXDocInfo(new API.XDoc.XDocInfo() { AppCache = null, SVCache = svCache }, agent);

                // TODO: better way to forward session
                var result = API.Functions.include(
                    Static.Undefined,
                    new Constant[] { new Core.String(template), arguments },
                    agent
                );

                if (result is Core.String s) {
                    return s.Value;
                } else {
                    return agent.Running.Buffer.ToString();
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
                Core.Constant arguments;
                if (data.Length == 0) {
                    arguments = Core.Tool.Construct("Object", application.Realm.GetAgent());
                } else {
                    arguments = JSONAPI.parse(null, new[] { new Core.String(data) }, application.Realm.GetAgent());
                }

                if (arguments is Core.Object a) {
                    return RunTemplate(template, a, ref application, ref session, ref svCache);
                }
            } catch { }
            
            return "invalid arguments (must be valid json)";
        }

        public string RunTemplate(string template, Core.Object arguments, ref JSApplication application) {
            if(application == null) {
                application = new JSApplication();
            }

            var session = new JSSession();

            return RunTemplate(template, arguments, ref application, ref session);
        }

        public string RunTemplate(string template, Core.Object arguments, ref JSApplication application, ref JSSession session) {
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
