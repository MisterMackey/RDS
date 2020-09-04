using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace RelationalSubsettingLib.Sources
{
    public class SourceTableInfo : DataSourceInformation
    {
        #region Public Properties

        public override string[] Columns { get; set; }
        public override string ConcreteType => "SourceTableInfo";
        public override string FullyQualifiedName => FQDN();
        public override Dictionary<string, Tuple<MaskingOptions, string>> MaskingInformation { get; }
        public override string SourceName => $"{SchemaName}.{TableName}";

        #endregion Public Properties

        #region Public Fields

        public string ConnectionString;
        public string DatabaseName;
        public string SchemaName;
        public string ServerName;
        public string TableName;

        #endregion Public Fields

        #region Public Constructors

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
            MaskingInformation = new Dictionary<string, Tuple<MaskingOptions, string>>();
        }

        #endregion Public Constructors

        #region Public Methods

        public override void LoadToDataTable(DataTable table)
        {
            using (SqlDataAdapter adapter = new SqlDataAdapter($"Select * from {SourceName}", ConnectionString))
            {
                adapter.Fill(table);
            }
        }

        #endregion Public Methods

        #region Private Methods

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

        #endregion Private Methods
    }
}