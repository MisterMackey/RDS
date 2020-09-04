using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

/// <summary>
/// Holds info on a position data file like column names, primary-foreign key relationships and which columns to use as
/// a basis for subsetting. as well as a fileinfo object on the file
/// </summary>
namespace RelationalSubsettingLib
{
    public class DataFileInfo : DataSourceInformation
    {
        #region Public Properties

        public override string[] Columns { get; set; }
        public override string ConcreteType => "DataFileInfo";
        public override string FullyQualifiedName { get => Info.FullName; }
        public override Dictionary<string, Tuple<MaskingOptions, string>> MaskingInformation { get; }
        public override string SourceName => Info.Name;

        #endregion Public Properties

        #region Public Fields

        public string Delimiter;
        public FileInfo Info;

        #endregion Public Fields

        #region Public Constructors

        public DataFileInfo(FileInfo information) : this()
        {
            Info = information;
            using (StreamReader reader = new StreamReader(Info.FullName))
            {
                string firstLine = reader.ReadLine();
                Delimiter = Extensions.DetectDelimiter(firstLine);
                Columns = firstLine.Delimit(Delimiter, null);
            }
        }

        public DataFileInfo()
        {
            MaskingInformation = new Dictionary<string, Tuple<MaskingOptions, string>>();
        }

        #endregion Public Constructors

        #region Public Methods

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

        #endregion Public Methods
    }
}