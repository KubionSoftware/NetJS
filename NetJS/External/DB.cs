using NetJS.Javascript;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace NetJS.External {
    class DB {

        public static Constant get(Constant _this, Constant[] arguments, Scope scope) {
            var connectionName = ((Javascript.String)arguments[0]).Value;
            var connection = scope.Application.Connections.GetSqlConnection(connectionName);

            var table = ((Javascript.String)arguments[1]).Value;
            var query = "SELECT * FROM " + table;

            var singleResult = false;

            var parameters = new List<SqlParameter>();

            if(arguments.Length > 2) {
                query += " WHERE ";

                var argument = arguments[2];
                if (argument is Javascript.Number) {
                    query += "id = @id";
                    parameters.Add(new SqlParameter("id", (int)((Javascript.Number)argument).Value));
                    singleResult = true;
                } else if (argument is Javascript.Object) {
                    var obj = (Javascript.Object)argument;
                    foreach (var key in obj.GetKeys()) {
                        var property = obj.Get(key);

                        if(property is Javascript.String) {
                            if (parameters.Count > 0) query += " AND";
                            query += key + " = @" + key;
                            parameters.Add(new SqlParameter(key, ((Javascript.String)property).Value));
                        }
                    }
                } else {
                    throw new Exception("Invalid argument type for DB.get");
                }
            }

            var results = Util.SQL.Get(connection, query, parameters.ToArray());
            if (singleResult) {
                if(results.Count > 0) {
                    return Convert.JsonToObject(results[0], scope);
                } else {
                    return Static.Undefined;
                }
            } else {
                // TODO: without convert to List<object>
                return Convert.JsonToValue(results.Select(result => (object)result).ToList(), scope);
            }
        }

        public static Constant set(Constant _this, Constant[] arguments, Scope scope) {
            var connectionName = ((Javascript.String)arguments[0]).Value;
            var connection = scope.Application.Connections.GetSqlConnection(connectionName);

            var table = ((Javascript.String)arguments[1]).Value;
            var id = ((Javascript.Number)arguments[2]).Value;
            var obj = ((Javascript.Object)arguments[3]);

            var query = "UPDATE " + table + " SET";

            var parameters = new List<SqlParameter>();

            foreach (var key in obj.GetKeys()) {
                var property = obj.Get(key);
                if (property is Javascript.String) {
                    if (parameters.Count > 0) query += ",";
                    query += " " + key + " = @" + key;
                    parameters.Add(new SqlParameter(key, ((Javascript.String)property).Value));
                }
            }

            query += " WHERE id = @id";
            parameters.Add(new SqlParameter("id", (int)id));

            Util.SQL.Execute(connection, query, parameters.ToArray());

            return new Javascript.Boolean(true);
        }

        public static Constant insert(Constant _this, Constant[] arguments, Scope scope) {
            var connectionName = ((Javascript.String)arguments[0]).Value;
            var connection = scope.Application.Connections.GetSqlConnection(connectionName);

            var table = ((Javascript.String)arguments[1]).Value;
            var obj = ((Javascript.Object)arguments[2]);

            var names = new List<string>();
            var parameters = new List<SqlParameter>();

            foreach (var key in obj.GetKeys()) {
                var property = obj.Get(key);
                if (property is Javascript.String) {
                    names.Add(key);
                    parameters.Add(new SqlParameter(key, ((Javascript.String)property).Value));
                }
            }

            return new Javascript.Number(Util.SQL.Insert(connection, table, names.ToArray(), parameters.ToArray()));
        }

        public static Constant execute(Constant _this, Constant[] arguments, Scope scope) {
            var connectionName = ((Javascript.String)arguments[0]).Value;
            var connection = scope.Application.Connections.GetSqlConnection(connectionName);

            var query = ((Javascript.String)arguments[1]).Value;

            if (query.StartsWith("SELECT")) {
                var rows = Util.SQL.Get(connection, query, new SqlParameter[] { });

                var result = Tool.Construct("Object", scope);

                var rowsArray = new Javascript.Array();
                
                foreach(var row in rows) {
                    var rowObject = Tool.Construct("Object", scope);
                    foreach(var key in row.Keys) {
                        var value = row[key];
                        if (value is string) {
                            rowObject.Set(key, new Javascript.String((string)value));
                        } else if (value is int) {
                            rowObject.Set(key, new Javascript.Number((int)value));
                        } else if (value is DateTime) {
                            rowObject.Set(key, new Javascript.String(((DateTime)value).ToString()));
                        } else if (value is DBNull) {
                            // TODO: make this just Javascript.Null
                            rowObject.Set(key, new Javascript.String("<null>"));
                        } else {
                            rowObject.Set(key, new Javascript.String("Unkown type - " + value.GetType()));
                        }
                    }

                    rowsArray.List.Add(rowObject);
                }

                result.Set("result", rowsArray);

                return result;
            } else {
                Util.SQL.Execute(connection, query);
                return Static.Undefined;
            }
        }
    }
}