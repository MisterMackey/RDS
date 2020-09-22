using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace RelationalSubsettingLib.Masking
{
    public class DataMasker : IDataMasker
    {
        #region Private Fields

        private IMaskingStrategy m_Strategy;

        #endregion Private Fields

        #region Public Constructors

        public DataMasker(IMaskingStrategy strategy)
        {
            m_Strategy = strategy;
        }

        #endregion Public Constructors

        #region Public Methods

        public async Task MaskDataRowEnumerableAsync(IEnumerable<DataRow> source, string columnName)
        {
            await Task.Factory.StartNew(
                () => DoMasking(m_Strategy, source, columnName));
        }

        public async Task MaskDatatableAsync(DataTable dataTable, string columnName)
        {
            await Task.Factory.StartNew(
                () => DoMasking(m_Strategy, dataTable, columnName));
        }

        #endregion Public Methods

        #region Private Methods

        private void DoMasking(IMaskingStrategy strategy, DataTable table, string columnName)
        {
            strategy.Initialize(table, columnName);
            foreach (var row in table.AsEnumerable())
            {
                strategy.ParallelMaskingAction(row);
            }
        }

        private void DoMasking(IMaskingStrategy strategy, IEnumerable<DataRow> source, string columnName)
        {
            strategy.Initialize(source.First().Table, columnName);
            foreach (var row in source)
            {
                strategy.ParallelMaskingAction(row);
            }
        }

        #endregion Private Methods
    }
}