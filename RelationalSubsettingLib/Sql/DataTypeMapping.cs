﻿using RelationalSubsettingLib.Properties;
using System;
using System.Collections.Generic;
using System.Text;

namespace RelationalSubsettingLib.Sql
{
    public static class DataTypeMapping
    {
        /// <summary>
        /// Maps from a dotnet type to a sql type (in string form)
        /// </summary>
        public static Dictionary<Type, string> SystemToSql = new Dictionary<Type, string>()
        {
            {typeof(Boolean), "BIT" },
            {typeof(Byte), "BYTE" },
            {typeof(Char), "CHAR(1)" },
            {typeof(DateTime), "DATETIME" },
            {typeof(Decimal), Settings.SqlDecimalPrecision },
            {typeof(Double), "DOUBLE" },
            {typeof(Guid), "GUID" },
            {typeof(Int32), "INT" },
            {typeof(Int64), "BIGINT" },
            {typeof(String), $"VARCHAR({Settings.PrefferedSqlStringLength.ToString()})" }
        };
    }
}
