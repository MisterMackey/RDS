using System;
using System.Collections.Generic;
using System.Text;

namespace RelationalSubsettingLib.Properties
{
    public static class Settings
    {
        private static string _DataSourceFileExtension = ".rdsdsf";
        public static string DataSourceFileExtension { get => _DataSourceFileExtension; }
    }
}
