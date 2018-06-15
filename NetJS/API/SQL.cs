using NetJS.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using String = NetJS.Core.String;

namespace NetJS.API {
    /// <summary>SQL class contains basic methods for communicating with SQL databases configured in the config.</summary>
    /// <remarks>This class can make SELECT, UPDATE, INSERT and custom query's.
    /// Configuration is needed to make a DB connection.</remarks>
    /// <configuration>To enable acces to an SQL database, the connection must be defined in a file in the root of the project named 'connections.json'.
    /// <example>Configuration structure:<code lang="json">{"Data": {"type": "sql", "connectionString": "Server=example.com;Database=ExampleName;UserId=sa;Password=test"}}</code></example></configuration>
    /// <example name="Query's">This example expects to have the following connection in the configuration:<code lang="json">{"Data": {"type": "sql", "connectionString": "Server=example.com;Database=ExampleName;UserId=sa;Password=test"}}</code>
    /// We can INSERT a new user and UPDATE his values:
    /// <code lang="javascript">var db = "ExampleName";
    /// var user = {name: "Hello World!", mail: "HelloWorld@example.com"};
    /// var id = SQL.insert(db, "users", user);
    /// user.name = "NewExample"
    /// // updating our db user based on the id of insert
    /// SQL.set(db, "users", id, user);</code>
    /// Now let's check if everything went fine:
    /// <code lang="javascript">console.log(SQL.get(db, "users")); //Prints all users</code>
    /// And to set the db back, we delete the row with a custom query:
    /// <code lang="javascript">var query = "DELETE FROM users WHERE id = " + id.toString() + ";";
    /// SQL.execute(db, query);</code></example>
    class SQL {

        /// <summary>Escapes a string to use in an SQL query.</summary>
        /// <param name="s">Value to escape (string)</param>
        /// <returns>The escaped string.</returns>
        /// <example><code lang="javascript">var escaped = SQL.escape(username);</code></example>
        public static Constant escape(Constant _this, Constant[] arguments, Agent agent) {
            var value = ((Core.String)arguments[0]).Value;
            return new Core.String(Util.SQL.Escape(value));
        }

        /// <summary>Creates an escaped SQL query string with parameters.</summary>
        /// <param name="query">Template query (string)</param>
        /// <param name="parameters">Parameters (object)</param>
        /// <returns>The generated query (string)</returns>
        /// <example><code lang="javascript">var query = SQL.format("SELECT * FROM Users WHERE Name = {name};", {name: "Alex Jones"});
        /// // SELECT * FROM Users Where Name = 'Alex Jones';</code></example>
        public static Constant format(Constant _this, Constant[] arguments, Agent agent) {
            var query = ((Core.String)arguments[0]).Value;
            var data = ((Core.Object)arguments[1]);

            foreach(var key in data.OwnPropertyKeys()) {
                var value = data.Get(key);
                string stringValue = null;

                if (value is Number n) {
                    stringValue = n.ToString();
                } else if (value is String s) {
                    stringValue = "'" + Util.SQL.Escape(s.Value) + "'";
                } else if (value is Null) {
                    stringValue = "NULL";
                } else {
                    throw new Error("Invalid type in SQL.format, only strings and integers allowed");
                }

                query = query.Replace("{" + key + "}", stringValue);
            }

            return new Core.String(query);
        }

        /// <summary>SQL.execute takes a connectionName and a query, executes the query and returns the result if the query is a SELECT statement.</summary>
        /// <param name="connectionName">Name of a configured connection</param>
        /// <param name="query">The query to be executed</param>
        /// <returns>the result if the query is a SELECT statement.</returns>
        /// <example><code lang="javascript">var id = SQL.execute("NETDB", "SELECT * FROM users;");</code></example>
        /// <exception cref="Error">Thrown when an error has been found while executing the query.</exception>
        public static Constant execute(Constant _this, Constant[] arguments, Agent agent) {
            var connectionName = ((Core.String)arguments[0]).Value;

            var application = (agent as NetJSAgent).Application;
            var connection = application.Connections.GetSqlConnection(connectionName);

            var query = ((Core.String)arguments[1]).Value;

            try {
                // TODO: Always return result for now, because of insert, then select possibility
                if (true /*query.Trim().ToUpper().StartsWith("SELECT")*/) {
                    var rows = Util.SQL.Get(connection, query, new SqlParameter[] { });

                    var result = new Core.Array(0, agent);

                    foreach (var row in rows) {
                        var rowObject = Core.Tool.Construct("Object", agent);
                        foreach (var key in row.Keys) {
                            var value = row[key];
                            if (value is string s) {
                                rowObject.Set(key, new Core.String(s));
                            } else if (value is int i) {
                                rowObject.Set(key, new Core.Number(i));
                            } else if (value is double d) {
                                rowObject.Set(key, new Core.Number(d));
                            } else if (value is bool b) {
                                rowObject.Set(key, Core.Boolean.Create(b));
                            } else if (value is byte bt) {
                                rowObject.Set(key, new Core.Number(bt));
                            } else if (value is Int16 i16) {
                                rowObject.Set(key, new Core.Number(i16));
                            } else if (value is Decimal dc) {
                                rowObject.Set(key, new Core.Number((double)dc));
                            } else if (value is DateTime date) {
                                var dateObj = Tool.Construct("Date", agent);

                                // TODO: don't hardcode this key
                                dateObj.Set("__date__", new Foreign(date));
                                rowObject.Set(key, dateObj);
                            } else if (value is DBNull) {
                                rowObject.Set(key, Core.Static.Null);
                            } else {
                                rowObject.Set(key, new Core.String("Unkown type - " + value.GetType()));
                            }
                        }

                        result.Add(rowObject);
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