using CsvHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using D2S.Library.Utilities;
using D2S.Library.Services;
using D2S.Library.Pipelines;
using RelationalSubsettingLib.Sql;

namespace RelationalSubsettingLib
{
    public static class Extensions
    {
        /// <summary>
        /// splits a string, auto-detects delimiters. doesn't use qualifier
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] Delimit(this string str)
        {
            string del = DetectDelimiter(str);
            return SplitRow(str,del,null,true);
        }
        /// <summary>
        /// splits a string based on supplied qualifier and delimiter.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="del"></param>
        /// <param name="qual"></param>
        /// <returns></returns>
        public static string[] Delimit(this string str, string del, string qual)
        {
            
            return SplitRow(str, del, qual, true);
        }

        public static void SaveToFile<T>(this T val, string filePath)
        {
            string txt = JsonConvert.SerializeObject(val, Formatting.Indented);
            File.WriteAllText(filePath, txt);
        }
        public static T LoadFromFile<T>(this T val, string filePath)
        {
            string txt = File.ReadAllText(filePath);
            T ret = JsonConvert.DeserializeObject<T>(txt);
            return ret;
        }

        public static bool NotInList(this string value, string[] list)
        {
            if (list.Any(x => x.Equals(value, StringComparison.InvariantCultureIgnoreCase)))
            {
                return false;
            }
            else return true;
        }

        public static bool InList(this string value, string[] list)
        {
            return !value.NotInList(list);
        }
        /// <summary>
        /// Split strings while preserving possible delimiter characters inside the strings
        /// (this function performace is better than using regular expressions)
        /// <para>
        /// https://stackoverflow.com/questions/3776458/split-a-comma-separated-string-with-both-quoted-and-unquoted-strings
        /// </para>
        /// </summary>
        /// <param name="record"></param>
        /// <param name="delimiter"></param>
        /// <param name="qualifier"></param>
        /// <param name="trimData"></param>
        /// <returns></returns>
        public static string[] SplitRow(string record, string delimiter, string qualifier, bool trimData)
        {
            //call the version that isn't checking for qualifiers (save the anima- i mean CPU cycles!!)
            if (qualifier == null)
            {
                return SplitRow(record, delimiter, trimData);
            }
            // In-Line for example, but I implemented as string extender in production code
            Func<string, int, int> IndexOfNextNonWhiteSpaceChar = delegate (string source, int startIndex)
            {
                if (startIndex >= 0)
                {
                    if (source != null)
                    {
                        for (int i = startIndex; i < source.Length; i++)
                        {
                            if (!char.IsWhiteSpace(source[i]))
                            {
                                return i;
                            }
                        }
                    }
                }

                return -1;
            };

            var results = new List<string>();
            var result = new StringBuilder();
            var inQualifier = false;
            var inField = false;

            // We add new columns at the delimiter, so append one for the parser.
            var row = $"{record}{delimiter}";

            for (var idx = 0; idx < row.Length; idx++)
            {
                // A delimiter character...
                if (row[idx] == delimiter[0])
                {
                    // Are we inside qualifier? If not and we use a single char delimiter we are done with this field. otherwise peek ahead and check if the full delimiter is encountered.
                    if (!inQualifier)
                    {
                        bool delimiterMatch = true;
                        for (int i = 1; i < delimiter.Length; i++) //this loop is not entered for single char delimiters
                        {
                            if ((idx + i) < row.Length && row[idx + i] != delimiter[i]) //the row length check is there to prevent indexoutofrange exceptions when dealing with certain delimiters like ||
                            {
                                //set success to false if we don't encounter the full multichar delimiter)
                                delimiterMatch = false;
                            }
                        }

                        if (delimiterMatch)
                        {
                            //increment the idx variable so we don't read characters that belong to the delimiter into the result
                            idx += (delimiter.Length - 1);
                            results.Add(trimData ? result.ToString().Trim() : result.ToString());
                            result.Clear();
                            inField = false;
                        }
                        else
                        {
                            result.Append(row[idx]);
                        }
                    }
                    else
                    {
                        result.Append(row[idx]);
                    }
                }

                // NOT a delimiter character...
                else
                {
                    // ...Not a space character
                    if (row[idx] != ' ')
                    {
                        // A qualifier character...
                        if (row[idx] == qualifier[0])
                        {
                            // Qualifier is closing qualifier...
                            if (inQualifier && row[IndexOfNextNonWhiteSpaceChar(row, idx + 1)] == delimiter[0])
                            {
                                inQualifier = false;
                                continue;
                            }

                            else
                            {
                                // ...Qualifier is opening qualifier
                                if (!inQualifier)
                                {
                                    inQualifier = true;
                                }

                                // ...It's a qualifier inside a qualifier.
                                else
                                {
                                    inField = true;
                                    result.Append(row[idx]);
                                }
                            }
                        }

                        // Not a qualifier character...
                        else
                        {
                            result.Append(row[idx]);
                            inField = true;
                        }
                    }

                    // ...A space character
                    else
                    {
                        if (inQualifier || inField)
                        {
                            result.Append(row[idx]);
                        }
                    }
                }
            }

            return results.ToArray(); // .ToArray<string>();
        }

        /// <summary>
        /// same as the other splitrow but it doesn't have the qualifier checking bit. Called from the other one when quali is set to null in the args.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="delimiter"></param>
        /// <param name="trimData"></param>
        /// <returns></returns>
        private static string[] SplitRow(string record, string delimiter, bool trimData)
        {

            var results = new List<string>();
            var result = new StringBuilder();
            var inField = false;

            // We add new columns at the delimiter, so append one for the parser.
            var row = $"{record}{delimiter}";

            for (var idx = 0; idx < row.Length; idx++)
            {
                // A delimiter character...
                if (row[idx] == delimiter[0])
                {
                    bool delimiterMatch = true;
                    for (int i = 1; i < delimiter.Length; i++) //this loop is not entered for single char delimiters
                    {
                        if ((idx + i) < row.Length && row[idx + i] != delimiter[i]) //the row length check is there to prevent indexoutofrange exceptions when dealing with certain delimiters like ||
                        {
                            //set success to false if we don't encounter the full multichar delimiter)
                            delimiterMatch = false;
                        }
                    }

                    if (delimiterMatch)
                    {
                        //increment the idx variable so we don't read characters that belong to the delimiter into the result
                        idx += (delimiter.Length - 1);
                        results.Add(trimData ? result.ToString().Trim() : result.ToString());
                        result.Clear();
                        inField = false;
                    }
                    else
                    {
                        result.Append(row[idx]);
                    }
                }

                // NOT a delimiter character...
                else
                {
                    // ...Not a space character
                    if (row[idx] != ' ')
                    {
                        result.Append(row[idx]);
                        inField = true;
                    }

                    // ...A space character
                    else
                    {
                        if (inField)
                        {
                            result.Append(row[idx]);
                        }
                    }
                }
            }

            return results.ToArray();

        }

        public static string DetectDelimiter(string str)
        {
            Console.Out.WriteLine("warn, delimiter detection not yet implemented");
            return "|";
        }

        //https://stackoverflow.com/questions/1287567/is-using-random-and-orderby-a-good-shuffle-algorithm
        //thank you Jon Skeet ;)
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            T[] elements = source.ToArray();
            for (int i = elements.Length - 1; i >= 0; i--)
            {
                // Swap element "i" with a random earlier element it (or itself)
                // ... except we don't really need to swap it fully, as we can
                // return it immediately, and afterwards it's irrelevant.
                int swapIndex = rng.Next(i + 1);
                yield return elements[swapIndex];
                elements[swapIndex] = elements[i];
            }
        }

        public static void ExportToDelimitedText(this DataTable source, string path, string delimiter)
        {
            using (var writer = new StreamWriter(path))
            {
                using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csvWriter.Configuration.Delimiter = delimiter;
                    foreach (DataColumn c in source.Columns)
                    {
                        csvWriter.WriteField(c.ColumnName);
                    }
                    foreach (DataRow r in source.Rows)
                    {
                        csvWriter.NextRecord();
                        for (int i = 0; i < source.Columns.Count; i++)
                        {
                            csvWriter.WriteField(r[i].ToString());
                        }
                    }
                }
            }
        }
        public static void ExportToDelimitedText(this IEnumerable<DataRow> source, string path, string delimiter, DataColumnCollection header)
        {
            using (var writer = new StreamWriter(path))
            {
                using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csvWriter.Configuration.Delimiter = delimiter;
                    foreach (DataColumn c in header)
                    {
                        csvWriter.WriteField(c.ColumnName);
                    }
                    csvWriter.NextRecord();
                    foreach (DataRow r in source)
                    {
                        for (int i = 0; i < header.Count; i++)
                        {
                            csvWriter.WriteField(r[i].ToString());
                        }
                        csvWriter.NextRecord();
                    }
                }
            }
        }

        public static void ExportToSqlTable(this IEnumerable<DataRow> source, DataColumnCollection header, string connectionString, string schemaAndTable, bool AppendIfTableExists = false)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                //does table exist?
                bool tableExists = DoesTableExist(schemaAndTable, conn);
                //error if append is not allowed and table exists
                if (tableExists && !AppendIfTableExists)
                {
                    Console.Error.WriteLine($"{schemaAndTable} already exists and appending is set to false");
                    return;
                }
                if (!tableExists)
                {
                    //create table if it doesn't exist
                    string[] cols = new string[header.Count];
                    string[] datatypes = new string[header.Count];
                    for (int i = 0; i < header.Count; i++)
                    {
                        cols[i] = header[i].ColumnName;
                        datatypes[i] = DataTypeMapping.SystemToSql[header[i].DataType];
                    }
                    DestinationTableCreator dtc = new DestinationTableCreator(schemaAndTable, cols, datatypes);
                    //using d2s class for this
                    ConfigVariables.Instance.ConfiguredConnection = connectionString; //this is where the d2s class get their connstring from
                    dtc.CreateTable();
                }
                //fill table
                using (SqlBulkCopy bulk = new SqlBulkCopy(conn))
                {
                    bulk.BatchSize = 10000;
                    bulk.DestinationTableName = schemaAndTable;
                    bulk.WriteToServer(rows: source.ToArray());
                }

            }
        }


        public static void ExportToSqlTable(this DataTable source, string connectionString, string schemaAndTable, bool AppendIfTableExists = false)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                //does table exist?
                bool tableExists = DoesTableExist(schemaAndTable, conn);
                //error if append is not allowed and table exists
                if (tableExists && !AppendIfTableExists)
                {
                    Console.Error.WriteLine($"{schemaAndTable} already exists and appending is set to false");
                    return;
                }
                if (!tableExists)
                {
                    //create table if it doesn't exist
                    string[] cols = new string[source.Columns.Count];
                    string[] datatypes = new string[source.Columns.Count];
                    for (int i = 0; i < source.Columns.Count; i++)
                    {
                        cols[i] = source.Columns[i].ColumnName;
                        datatypes[i] = DataTypeMapping.SystemToSql[source.Columns[i].DataType];
                    }
                    DestinationTableCreator dtc = new DestinationTableCreator(schemaAndTable, cols, datatypes);
                    //using d2s class for this
                    ConfigVariables.Instance.ConfiguredConnection = connectionString; //this is where the d2s class get their connstring from
                    dtc.CreateTable();
                }
                //fill table
                using (SqlBulkCopy bulk = new SqlBulkCopy(conn))
                {
                    bulk.BatchSize = 10000;
                    bulk.DestinationTableName = schemaAndTable;
                    bulk.WriteToServer(source);
                }

            }
        }
        private static bool DoesTableExist(string schemaAndTable, SqlConnection conn)
        {
            bool tableExists;
            using (SqlCommand comm = new SqlCommand())
            {
                comm.Connection = conn;
                comm.CommandText = "select object_id(@object)";
                comm.Parameters.AddWithValue("@object", schemaAndTable);
                comm.Prepare();
                var retValue = comm.ExecuteScalar();
                tableExists = retValue != null;
            }

            return tableExists;
        }

    }
}
