using NetJS.Core.Javascript;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using String = NetJS.Core.Javascript.String;

namespace NetJS.External
{
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
    class SQL
    {
        /// <summary>SQL.get takes a connectionName and a tablename and executes a SELECT * FROM [table] query which result will be returned.</summary>
        /// <param name="connectionName">Name of a configured connection</param>
        /// <param name="table">Tablename for SELECT statement</param>
        /// <returns>Response from database, in json.</returns>
        /// <example><code lang="javascript">var results = SQL.get("NetDB", "users");</code></example>
        /// <exception cref="InternalError">Thrown when no application can be found in application scope.</exception>
        /// <exception cref="Exception">Thrown when the type of the DB result can't be identified.</exception>
        public static Constant get(Constant _this, Constant[] arguments, Scope scope)
        {
            var connectionName = ((Core.Javascript.String) arguments[0]).Value;

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");
            var connection = application.Connections.GetSqlConnection(connectionName);

            var table = ((Core.Javascript.String) arguments[1]).Value;
            var query = "SELECT * FROM " + table;

            var singleResult = false;

            var parameters = new List<SqlParameter>();

            if (arguments.Length > 2)
            {
                query += " WHERE ";

                var argument = arguments[2];
                if (argument is Core.Javascript.Number)
                {
                    query += "id = @id";
                    parameters.Add(new SqlParameter("id", (int) ((Core.Javascript.Number) argument).Value));
                    singleResult = true;
                }
                else if (argument is Core.Javascript.Object)
                {
                    var obj = (Core.Javascript.Object) argument;
                    foreach (var key in obj.GetKeys())
                    {
                        var property = obj.Get(key);

                        if (property is Core.Javascript.String)
                        {
                            if (parameters.Count > 0) query += " AND";
                            query += key + " = @" + key;
                            parameters.Add(new SqlParameter(key, ((Core.Javascript.String) property).Value));
                        }
                    }
                }
                else
                {
                    throw new Exception("Invalid argument type for DB.get");
                }
            }

            var results = Util.SQL.Get(connection, query, parameters.ToArray());
            if (singleResult)
            {
                if (results.Count > 0)
                {
                    return Core.Convert.JsonToObject(results[0], scope);
                }
                else
                {
                    return Static.Undefined;
                }
            }
            else
            {
                // TODO: without convert to List<object>
                return Core.Convert.JsonToValue(results.Select(result => (object) result).ToList(), scope);
            }
        }

        /// <summary>SQL.set takes a connectionName, tablename, id and a object and executes a UPDATE statement and returns a boolean (only true).</summary>
        /// <param name="connectionName">Name of a configured connection</param>
        /// <param name="table">Tablename for UPDATE statement</param>
        /// <param name="id">ID of row for the UPDATE statement</param>
        /// <param name="info">An object with information to be updated to</param>
        /// <returns>A boolean (always true).</returns>
        /// <example><code lang="javascript">var results = SQL.set("NETDB", "users", 1, {name:"NetJS rules!"});</code></example>
        /// <exception cref="InternalError">Thrown when no application can be found in application scope.</exception>
        public static Constant set(Constant _this, Constant[] arguments, Scope scope)
        {
            var connectionName = ((Core.Javascript.String) arguments[0]).Value;

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");
            var connection = application.Connections.GetSqlConnection(connectionName);

            var table = ((Core.Javascript.String) arguments[1]).Value;
            var id = ((Core.Javascript.Number) arguments[2]).Value;
            var obj = ((Core.Javascript.Object) arguments[3]);

            var query = "UPDATE " + table + " SET";

            var parameters = new List<SqlParameter>();

            foreach (var key in obj.GetKeys())
            {
                var property = obj.Get(key);
                if (property is Core.Javascript.String)
                {
                    if (parameters.Count > 0) query += ",";
                    query += " " + key + " = @" + key;
                    parameters.Add(new SqlParameter(key, ((Core.Javascript.String) property).Value));
                }
            }

            query += " WHERE id = @id";
            parameters.Add(new SqlParameter("id", (int) id));

            Util.SQL.Execute(connection, query, parameters.ToArray());

            return new Core.Javascript.Boolean(true);
        }

        /// <summary>SQL.insert takes a connectionName, table and a object which will be inserted into the database. The last inserted id will be returned.</summary>
        /// <param name="connectionName">Name of a configured connection</param>
        /// <param name="table">Tablename for INSERT statement</param>
        /// <param name="info">An object with information to be updated to</param>
        /// <returns>The ID of the last inserted row.</returns>
        /// <example><code lang="javascript">var id = SQL.insert("NETDB", "users", {name:"Hello World!"});</code></example>
        /// <exception cref="InternalError">Thrown when no application can be found in application scope.</exception>
        public static Constant insert(Constant _this, Constant[] arguments, Scope scope)
        {
            var connectionName = ((Core.Javascript.String) arguments[0]).Value;

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");
            var connection = application.Connections.GetSqlConnection(connectionName);

            var table = ((Core.Javascript.String) arguments[1]).Value;
            var obj = ((Core.Javascript.Object) arguments[2]);

            var names = new List<string>();
            var parameters = new List<SqlParameter>();

            foreach (var key in obj.GetKeys())
            {
                var property = obj.Get(key);
                if (property is Core.Javascript.String)
                {
                    names.Add(key);
                    parameters.Add(new SqlParameter(key, ((Core.Javascript.String) property).Value));
                }
            }

            return new Core.Javascript.Number(Util.SQL.Insert(connection, table, names.ToArray(),
                parameters.ToArray()));
        }

        /// <summary>SQL.execute takes a connectionName and a query, executes the query and returns the result if the query is a SELECT statement.</summary>
        /// <param name="connectionName">Name of a configured connection</param>
        /// <param name="query">The query to be executed</param>
        /// <returns>the result if the query is a SELECT statement.</returns>
        /// <example><code lang="javascript">var id = SQL.execute("NETDB", "SELECT * FROM users;");</code></example>
        /// <exception cref="InternalError">Thrown when no application can be found in application scope.</exception>
        /// <exception cref="Error">Thrown when an error has been found while executing the query.</exception>
        public static Constant execute(Constant _this, Constant[] arguments, Scope scope)
        {
            var connectionName = ((Core.Javascript.String) arguments[0]).Value;

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");
            var connection = application.Connections.GetSqlConnection(connectionName);

            var query = ((Core.Javascript.String) arguments[1]).Value;

            try
            {
                if (query.Trim().ToUpper().StartsWith("SELECT"))
                {
                    var rows = Util.SQL.Get(connection, query, new SqlParameter[] { });

                    var result = new Core.Javascript.Array();

                    foreach (var row in rows)
                    {
                        var rowObject = Core.Tool.Construct("Object", scope);
                        foreach (var key in row.Keys)
                        {
                            var value = row[key];
                            if (value is string s)
                            {
                                rowObject.Set(key, new Core.Javascript.String(s));
                            }
                            else if (value is int i)
                            {
                                rowObject.Set(key, new Core.Javascript.Number(i));
                            }
                            else if (value is double d)
                            {
                                rowObject.Set(key, new Core.Javascript.Number(d));
                            }
                            else if (value is DateTime date)
                            {
                                rowObject.Set(key, new Core.Javascript.Date(date));
                            }
                            else if (value is DBNull)
                            {
                                // TODO: make this just Javascript.Null
                                rowObject.Set(key, Core.Javascript.Static.Null);
                            }
                            else
                            {
                                rowObject.Set(key, new Core.Javascript.String("Unkown type - " + value.GetType()));
                            }
                        }

                        result.List.Add(rowObject);
                    }

                    return result;
                }
                else
                {
                    Util.SQL.Execute(connection, query);
                    return Static.Undefined;
                }
            }
            catch (Exception e)
            {
                throw new Error($"SQL error in query: '{query}'\n{e.Message}");
            }
        }
    }
}