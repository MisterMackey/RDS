using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

/// <summary>
/// Holds info on a position data file like column names, primary-foreign key relationships and which columns to use as a basis for subsetting.
/// as well as a fileinfo object on the file
/// </summary>
namespace RelationalSubsettingLib
{
    public class DataFileInfo : DataSourceInformation
    {
        public override string ConcreteType => "DataFileInfo";
        public FileInfo Info;
        public override string[] Columns { get; set; }
        public string Delimiter;
        public DataFileInfo(FileInfo information) : this()
        {
            Info = information;
            using (StreamReader reader = new StreamReader(Info.FullName))
            {
                string firstLine = reader.ReadLine();
                Delimiter = Extensions.DetectDelimiter(firstLine);  
                Columns = firstLine.Delimit(Delimiter,null);
            }
        }
        public DataFileInfo()
        {

        }

        public override void LoadToDataTable(DataTable table)
        {
            using (StreamReader reader = new StreamReader(Info.FullName))
            {
                using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Configuration.Delimiter = Delimiter;
                    using (CsvDataReader csvreader = new CsvDataReader(csv))
                    {
                        table.Load(csvreader);
                    }
                }
            }
        }

        public override string FullyQualifiedName { get => Info.FullName; }
        public override string SourceName => Info.Name;
    }
}
