using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// Holds info on a position data file like column names, primary-foreign key relationships and which columns to use as a basis for subsetting.
/// as well as a fileinfo object on the file
/// </summary>
namespace RelationalSubsettingLib
{
    public class DataFileInfo
    {
        public FileInfo Info;
        public string[] Columns;
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
    }
}
