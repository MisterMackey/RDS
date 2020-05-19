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
        #endregion
    }
}
