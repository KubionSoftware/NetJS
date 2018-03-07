using NetJS.Core.Javascript;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace NetJS.External {
    class SQL {

        public static Constant get(Constant _this, Constant[] arguments, Scope scope) {
            var connectionName = ((Core.Javascript.String)arguments[0]).Value;

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");
            var connection = application.Connections.GetSqlConnection(connectionName);

            var table = ((Core.Javascript.String)arguments[1]).Value;
            var query = "SELECT * FROM " + table;

            var singleResult = false;

            var parameters = new List<SqlParameter>();

            if(arguments.Length > 2) {
                query += " WHERE ";

                var argument = arguments[2];
                if (argument is Core.Javascript.Number) {
                    query += "id = @id";
                    parameters.Add(new SqlParameter("id", (int)((Core.Javascript.Number)argument).Value));
                    singleResult = true;
                } else if (argument is Core.Javascript.Object) {
                    var obj = (Core.Javascript.Object)argument;
                    foreach (var key in obj.GetKeys()) {
                        var property = obj.Get(key);

                        if(property is Core.Javascript.String) {
                            if (parameters.Count > 0) query += " AND";
                            query += key + " = @" + key;
                            parameters.Add(new SqlParameter(key, ((Core.Javascript.String)property).Value));
                        }
                    }
                } else {
                    throw new Exception("Invalid argument type for DB.get");
                }
            }

            var results = Util.SQL.Get(connection, query, parameters.ToArray());
            if (singleResult) {
                if(results.Count > 0) {
                    return Core.Convert.JsonToObject(results[0], scope);
                } else {
                    return Static.Undefined;
                }
            } else {
                // TODO: without convert to List<object>
                return Core.Convert.JsonToValue(results.Select(result => (object)result).ToList(), scope);
            }
        }

        public static Constant set(Constant _this, Constant[] arguments, Scope scope) {
            var connectionName = ((Core.Javascript.String)arguments[0]).Value;

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");
            var connection = application.Connections.GetSqlConnection(connectionName);

            var table = ((Core.Javascript.String)arguments[1]).Value;
            var id = ((Core.Javascript.Number)arguments[2]).Value;
            var obj = ((Core.Javascript.Object)arguments[3]);

            var query = "UPDATE " + table + " SET";

            var parameters = new List<SqlParameter>();

            foreach (var key in obj.GetKeys()) {
                var property = obj.Get(key);
                if (property is Core.Javascript.String) {
                    if (parameters.Count > 0) query += ",";
                    query += " " + key + " = @" + key;
                    parameters.Add(new SqlParameter(key, ((Core.Javascript.String)property).Value));
                }
            }

            query += " WHERE id = @id";
            parameters.Add(new SqlParameter("id", (int)id));

            Util.SQL.Execute(connection, query, parameters.ToArray());

            return new Core.Javascript.Boolean(true);
        }

        public static Constant insert(Constant _this, Constant[] arguments, Scope scope) {
            var connectionName = ((Core.Javascript.String)arguments[0]).Value;

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");
            var connection = application.Connections.GetSqlConnection(connectionName);

            var table = ((Core.Javascript.String)arguments[1]).Value;
            var obj = ((Core.Javascript.Object)arguments[2]);

            var names = new List<string>();
            var parameters = new List<SqlParameter>();

            foreach (var key in obj.GetKeys()) {
                var property = obj.Get(key);
                if (property is Core.Javascript.String) {
                    names.Add(key);
                    parameters.Add(new SqlParameter(key, ((Core.Javascript.String)property).Value));
                }
            }

            return new Core.Javascript.Number(Util.SQL.Insert(connection, table, names.ToArray(), parameters.ToArray()));
        }

        public static Constant execute(Constant _this, Constant[] arguments, Scope scope) {
            var connectionName = ((Core.Javascript.String)arguments[0]).Value;

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");
            var connection = application.Connections.GetSqlConnection(connectionName);

            var query = ((Core.Javascript.String)arguments[1]).Value;

            try {
                if (query.Trim().ToUpper().StartsWith("SELECT")) {
                    var rows = Util.SQL.Get(connection, query, new SqlParameter[] { });

                    var result = new Core.Javascript.Array();

                    foreach (var row in rows) {
                        var rowObject = Core.Tool.Construct("Object", scope);
                        foreach (var key in row.Keys) {
                            var value = row[key];
                            if (value is string s) {
                                rowObject.Set(key, new Core.Javascript.String(s));
                            } else if (value is int i) {
                                rowObject.Set(key, new Core.Javascript.Number(i));
                            } else if (value is double d) {
                                rowObject.Set(key, new Core.Javascript.Number(d));
                            } else if (value is DateTime date) {
                                rowObject.Set(key, new Core.Javascript.Date(date));
                            } else if (value is DBNull) {
                                // TODO: make this just Javascript.Null
                                rowObject.Set(key, Core.Javascript.Static.Null);
                            } else {
                                rowObject.Set(key, new Core.Javascript.String("Unkown type - " + value.GetType()));
                            }
                        }

                        result.List.Add(rowObject);
                    }

                    return result;
                } else {
                    Util.SQL.Execute(connection, query);
                    return Static.Undefined;
                }
            } catch (Exception e) {
                throw new Error($"SQL error in query: '{query}'\n{e.Message}");
            }
        }
    }
}