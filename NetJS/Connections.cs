using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Util;

namespace NetJS {

    public abstract class Connection {

    }

    public class SQLConnection : Connection {
        public SqlConnection Connection;

        public SQLConnection(string connectionString) {
            Connection = SQL.Open(connectionString);
        }
    }

    public class HTTPConnection : Connection {
        public string Url;

        public HTTPConnection(string url) {
            Url = url;
        }
    }

    public class Connections {

        private Dictionary<string, Connection> _connections = new Dictionary<string, Connection>();

        public Connections(Settings settings) {
            var file = settings.Root + settings.Connections;

            Load(file);
        }

        public void Load(string file) {
            lock (_connections) {
                foreach (var key in _connections.Keys) {
                    if (_connections[key] is SQLConnection) {
                        var sqlConnection = (SQLConnection)_connections[key];

                        if (sqlConnection != null) {
                            try {
                                sqlConnection.Connection.Close();
                                sqlConnection.Connection.Dispose();
                            } catch (Exception) {
                                // TODO: log error
                            }
                        }
                    }
                }
                _connections.Clear();

                if (System.IO.File.Exists(file)) {
                    var content = System.IO.File.ReadAllText(file);

                    try {
                        var json = new Json(content);
                        foreach (var key in json.Keys) {
                            var connectionJson = json.Object(key);
                            var type = connectionJson.String("type").ToLower();

                            if (type == "sql") {
                                _connections[key] = new SQLConnection(connectionJson.String("connectionString"));
                            } else if (type == "http") {
                                _connections[key] = new HTTPConnection(connectionJson.String("url"));
                            }
                        }
                    }catch {
                        // TODO: log error
                    }
                } else {

                }
            }
        }

        public SqlConnection GetSqlConnection(string name) {
            lock (_connections) {
                if (_connections.ContainsKey(name)) {
                    var connection = _connections[name];
                    if (connection is SQLConnection sqlConnection) {
                        if(sqlConnection.Connection.State != System.Data.ConnectionState.Open) {
                            try {
                                sqlConnection.Connection.Open();
                            } catch {
                                // TODO: log error
                            }
                        }
                        return sqlConnection.Connection;
                    }
                }
            }

            // TODO: throw exception?
            return null;
        }

        public string GetHttpUrl(string name) {
            lock (_connections) {
                if (_connections.ContainsKey(name)) {
                    var connection = _connections[name];
                    if (connection is HTTPConnection httpConnection) {
                        var url = httpConnection.Url;
                        return url;
                    }
                }
            }

            // TODO: throw exception?
            return "";
        }
    }
}