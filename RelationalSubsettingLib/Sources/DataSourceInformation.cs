﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace RelationalSubsettingLib
{
    [JsonConverter(typeof(DataSourceInformationJsonConverter))]
    public abstract class DataSourceInformation
    {
        #region Public Properties

        public abstract string[] Columns { get; set; }

        public abstract string ConcreteType { get; }

        /// <summary>
        /// for tables: connectionstring and schema.table as a json dict (string form) for files: full path
        /// </summary>
        public abstract string FullyQualifiedName { get; }

        public abstract Dictionary<string, Tuple<MaskingOptions, string>> MaskingInformation { get; }

        /// <summary>
        /// The name of the file for files, name of the schema.table for tables
        /// </summary>
        public abstract string SourceName { get; }

        #endregion Public Properties

        #region Public Methods

        //required as the static extension requires an initialized object, which is one thing an abstract class won't give you...
        public static DataSourceInformation LoadFromFile(string filePath)
        {
            string txt = File.ReadAllText(filePath);
            DataSourceInformation ret = JsonConvert.DeserializeObject<DataSourceInformation>(txt);
            return ret;
        }

        public abstract void LoadToDataTable(DataTable table);

        #endregion Public Methods
    }
}