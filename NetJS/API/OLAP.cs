using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AnalysisServices.AdomdClient;
using Newtonsoft.Json;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Security;

namespace NetJS.API {
    public class OLAP {

        /// <summary>OLAP.execute takes a connectionName and a query, executes the query and returns the result of the MDX query in JSON format.</summary>
        /// <param name="connectionName">Name of a configured connection</param>
        /// <param name="query">The query to be executed</param>
        /// <returns>the result of the MDX query in JSON format.</returns>
        /// <example><code lang="javascript">var graph = OLAP.execute("olap", "SELECT [Measures].[Page View Count] on columns, [Page].[Page].Children on rows FROM [Model];");</code></example>
        public static dynamic execute(string connectionName, string query) {

            var application = State.Application;
            var state = State.Get();

            return Tool.CreatePromise((resolve, reject) => {
                IntPtr userToken = IntPtr.Zero;

                try {
                    string connectionString = application.Connections.GetOLAPConnection(connectionName);
                    var connectionStringSplit = connectionString.Split(';');
                    string password = null;
                    string domain = null;
                    string username = null;
                    foreach (string s in connectionStringSplit) {
                        var splitCheck = s.Split('=');
                        if (splitCheck[0].Equals("Password")) {
                            password = splitCheck[1];
                        }
                        else if (splitCheck[0].Equals("User ID")) {
                            var usernameSplit = splitCheck[1].Split('\\');
                            if (usernameSplit.Length > 1) {
                                domain = usernameSplit[0];
                                username = usernameSplit[1];
                            }
                            else if (usernameSplit.Length == 1) {
                                username = usernameSplit[0];
                            }
                        }
                    }

                    bool succes = LogonUser(username, domain, password, 2, 0, out userToken);

                    if (!succes) {
                        throw new SecurityException("Logon user failed");
                    }

                    using (WindowsIdentity.Impersonate(userToken)) {

                        List<string> curColumn = new List<string>();
                        StringBuilder sb = new StringBuilder();
                        StringWriter sw = new StringWriter(sb);
                        string columnName = string.Empty;
                        string fieldVal = string.Empty;
                        string prevFieldVal = string.Empty;

                        using (AdomdConnection connection = new AdomdConnection(connectionString)) {

                            connection.Open();

                            using (AdomdCommand command = new AdomdCommand(query, connection)) {
                                AdomdDataReader rdr = command.ExecuteReader();
                                if (rdr != null) {
                                    JsonWriter myJson = new JsonTextWriter(sw);
                                    myJson.WriteStartArray();
                                    while (rdr.Read()) {
                                        myJson.WriteStartObject();
                                        int fields = rdr.FieldCount;
                                        for (int i = 0; i < fields; i++) {
                                            if (rdr[i] != null) {
                                                fieldVal = rdr[i].ToString();
                                                if (i != 0 && rdr[i - 1] != null) {
                                                    prevFieldVal = rdr[i - 1].ToString();
                                                }
                                                else {
                                                    prevFieldVal = "First";
                                                }
                                                if ((fieldVal == null || fieldVal.ToLower().Trim() == "undefined" || fieldVal.ToLower().Trim() == "unknown") && (prevFieldVal == null || prevFieldVal.ToLower().Trim() == "undefined" || prevFieldVal.ToLower().Trim() == "unknown")) {
                                                    continue;
                                                }
                                                else {
                                                    columnName = rdr.GetName(i).Replace(".[MEMBER_CAPTION]", "").Trim();
                                                    curColumn = columnName.Split(new string[] { "." }, StringSplitOptions.None).ToList();
                                                    columnName = curColumn[curColumn.Count - 1].Replace("[", "").Replace("]", "");
                                                    myJson.WritePropertyName(columnName);
                                                    myJson.WriteValue(rdr[i]);
                                                }
                                            }
                                        }
                                        myJson.WriteEndObject();
                                    }
                                    myJson.WriteEndArray();
                                }
                                else {
                                    throw new ArgumentException("The query result is empty");
                                }
                            }
                            connection.Close();
                        }

                        if (sb.Length < 3) {
                            throw new ArgumentException("The query result is empty");
                        }

                        var json = sw.ToString();
                        application.AddCallback(resolve, json, state);

                    }
                }
                catch (Exception e) {
                    application.AddCallback(reject, $"{e.Message}", state);
                }
            });
        }

        /// <summary>OLAP.getDimensionsByFact takes a connectionName and a fact, returns a list of all available dimensions for a selected fact on the cubes that are visible.</summary>
        /// <param name="connectionName">Name of a configured connection</param>
        /// <param name="fact">The selected fact</param>
        /// <returns>the list in JSON format of all available dimensions for the selected fact on the cubes that are visible.</returns>
        /// <example><code lang="javascript">var list = OLAP.getDimensionsByFact("olap", "PageView");</code></example>
        public static dynamic getDimensionsByFact(string connectionName, string fact) {
            var application = State.Application;
            var state = State.Get();

            return Tool.CreatePromise((resolve, reject) => {
                IntPtr userToken = IntPtr.Zero;

                try {
                    string connectionString = application.Connections.GetOLAPConnection(connectionName);
                    var connectionStringSplit = connectionString.Split(';');
                    string password = null;
                    string domain = null;
                    string username = null;
                    foreach (string s in connectionStringSplit) {
                        var splitCheck = s.Split('=');
                        if (splitCheck[0].Equals("Password")) {
                            password = splitCheck[1];
                        }
                        else if (splitCheck[0].Equals("User ID")) {
                            var usernameSplit = splitCheck[1].Split('\\');
                            if (usernameSplit.Length > 1) {
                                domain = usernameSplit[0];
                                username = usernameSplit[1];
                            }
                            else if (usernameSplit.Length == 1) {
                                username = usernameSplit[0];
                            }
                        }
                    }

                    bool succes = LogonUser("SSASDev", "DEVWOA01", "Ch3ssM@ster", 2, 0, out userToken);

                    if (!succes) {
                        throw new SecurityException("Logon user failed");
                    }

                    using (WindowsIdentity.Impersonate(userToken)) {

                        List<string> curColumn = new List<string>();
                        StringBuilder sb = new StringBuilder();
                        StringWriter sw = new StringWriter(sb);
                        string columnName = string.Empty;
                        string fieldVal = string.Empty;
                        string prevFieldVal = string.Empty;
                        string query = "SELECT[DIMENSION_UNIQUE_NAME] FROM $system.MDSchema_measuregroup_dimensions WHERE[MEASUREGROUP_NAME] = '" + fact + "'";

                        using (AdomdConnection connection = new AdomdConnection(connectionString)) {

                            connection.Open();

                            using (AdomdCommand command = new AdomdCommand(query, connection)) {
                                AdomdDataReader rdr = command.ExecuteReader();
                                if (rdr != null) {
                                    JsonWriter myJson = new JsonTextWriter(sw);
                                    myJson.WriteStartArray();
                                    while (rdr.Read()) {
                                        myJson.WriteStartObject();
                                        int fields = rdr.FieldCount;
                                        for (int i = 0; i < fields; i++) {
                                            if (rdr[i] != null) {
                                                fieldVal = rdr[i].ToString();
                                                if (i != 0 && rdr[i - 1] != null) {
                                                    prevFieldVal = rdr[i - 1].ToString();
                                                }
                                                else {
                                                    prevFieldVal = "First";
                                                }
                                                if ((fieldVal == null || fieldVal.ToLower().Trim() == "undefined" || fieldVal.ToLower().Trim() == "unknown") && (prevFieldVal == null || prevFieldVal.ToLower().Trim() == "undefined" || prevFieldVal.ToLower().Trim() == "unknown")) {
                                                    continue;
                                                }
                                                else {
                                                    columnName = "name";
                                                    myJson.WritePropertyName(columnName);
                                                    myJson.WriteValue(rdr[i].ToString().Replace("[", "").Replace("]", ""));
                                                }
                                            }
                                        }
                                        myJson.WriteEndObject();
                                    }
                                    myJson.WriteEndArray();
                                }
                                else {
                                    throw new ArgumentException("Could not find any dimensions for this fact");
                                }
                            }
                            connection.Close();
                        }

                        if (sb.Length < 3) {
                            throw new ArgumentException("Could not find any dimensions for this fact");
                        }

                        var json = sw.ToString();
                        application.AddCallback(resolve, json, state);
                    }
                }
                catch (Exception e) {
                    application.AddCallback(reject, $"{e.Message}", state);
                }
            });
        }

        /// <summary>OLAP.getMeasuresByFact takes a connectionName and a fact, returns a list of all available measures of a selected fact on the cubes that are visible.</summary>
        /// <param name="connectionName">Name of a configured connection</param>
        /// <param name="fact">The selected fact</param>
        /// <returns>the list in JSON format of all available measures of the selected fact on the cubes that are visible.</returns>
        /// <example><code lang="javascript">var list = OLAP.getMeasuresByFact("olap", "PageView");</code></example>
        public static dynamic getMeasuresByFact(string connectionName, string fact) {
            var application = State.Application;
            var state = State.Get();

            List<string> measures = new List<string>();

            return Tool.CreatePromise((resolve, reject) => {
                IntPtr userToken = IntPtr.Zero;

                try {
                    string connectionString = application.Connections.GetOLAPConnection(connectionName);
                    var connectionStringSplit = connectionString.Split(';');
                    string password = null;
                    string domain = null;
                    string username = null;
                    foreach (string s in connectionStringSplit) {
                        var splitCheck = s.Split('=');
                        if (splitCheck[0].Equals("Password")) {
                            password = splitCheck[1];
                        }
                        else if (splitCheck[0].Equals("User ID")) {
                            var usernameSplit = splitCheck[1].Split('\\');
                            if (usernameSplit.Length > 1) {
                                domain = usernameSplit[0];
                                username = usernameSplit[1];
                            }
                            else if (usernameSplit.Length == 1) {
                                username = usernameSplit[0];
                            }
                        }
                    }

                    bool succes = LogonUser("SSASDev", "DEVWOA01", "Ch3ssM@ster", 2, 0, out userToken);

                    if (!succes) {
                        throw new SecurityException("Logon user failed");
                    }

                    using (WindowsIdentity.Impersonate(userToken)) {
                        using (AdomdConnection connection = new AdomdConnection(connectionString)) {
                            connection.Open();
                            foreach (CubeDef cube in connection.Cubes) {

                                if (cube.Name.StartsWith("$")) {
                                    continue;
                                }

                                foreach (Measure measure in cube.Measures) {
                                    if (fact.Equals(measure.Properties["MEASUREGROUP_NAME"].Value.ToString())) {
                                        if (!measures.Contains(measure.Name.ToString())) {
                                            measures.Add(measure.Name.ToString());
                                        }
                                    }
                                }
                            }
                            connection.Close();
                        }
                        var result = new List<Dictionary<string, string>>();
                        foreach (string s in measures) {
                            var row = new Dictionary<string, string>();
                            row.Add("name", s);
                            result.Add(row);
                        }

                        if (!result.Any()) {
                            throw new ArgumentException("Could not find any measures for this fact");
                        }

                        var json = JsonConvert.SerializeObject(result);
                        application.AddCallback(resolve, json, state);
                    }
                }
                catch (Exception e) {
                    application.AddCallback(reject, $"{e.Message}", state);
                }
            });
        }

        /// <summary>OLAP.getFacts takes a connectionName and returns a list of all available facts/measure_groups of the cubes that are visible.</summary>
        /// <param name="connectionName">Name of a configured connection</param>
        /// <returns>the list in JSON format of all available facts/measure_groups of the cubes that are visible.</returns>
        /// <example><code lang="javascript">var list = OLAP.getFacts("olap");</code></example>
        public static dynamic getFacts(string connectionName) {
            var application = State.Application;
            var state = State.Get();

            var facts = new List<string>();

            return Tool.CreatePromise((resolve, reject) => {
                IntPtr userToken = IntPtr.Zero;

                try {
                    string connectionString = application.Connections.GetOLAPConnection(connectionName);
                    var connectionStringSplit = connectionString.Split(';');
                    string password = null;
                    string domain = null;
                    string username = null;
                    foreach (string s in connectionStringSplit) {
                        var splitCheck = s.Split('=');
                        if (splitCheck[0].Equals("Password")) {
                            password = splitCheck[1];
                        }
                        else if (splitCheck[0].Equals("User ID")) {
                            var usernameSplit = splitCheck[1].Split('\\');
                            if (usernameSplit.Length > 1) {
                                domain = usernameSplit[0];
                                username = usernameSplit[1];
                            }
                            else if (usernameSplit.Length == 1) {
                                username = usernameSplit[0];
                            }
                        }
                    }

                    bool succes = LogonUser("SSASDev", "DEVWOA01", "Ch3ssM@ster", 2, 0, out userToken);

                    if (!succes) {
                        throw new SecurityException("Logon user failed");
                    }

                    using (WindowsIdentity.Impersonate(userToken)) {
                        using (AdomdConnection connection = new AdomdConnection(connectionString)) {
                            connection.Open();
                            foreach (CubeDef cube in connection.Cubes) {

                                if (cube.Name.StartsWith("$")) {
                                    continue;
                                }

                                foreach (Measure measure in cube.Measures) {
                                    string Name = measure.Properties["MEASUREGROUP_NAME"].Value.ToString();
                                    if (!facts.Contains(Name)) {
                                        facts.Add(Name);
                                    }
                                }
                            }
                            connection.Close();
                        }

                        var result = new List<Dictionary<string, string>>();
                        foreach (string s in facts) {
                            var row = new Dictionary<string, string>();
                            row.Add("name", s);
                            result.Add(row);
                        }

                        if (!result.Any()) {
                            throw new ArgumentException("Could not find any facts on the cube");
                        }

                        var json = JsonConvert.SerializeObject(result);
                        application.AddCallback(resolve, json, state);
                    }
                }
                catch (Exception e) {
                    application.AddCallback(reject, $"{e.Message}", state);
                }
            });
        }

        /// <summary>OLAP.getHierarchiesByDimension takes a connectionName and a dimension, returns a list of all available hierarchies of a selected dimension on the cubes that are visible.</summary>
        /// <param name="connectionName">Name of a configured connection</param>
        /// <param name="dimension">The selected dimension</param>
        /// <returns>the list in JSON format of all available hierarchies of the selected dimension on the cubes that are visible.</returns>
        /// <example><code lang="javascript">var list = OLAP.getHierarchiesByDimension("olap", "StartDate");</code></example>
        public static dynamic getHierarchiesByDimension(string connectionName, string dimension) {
            var application = State.Application;
            var state = State.Get();

            return Tool.CreatePromise((resolve, reject) => {
                IntPtr userToken = IntPtr.Zero;

                try {
                    string connectionString = application.Connections.GetOLAPConnection(connectionName);
                    var connectionStringSplit = connectionString.Split(';');
                    string password = null;
                    string domain = null;
                    string username = null;
                    foreach (string s in connectionStringSplit) {
                        var splitCheck = s.Split('=');
                        if (splitCheck[0].Equals("Password")) {
                            password = splitCheck[1];
                        }
                        else if (splitCheck[0].Equals("User ID")) {
                            var usernameSplit = splitCheck[1].Split('\\');
                            if (usernameSplit.Length > 1) {
                                domain = usernameSplit[0];
                                username = usernameSplit[1];
                            }
                            else if (usernameSplit.Length == 1) {
                                username = usernameSplit[0];
                            }
                        }
                    }

                    bool succes = LogonUser("SSASDev", "DEVWOA01", "Ch3ssM@ster", 2, 0, out userToken);

                    if (!succes) {
                        throw new SecurityException("Logon user failed");
                    }

                    using (WindowsIdentity.Impersonate(userToken)) {

                        List<string> curColumn = new List<string>();
                        StringBuilder sb = new StringBuilder();
                        StringWriter sw = new StringWriter(sb);
                        string columnName = string.Empty;
                        string fieldVal = string.Empty;
                        string prevFieldVal = string.Empty;
                        string query = "SELECT [HIERARCHY_NAME] FROM $system.MDSchema_hierarchies WHERE [DIMENSION_UNIQUE_NAME] = '[" + dimension + "]' AND [CUBE_NAME] = 'Model'";

                        using (AdomdConnection connection = new AdomdConnection(connectionString)) {

                            connection.Open();

                            using (AdomdCommand command = new AdomdCommand(query, connection)) {
                                AdomdDataReader rdr = command.ExecuteReader();
                                if (rdr != null) {
                                    JsonWriter myJson = new JsonTextWriter(sw);
                                    myJson.WriteStartArray();
                                    while (rdr.Read()) {
                                        myJson.WriteStartObject();
                                        int fields = rdr.FieldCount;
                                        for (int i = 0; i < fields; i++) {
                                            if (rdr[i] != null) {
                                                fieldVal = rdr[i].ToString();
                                                if (i != 0 && rdr[i - 1] != null) {
                                                    prevFieldVal = rdr[i - 1].ToString();
                                                }
                                                else {
                                                    prevFieldVal = "First";
                                                }
                                                if ((fieldVal == null || fieldVal.ToLower().Trim() == "undefined" || fieldVal.ToLower().Trim() == "unknown") && (prevFieldVal == null || prevFieldVal.ToLower().Trim() == "undefined" || prevFieldVal.ToLower().Trim() == "unknown")) {
                                                    continue;
                                                }
                                                else {
                                                    columnName = "name";
                                                    myJson.WritePropertyName(columnName);
                                                    myJson.WriteValue(rdr[i]);
                                                }
                                            }
                                        }
                                        myJson.WriteEndObject();
                                    }
                                    myJson.WriteEndArray();
                                }
                                else {
                                    throw new ArgumentException("Could not find any hierarchies for this dimension");
                                }
                            }
                            connection.Close();
                        }

                        if (sb.Length < 3) {
                            throw new ArgumentException("Could not find any hierarchies for this dimension");
                        }

                        var json = sw.ToString();
                        application.AddCallback(resolve, json, state);

                    }
                }
                catch (Exception e) {
                    application.AddCallback(reject, $"{e.Message}", state);
                }
            });
        }

        /// <summary>OLAP.getMembers takes a connectionName, a dimension and an hierarchy, returns a list of all available members of a selected dimension.hierarchy on the cubes that are visible.</summary>
        /// <param name="connectionName">Name of a configured connection</param>
        /// <param name="dimension">The selected dimension</param>
        /// <param name="hierarchy">The selected hierarchy</param>
        /// <returns>the list in JSON format of all available members of the selected dimension.hierarchy on the cubes that are visible.</returns>
        /// <example><code lang="javascript">var list = OLAP.getMembers("olap", "StartDate", "Month");</code></example>
        public static dynamic getMembers(string connectionName, string dimension, string hierarchy) {
            var application = State.Application;
            var state = State.Get();

            List<string> members = new List<string>();
            DataSet ds = new DataSet();

            return Tool.CreatePromise((resolve, reject) => {
                IntPtr userToken = IntPtr.Zero;

                try {
                    string connectionString = application.Connections.GetOLAPConnection(connectionName);
                    var connectionStringSplit = connectionString.Split(';');
                    string password = null;
                    string domain = null;
                    string username = null;
                    foreach (string s in connectionStringSplit) {
                        var splitCheck = s.Split('=');
                        if (splitCheck[0].Equals("Password")) {
                            password = splitCheck[1];
                        }
                        else if (splitCheck[0].Equals("User ID")) {
                            var usernameSplit = splitCheck[1].Split('\\');
                            if (usernameSplit.Length > 1) {
                                domain = usernameSplit[0];
                                username = usernameSplit[1];
                            }
                            else if (usernameSplit.Length == 1) {
                                username = usernameSplit[0];
                            }
                        }
                    }

                    bool succes = LogonUser("SSASDev", "DEVWOA01", "Ch3ssM@ster", 2, 0, out userToken);

                    if (!succes) {
                        throw new SecurityException("Logon user failed");
                    }

                    using (WindowsIdentity.Impersonate(userToken)) {

                        using (AdomdConnection connection = new AdomdConnection(connectionString)) {
                            connection.Open();
                            AdomdRestrictionCollection restrColl = new AdomdRestrictionCollection();
                            restrColl.Add("CATALOG_NAME", "Iris-Ana-Ontwikkel");
                            restrColl.Add("CUBE_NAME", "Model");
                            restrColl.Add("HIERARCHY_UNIQUE_NAME", "[" + dimension + "].[" + hierarchy + "]");
                            ds = connection.GetSchemaDataSet("MDSCHEMA_MEMBERS", restrColl);

                            connection.Close();
                        }

                        var result = new List<Dictionary<string, string>>();
                        foreach (DataRow row in ds.Tables[0].Rows) {
                            var memberName = row["MEMBER_NAME"].ToString();
                            if (memberName != "All" && memberName != "Unknown") {
                                var dict = new Dictionary<string, string>();
                                dict.Add("name", memberName);
                                result.Add(dict);
                            }
                        }

                        if (!result.Any()) {
                            throw new ArgumentException("Could not find any members for this dimension/hierarchy");
                        }

                        var json = JsonConvert.SerializeObject(result);
                        application.AddCallback(resolve, json, state);

                    }
                }
                catch (Exception e) {
                    application.AddCallback(reject, $"{e.Message}", state);
                }
            });
        }

        // logon method voor impersonation windows authentication
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);
    }
}
