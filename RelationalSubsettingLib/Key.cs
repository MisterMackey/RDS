using System.IO;

namespace RelationalSubsettingLib
{
    public class Key
    {
        public string FileName;
        public string Column;
        public Key(string fileName, string column)
        {
            FileName = fileName;
            Column = column;
        }
        
    }
}