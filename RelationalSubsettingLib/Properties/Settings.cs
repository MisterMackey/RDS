using System;
using System.Collections.Generic;
using System.Text;

namespace RelationalSubsettingLib.Properties
{
    public static class Settings
    {
        #region Private Backing Fields
        private static string _DataSourceFileExtension = ".rdsdsf";
        private static string _KeyRelationshipFileName = "keyrelations.rdskrf";
        private static string _RdsDirectoryName = ".rds";
        private static string _RdsSubsettingSettingsFileName = "subsetsettings.rdssf";
        private static string _SqlDecimalPrecision = "DECIMAL(38,10)";
        private static int _PreferredSqlStringLength = 500;
        private static string _ConnectionAliasFileName = "aliases.rdsaf";
        private static string _DefaultReplacementValueForReplaceStrategy = "";
        #endregion

        #region Public properties
        /// <summary>
        /// the extension for files that contain information about data sources (file based or otherwise)
        /// </summary>
        public static string DataSourceFileExtension { get => _DataSourceFileExtension; }
        /// <summary>
        /// the name of the file holding the relationships between sources
        /// </summary>
        public static string KeyRelationshipFileName { get => _KeyRelationshipFileName; }
        /// <summary>
        /// name of the subdirectory in which the configuration files sit
        /// </summary>
        public static string RdsDirectoryName { get => _RdsDirectoryName; }
        /// <summary>
        /// name of the file containing settings for the subsetting function
        /// </summary>
        public static string RdsSubsettingSettingsFileName { get => _RdsSubsettingSettingsFileName; }
        /// <summary>
        /// Precision of the sql decimal datatype used when a column has this datatype. in the form of DECIMAL(p,s)
        /// </summary>
        public static string SqlDecimalPrecision { get => _SqlDecimalPrecision; }
        /// <summary>
        /// the preferred length of Varchar columns in sql when a table is created
        /// </summary>
        public static int PrefferedSqlStringLength { get => _PreferredSqlStringLength; }
        /// <summary>
        /// The name of the file holding a json formatted dictionary containing connection strings. Keyed by user defined Aliases.
        /// </summary>
        public static string ConnectionAliasFileName { get => _ConnectionAliasFileName; }

        public static string DefaultReplacementValueForReplaceStrategy { get => _DefaultReplacementValueForReplaceStrategy; }
        #endregion
    }
}
