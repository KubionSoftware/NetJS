using Microsoft.ClearScript;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

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
    public class SQL {

        /// <summary>Escapes a string to use in an SQL query.</summary>
        /// <param name="s">Value to escape (string)</param>
        /// <returns>The escaped string.</returns>
        /// <example><code lang="javascript">var escaped = SQL.escape(username);</code></example>
        public static string escape(string value) {
            return Util.SQL.Escape(value);
        }

        /// <summary>Creates an escaped SQL query string with parameters.</summary>
        /// <param name="query">Template query (string)</param>
        /// <param name="parameters">Parameters (object)</param>
        /// <returns>The generated query (string)</returns>
        /// <example><code lang="javascript">var query = SQL.format("SELECT * FROM Users WHERE Name = {name};", {name: "Alex Jones"});
        /// // SELECT * FROM Users Where Name = 'Alex Jones';</code></example>
        public static string format(string query, ScriptObject data) {
            foreach(var key in data.GetDynamicMemberNames()) {
                var value = Tool.GetValue(data, key);
                string stringValue = null;

                if (value is int n) {
                    stringValue = n.ToString();
                } else if (value is string s) {
                    stringValue = "'" + Util.SQL.Escape(s) + "'";
                } else if (value is null) {
                    stringValue = "NULL";
                } else {
                    State.Application.Error(new Error("Invalid type in SQL.format, only strings and integers allowed"), ErrorStage.Runtime);
                }

                query = query.Replace("{" + key + "}", stringValue);
            }

            return query;
        }

        /// <summary>SQL.execute takes a connectionName and a query, executes the query and returns the result if the query is a SELECT statement.</summary>
        /// <param name="connectionName">Name of a configured connection</param>
        /// <param name="query">The query to be executed</param>
        /// <returns>the result if the query is a SELECT statement.</returns>
        /// <example><code lang="javascript">var id = SQL.execute("NETDB", "SELECT * FROM users;");</code></example>
        public static dynamic execute(string connectionName, string query) {
            var application = State.Application;
            var state = State.Get();

            return Tool.CreatePromise((resolve, reject) => {
                try {
                    var connection = application.Connections.GetSqlConnection(connectionName);
                    var rows = Util.SQL.Get(connection, query, new SqlParameter[] { });

                    var result = new List<object>();

                    foreach (var row in rows) {
                        dynamic rowObject = new NetJSObject();

                        foreach (var key in row.Keys) {
                            var value = row[key];

                            if (value is string s) {
                                rowObject[key] = s;
                            } else if (value is int i) {
                                rowObject[key] = i;
                            } else if (value is double d) {
                                rowObject[key] = d;
                            } else if (value is bool b) {
                                rowObject[key] = b;
                            } else if (value is byte bt) {
                                rowObject[key] = bt;
                            } else if (value is Int16 i16) {
                                rowObject[key] = i16;
                            } else if (value is Decimal dc) {
                                rowObject[key] = (double)dc;
                            } else if (value is DateTime date) {
                                rowObject[key] = date;
                            } else if (value is DBNull) {
                                rowObject[key] = null;
                            } else {
                                State.Application.Error(new Error("Unkown SQL type - " + value.GetType()), ErrorStage.Runtime);
                            }
                        }

                        result.Add(rowObject);
                    }

                    application.AddCallback(resolve, Tool.ToArray(result.ToArray()), state);
                } catch (Exception e) {
                    application.AddCallback(reject, $"SQL error in query: '{query}'\n{e.Message}", state);
                }
            });
        }
    }
}