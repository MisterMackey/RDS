using CsvHelper;
using Newtonsoft.Json;
using RelationalSubsettingLib.Sources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RelationalSubsettingLib.Subsetting
{
    public class Subsetter
    {
        private SubsettingOptions m_Options;

        public Subsetter(SubsettingOptions options)
        {
            m_Options = options;
        }

        public async Task CreateSubsetAsync()
        {
            Console.Out.WriteLine("Starting subset creation with settings:");
            string txt = JsonConvert.SerializeObject(m_Options, Formatting.Indented);
            Console.Out.WriteLine(txt);
            await Task.Factory.StartNew(
                Start);
        }

        private void Start()
        {
            Console.Out.WriteLine("Detecting files...");
            List<DataSourceInformation> DataSourceInformations = RetrieveDataSourceInformationList();
            Console.Out.WriteLine($"{DataSourceInformations.Count} files detected");

            Console.Out.WriteLine("Loading base file");
            DataTable baseFileTable = new DataTable(m_Options.BaseFileName);
            if (!TryLoadBaseFile(baseFileTable, DataSourceInformations))
            {
                return;
            }
            Console.Out.WriteLine("Basefile loaded");
            Console.Out.WriteLine("Determining base column values...");

            DataTable uniques = RetrieveUniqueColumnValues(baseFileTable, m_Options.BaseColumn);
            Console.Out.WriteLine("Base Column values loaded.");
            int AmountToSelect = (int)Math.Floor(m_Options.Factor * (double)uniques.Rows.Count);
            Console.Out.WriteLine($"Selecting {AmountToSelect} values for subset...");
            DataTable selected = SelectRandomValues(uniques, AmountToSelect);
            Console.Out.WriteLine($"Selected {selected.Rows.Count} values");
            Console.Out.WriteLine("Subsetting base file...");
            DataTable BaseFileSubset = SubSetBaseFile(baseFileTable, selected);
            Console.Out.WriteLine("Subsetting succeeded");
            //apply masking where necessary
            var inf = DataSourceInformations.Where(x => x.SourceName.Equals(m_Options.BaseFileName)).FirstOrDefault();

            if (inf.MaskingInformation.Any())
            {
                throw new NotImplementedException("Masking not yet implemented");
            }

            //clean some stuff up i guess
            baseFileTable.Dispose();
            uniques.Dispose();
            baseFileTable = null;
            uniques = null;
            Console.Out.WriteLine("Writing basefile subset...");
            _CreateSubset(BaseFileSubset, inf);
            Console.Out.WriteLine("Write completed");
            Console.Out.WriteLine("Writing related files");
            List<KeyRelationship> relations = RetrieveKeyRelationships();

            var related = relations.
                Where(x => x.Primary.FileName.Equals(m_Options.BaseFileName) || x.Foreign.FileName.Equals(m_Options.BaseFileName)).
                ToList();
            if (related.Count == 0)
            {
                Console.Out.WriteLine("No related files defined");
                return;
            }
            else
            {
                Console.Out.WriteLine("Loading related files");
                bool recurse = true;
                _SubsetRelatedFiles(BaseFileSubset, related, DataSourceInformations, recurse, m_Options.BaseFileName);
            }
        }

        private void _SubsetRelatedFiles(DataTable baseFileSubset, List<KeyRelationship> relations, List<DataSourceInformation> dfInfos, bool Recurse, string baseFileName)
        {
            foreach(var rel in relations)
            {
                //set basefile column name (used in the 'exists' clause) to the column that is from the basefile, whether its primary of foreign
                bool IsPrimaryColumnInBaseFile = rel.Primary.FileName.Equals(baseFileName);
                string PrimaryFileColumnName = IsPrimaryColumnInBaseFile ? rel.Primary.Column : rel.Foreign.Column;
                //same but reverse
                string RelatedFileColumnName = IsPrimaryColumnInBaseFile ? rel.Foreign.Column : rel.Primary.Column;
                //DataSourceInformation for related file
                string otherfileFileName = IsPrimaryColumnInBaseFile ? rel.Foreign.FileName : rel.Primary.FileName;
                var otherFileDataSourceInformation = dfInfos.
                    Where(x => x.SourceName.Equals(otherfileFileName)).FirstOrDefault();
                DataTable RelatedDataTable = new DataTable(otherfileFileName);
                Console.Out.WriteLine($"Loading {otherfileFileName}");
                LoadFileToDataTable(RelatedDataTable, otherFileDataSourceInformation);
                //select where exists, linq optimizer do ur magic :S
                //actually lets optimize this so we definitely don't enumerate the inner query hundreds of times
                List<string> validList = (from baseData in baseFileSubset.AsEnumerable()
                                          select (string)baseData[PrimaryFileColumnName]
                                ).ToList();
                IEnumerable<DataRow> ValidRowsFromSource = from otherData in RelatedDataTable.AsEnumerable()
                                where validList.
                                    Any(x => x.Equals((string)otherData[RelatedFileColumnName]))
                                select otherData;
                _CreateSubset(ValidRowsFromSource, otherFileDataSourceInformation, RelatedDataTable.Columns);
                Console.Out.WriteLine($"{otherfileFileName}: subset created");
                if (Recurse)
                {
                    //repeat this stuff for the relations of this file we just made
                    List<KeyRelationship> recursiveRelations = RetrieveKeyRelationships();
                    //excluding relations that point back to the file we just came from in the previous recursion step
                    var related = recursiveRelations.
                        Where(x => (x.Primary.FileName.Equals(otherfileFileName) && !x.Foreign.FileName.Equals(baseFileName)) 
                        || (x.Foreign.FileName.Equals(otherfileFileName) && !x.Primary.FileName.Equals(baseFileName)) ).
                        ToList();
                    _SubsetRelatedFiles(ValidRowsFromSource, related, dfInfos, true, otherfileFileName);
                }
            }
        }
        private void _SubsetRelatedFiles(IEnumerable<DataRow> baseFileSubset, List<KeyRelationship> relations, List<DataSourceInformation> dfInfos, bool Recurse, string baseFileName)
        {
            foreach (var rel in relations)
            {
                //set basefile column name (used in the 'exists' clause) to the column that is from the basefile, whether its primary of foreign
                bool IsPrimaryColumnInBaseFile = rel.Primary.FileName.Equals(baseFileName);
                string PrimaryFileColumnName = IsPrimaryColumnInBaseFile ? rel.Primary.Column : rel.Foreign.Column;
                //same but reverse
                string RelatedFileColumnName = IsPrimaryColumnInBaseFile ? rel.Foreign.Column : rel.Primary.Column;
                //DataSourceInformation for related file
                string otherfileFileName = IsPrimaryColumnInBaseFile ? rel.Foreign.FileName : rel.Primary.FileName;
                var otherFileDataSourceInformation = dfInfos.
                    Where(x => x.SourceName.Equals(otherfileFileName)).FirstOrDefault();
                DataTable RelatedDataTable = new DataTable(otherfileFileName);
                Console.Out.WriteLine($"Loading {otherfileFileName}");
                LoadFileToDataTable(RelatedDataTable, otherFileDataSourceInformation);
                //select where exists, linq optimizer do ur magic :S
                //actually lets optimize this so we definitely don't enumerate the inner query hundreds of times
                List<string> validList = (from baseData in baseFileSubset
                                          select (string)baseData[PrimaryFileColumnName]
                                ).ToList();
                IEnumerable<DataRow> rowsIWant = from otherData in RelatedDataTable.AsEnumerable()
                                                 where validList.
                                                     Any(x => x.Equals((string)otherData[RelatedFileColumnName]))
                                                 select otherData;
                _CreateSubset(rowsIWant, otherFileDataSourceInformation, RelatedDataTable.Columns);
                Console.Out.WriteLine($"{otherfileFileName}: subset created");
                if (Recurse)
                {
                    //repeat this stuff for the relations of this file we just made
                    List<KeyRelationship> recursiveRelations = RetrieveKeyRelationships();
                    //excluding relations that point back to the file we just came from in the previous recursion step
                    var related = recursiveRelations.
                        Where(x => (x.Primary.FileName.Equals(otherfileFileName) && !x.Foreign.FileName.Equals(baseFileName))
                        || (x.Foreign.FileName.Equals(otherfileFileName) && !x.Primary.FileName.Equals(baseFileName))).
                        ToList();
                    _SubsetRelatedFiles(RelatedDataTable, related, dfInfos, Recurse:true, otherfileFileName);
                }
            }
        }

            private List<KeyRelationship> RetrieveKeyRelationships()
        {
            var info = new FileInfo($"{Environment.CurrentDirectory}\\{Properties.Settings.RdsDirectoryName}\\{Properties.Settings.KeyRelationshipFileName}");
            return new List<KeyRelationship>().LoadFromFile(info.FullName);
        }

        private void _CreateSubset(DataTable dt, DataSourceInformation info)
        {
            if (info.GetType() == typeof(DataFileInfo))
            {
                DataFileInfo dfinfo = info as DataFileInfo;
                _CreateSubsetDataFileInfo(dt, dfinfo);
            }
            if (info.GetType() == typeof(SourceTableInfo))
            {
                SourceTableInfo stinfo = info as SourceTableInfo;
                _CreateSubsetSourceTableInfo(dt, stinfo);
            }
        }

        private void _CreateSubsetSourceTableInfo(DataTable dt, SourceTableInfo info)
        {
            string schemaAndTable = $"{info.SchemaName}.{info.TableName}_Subset";
            dt.ExportToSqlTable(info.ConnectionString, schemaAndTable, true);
        }
        private void _CreateSubsetDataFileInfo(DataTable dt, DataFileInfo info)
        {
            string path = $"{m_Options.TargetPath}\\{info.Info.Name}";
            if (path.Equals(info.Info.FullName))
            {
                Console.Error.WriteLine("Error: subset creation would overwrite original files, please specify a different targetpath.");
                return;
            }
            string delim = info.Delimiter;
            dt.ExportToDelimitedText(path, delim);
        }
        private void _CreateSubset(IEnumerable<DataRow> dt, DataSourceInformation info, DataColumnCollection header)
        {
            if (info.GetType() == typeof(DataFileInfo))
            {
                DataFileInfo dfinfo = info as DataFileInfo;
                _CreateSubsetDataFileInfo(dt, dfinfo, header);
            }
            if (info.GetType() == typeof(SourceTableInfo))
            {
                SourceTableInfo stinfo = info as SourceTableInfo;
                __CreateSubsetSourceTableInfo(dt, stinfo, header);
            }
        }
        private void __CreateSubsetSourceTableInfo(IEnumerable<DataRow> dt, SourceTableInfo info, DataColumnCollection header)
        {
            dt.ExportToSqlTable(header, info.ConnectionString, $"{info.SchemaName}.{info.TableName}_Subeset", AppendIfTableExists: false);
        }
        private void _CreateSubsetDataFileInfo(IEnumerable<DataRow> dt, DataFileInfo info, DataColumnCollection header)
        {
            string path = $"{m_Options.TargetPath}\\{info.Info.Name}";
            if (path.Equals(info.Info.FullName))
            {
                Console.Error.WriteLine("Error: subset creation would overwrite original files, please specify a different targetpath.");
                return;
            }
            string delim = info.Delimiter;
            dt.ExportToDelimitedText(path, delim, header);
        }

        private DataTable SubSetBaseFile(DataTable baseFileTable, DataTable selected)
        {
            IEnumerable<DataRow> RowsIWant =
                from baseTable in baseFileTable.AsEnumerable()
                join desiredValues in selected.AsEnumerable()
                    on baseTable[m_Options.BaseColumn] equals desiredValues[m_Options.BaseColumn]
                select baseTable;
            DataTable BaseFileSubset = baseFileTable.Clone();
            foreach (var r in RowsIWant)
            {
                var newrow = BaseFileSubset.NewRow();
                newrow.ItemArray = r.ItemArray;
                BaseFileSubset.Rows.Add(newrow);
            }
            return BaseFileSubset;
        }

        private static DataTable SelectRandomValues(DataTable uniques, int AmountToSelect)
        {
            string[] selected;
            string[] all = new string[uniques.Rows.Count];
            for (int i = 0; i < uniques.Rows.Count; i++)
            {
                all[i] = uniques.Rows[i][0].ToString();
            }
            Random rng = new Random();
            var shuffled = all.Shuffle(rng);

            selected = shuffled.Take(AmountToSelect).ToArray();
            DataTable sel = uniques.Clone(); 
            for (int i = 0; i < selected.Length; i++)
            {
                sel.Rows.Add(selected[i]);
            }
            return sel;
        }

        private DataTable RetrieveUniqueColumnValues(DataTable baseFileTable, string col)
        {
            DataTable uniques = new DataTable("uniques");
            var uniquerows = (from rows in baseFileTable.AsEnumerable()
                              select rows[col]).Distinct();
            uniques.Columns.Add(col);
            foreach(var r in uniquerows)
            {
                uniques.Rows.Add(r);
            }
            int numUniques = uniques.Rows.Count;
            Console.Out.WriteLine($"{numUniques} unique values found");
            return uniques;
        }

        private bool TryLoadBaseFile(DataTable baseFileTable, List<DataSourceInformation> DataSourceInformations)
        {
            var baseDfInfo = DataSourceInformations.Where(x => x.SourceName.Equals(m_Options.BaseFileName)).FirstOrDefault();
            if (baseDfInfo == null)
            {
                Console.Error.WriteLine("Basefile not found amongst files. exiting");
                return false;
            }
            LoadFileToDataTable(baseFileTable, baseDfInfo);
            return true;
        }

        private static void LoadFileToDataTable(DataTable table, DataSourceInformation dfInfo)
        {
            dfInfo.LoadToDataTable(table);
        }

        private static List<DataSourceInformation> RetrieveDataSourceInformationList()
        {
            string rdsDir = Properties.Settings.RdsDirectoryName;
            DirectoryInfo rdsinfo = new DirectoryInfo($"{Environment.CurrentDirectory}\\{rdsDir}");
            string ext = Properties.Settings.DataSourceFileExtension;
            List<DataSourceInformation> DataSourceInformations = rdsinfo.EnumerateFiles().
                Where(x => x.Extension.Equals(ext)).
                Select(x => DataSourceInformation.LoadFromFile(x.FullName)).
                ToList();
            return DataSourceInformations;
        }
    }
}
