using Microsoft.ClearScript;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Util;

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
    public class SQLAPI {

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
                    var list = new List<object>();

                    using (var dictSelectCommand = new SqlCommand(query, connection)) {
                        using (var reader = dictSelectCommand.ExecuteReader()) {
                            while (reader.Read()) {
                                var row = new Dictionary<string, object>();

                                for (var i = 0; i < reader.FieldCount; i++) {
                                    row[reader.GetName(i)] = SQLToJavascript(reader.GetValue(i));
                                }

                                list.Add(row);
                            }
                        }
                    }

                    var json = JsonParser.ValueToString(list);
                    application.AddCallback(resolve, json, state);
                } catch (Exception e) {
                    application.AddCallback(reject, $"{e.Message}", state);
                }
            });
        }

        private static object SQLToJavascript(object value) {
            if (value is string s) {
                return s;
            } else if (value is int i) {
                return (double)i;
            } else if (value is double d) {
                return d;
            } else if (value is bool b) {
                return b;
            } else if (value is byte bt) {
                return (double)bt;
            } else if (value is Int16 i16) {
                return (double)i16;
            } else if (value is Decimal dc) {
                return (double)dc;
            } else if (value is DateTime date) {
                return date.ToString();
            } else if (value is DBNull) {
                return null;
            } else {
                State.Application.Error(new Error("Unkown SQL type - " + value.GetType()), ErrorStage.Runtime);
                return null;
            }
        }
    }
}