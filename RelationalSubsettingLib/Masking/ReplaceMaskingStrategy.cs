using RelationalSubsettingLib.Properties;
using System;
using System.Data;

namespace RelationalSubsettingLib.Masking
{
    /// <summary>
    /// The <see cref="ReplaceMaskingStrategy"/> class implements an <see cref="IMaskingStrategy"/> that replaces values
    /// with a new value. This value is a static value that is injected in the constructor or taken from app.config
    /// during Initialize if no value is supplied in the constructor
    /// </summary>
    public class ReplaceMaskingStrategy : IMaskingStrategy
    {
        #region Public Properties

        public Action<DataRow> ParallelMaskingAction
        {
            get
            {
                ValidateBeforeReturningAction();
                return LeAction;
            }
        }

        #endregion Public Properties

        #region Private Fields

        private string ColumnName;
        private string ReplaceValue;

        #endregion Private Fields

        #region Public Constructors

        public ReplaceMaskingStrategy()
        {
        }

        public ReplaceMaskingStrategy(string ReplaceValue) : this()
        {
            this.ReplaceValue = ReplaceValue;
        }

        #endregion Public Constructors

        #region Public Methods

        public void Initialize(DataTable table, string columnName)
        {
            this.ColumnName = columnName;
            if (!table.Columns.Contains(columnName))
            {
                throw new ArgumentException("The provided columname could not be found in the provided datatable");
            }
            if (ReplaceValue == null)
            {
                ReplaceValue = Settings.DefaultReplacementValueForReplaceStrategy;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void LeAction(DataRow row)
        {
            row[ColumnName] = ReplaceValue;
        }

        private void ValidateBeforeReturningAction()
        {
            if (string.IsNullOrEmpty(ColumnName) || ReplaceValue == null)
            {
                throw new InvalidOperationException("This instance of ReplaceMaskingStrategy is not initialized correctly. Provide a valid columnName and a non-null replacemnet value string");
            }
        }

        #endregion Private Methods
    }
}