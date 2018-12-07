using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Util;
using Microsoft.AnalysisServices.AdomdClient;

namespace NetJS {

    public abstract class Connection {

    }

    public class OLAPConnection : Connection {
        public String Connection;

        public OLAPConnection(string connectionString) {
            Connection = connectionString;
        }
    }

    public class MongoDBConnection : Connection {
        public MongoClient Client;
        public IMongoDatabase Database;

        public MongoDBConnection (string connectionString, string database) {
            Client = new MongoClient(connectionString);
            Database = Client.GetDatabase(database);
        }
    }

    public class SQLConnection : Connection {
        public SqlConnection Connection;

        public SQLConnection(string connectionString) {
            Connection = Util.SQL.Open(connectionString);
        }
    }

    public class HTTPConnection : Connection {
        public string Url;

        public HTTPConnection(string url) {
            if (!url.EndsWith("/")) {
                url += "/";
            }

            Url = url;
        }
    }

    public class Connections {

        private Dictionary<string, Connection> _connections = new Dictionary<string, Connection>();
        private DateTime _lastChange;
        private string _file;

        public Connections(Settings settings) {
            _file = settings.Root + settings.Connections;
            _lastChange = DateTime.Now;

            Load(_file);
        }

        public void CheckForChanges() {
            var lastWrite = System.IO.File.GetLastWriteTime(_file);
            if (lastWrite > _lastChange) {
                Load(_file);
                _lastChange = lastWrite;
            }
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
                            } catch (Exception e) {
                                API.Log.write("Error while closing SQL connection - " + e);
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

                            if (type == "sql")
                            {
                                _connections[key] = new SQLConnection(connectionJson.String("connectionString"));
                            }
                            else if (type == "http")
                            {
                                _connections[key] = new HTTPConnection(connectionJson.String("url"));
                            }
                            else if (type == "mongodb")
                            {
                                _connections[key] = new MongoDBConnection(connectionJson.String("connectionString"), connectionJson.String("database"));
                            }
                            else if (type == "olap")
                            {
                                _connections[key] = new OLAPConnection(connectionJson.String("connectionString"));
                            }
                        }
                    }catch (Exception e) {
                        API.Log.write("Error while parsing connection JSON - " + e);
                    }
                } else {

                }
            }
        }

        public string GetOLAPConnection(string name) {
            API.Log.write(_connections[name].ToString());
            lock(_connections) {
                if(_connections.ContainsKey(name)) {
                    var connection = _connections[name];
                    if (connection is OLAPConnection olapConnection) {
                        return olapConnection.Connection;
                    }
                }
            }
            return null;
        }

        public IMongoDatabase GetMongoDBConnection(string name) {
            lock (_connections) {
                if (_connections.ContainsKey(name)) {
                    var connection = _connections[name];
                    if (connection is MongoDBConnection mongoConnection) {
                        return mongoConnection.Database;
                    }
                }
            }

            // TODO: throw exception?
            return null;
        }

        public SqlConnection GetSqlConnection(string name) {
            lock (_connections) {
                if (_connections.ContainsKey(name)) {
                    var connection = _connections[name];
                    if (connection is SQLConnection sqlConnection) {
                        if(sqlConnection.Connection.State != System.Data.ConnectionState.Open) {
                            try {
                                sqlConnection.Connection.Open();
                            } catch (Exception e) {
                                API.Log.write("Error while opening SQL connection - " + e);
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