using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

namespace RelationalSubsettingLib.Sources
{
    public class SourceTableInfo : DataSourceInformation
    {
        public string ConnectionString;
        public string ServerName;
        public string DatabaseName;
        public string SchemaName;
        public string TableName;
        public override string[] Columns { get; set; }

        public override string ConcreteType => "SourceTableInfo";

        public override string SourceName => $"{SchemaName}.{TableName}";

        public override string FullyQualifiedName => FQDN();


        public override void LoadToDataTable(DataTable table)
        {
            using (SqlDataAdapter adapter = new SqlDataAdapter($"Select * from {SourceName}", ConnectionString))
            {
                adapter.Fill(table);
            }
        }

        public SourceTableInfo(string connectionString, string schema, string table)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string cmd = "select name from sys.columns where object_id = object_id(@object)";
                using (SqlCommand comm = new SqlCommand(cmd, conn))
                {
                    comm.Parameters.AddWithValue("@object", $"{schema}.{table}");
                    comm.Parameters[0].DbType = DbType.String;
                    comm.Parameters[0].Size = 1000;
                    conn.Open();
                    comm.Prepare();
                    var returnVals = comm.ExecuteReader();
                    List<string> values = new List<string>();
                    while (returnVals.Read())
                    {
                        values.Add(returnVals.GetString(0));
                    }
                    Columns = values.ToArray();
                }
            }
            ExtractServerAndDatabaseFromConnectionString(connectionString);
            SchemaName = schema;
            TableName = table;
            ConnectionString = connectionString;
        }

        private void ExtractServerAndDatabaseFromConnectionString(string connectionString)
        {
            Regex server = new Regex(@"Server=(.+?);", RegexOptions.IgnoreCase);
            Regex data_source = new Regex(@"data source=(.+?);", RegexOptions.IgnoreCase);
            Regex database = new Regex(@"database=(.+?);", RegexOptions.IgnoreCase);
            Regex initial_catalog = new Regex(@"initial catalog=(.+?);", RegexOptions.IgnoreCase);

            if (server.IsMatch(connectionString))
            {
                this.ServerName = server.Match(connectionString).Groups[0].Value;
            }
            else if (data_source.IsMatch(connectionString))
            {
                this.ServerName = data_source.Match(connectionString).Groups[0].Value;
            }

            if (database.IsMatch(connectionString))
            {
                this.DatabaseName = database.Match(connectionString).Groups[0].Value;
            }
            else if (initial_catalog.IsMatch(connectionString))
            {
                this.DatabaseName = initial_catalog.Match(connectionString).Groups[0].Value;
            }
        }

        private string FQDN()
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            ret.Add("connectionstring", ConnectionString);
            ret.Add("objectname", $"{SchemaName}.{TableName}");
            string json = JsonConvert.SerializeObject(ret);
            return json;
        }


    }
}
