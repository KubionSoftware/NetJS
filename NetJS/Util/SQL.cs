using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Util;

namespace NetJS.Util {
    public static class SQL {

        public static SqlConnection Open(string connectionString) {
            try {
                var connection = new SqlConnection(connectionString);
                connection.Open();
                return connection;
            } catch (Exception e) {
                // TODO: throw exception?
                return null;
            }
        }

        public static void Execute(SqlConnection connection, string query, SqlParameter[] parameters = null) {
            var command = new SqlCommand(query, connection);
            if (parameters != null) {
                command.Parameters.AddRange(parameters);
            }
            command.ExecuteNonQuery();
        }

        public static SqlTransaction BeginTransation(SqlConnection connection) {
            return connection.BeginTransaction();
        }

        public static int Insert(SqlConnection connection, string table, string[] names, SqlParameter[] parameters) {
            var query = $"INSERT INTO {table} ({string.Join(", ", names)}) VALUES ({string.Join(", ", parameters.Select(param => "@" + param.ParameterName))})";
            Execute(connection, query, parameters);
            // TODO: return last inserted id
            return -1;
        }

        public static IList<Dictionary<string, object>> Get(SqlConnection connection, string query, SqlParameter[] parameters) {
            using (var dictSelectCommand = new SqlCommand(query, connection)) {
                foreach (var parameter in parameters) {
                    dictSelectCommand.Parameters.Add(parameter);
                }

                using (var reader = dictSelectCommand.ExecuteReader()) {
                    var list = new List<Dictionary<string, object>>();
                    while (reader.Read()) {
                        var data = new Dictionary<string, object>();
                        for (var i = 0; i < reader.FieldCount; i++) {
                            data[reader.GetName(i)] = reader.GetValue(i);
                        }
                        list.Add(data);
                    }
                    return list;
                }
            }
        }

        public static int ParseId(object id) {
            return (int)((long)id);
        }

        public static string Escape(string value) {
            return value.Replace("'", "''");
        }
    }
}