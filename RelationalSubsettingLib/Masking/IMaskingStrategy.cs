using System;
using System.Data;

namespace RelationalSubsettingLib.Masking
{
    public interface IMaskingStrategy
    {
        #region Public Properties

        Action<DataRow> ParallelMaskingAction { get; }

        #endregion Public Properties

        #region Public Methods

        void Initialize(DataTable table, string columnName);

        #endregion Public Methods
    }
}