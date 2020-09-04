namespace RelationalSubsettingLib
{
    public class Key
    {
        #region Public Fields

        public string Column;
        public string FileName;

        #endregion Public Fields

        #region Public Constructors

        public Key(string fileName, string column)
        {
            FileName = fileName;
            Column = column;
        }

        #endregion Public Constructors
    }
}