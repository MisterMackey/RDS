using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RelationalSubsettingLib.Masking
{
    public class DataMasker : IDataMasker
    {
        #region Private Fields

        private ParallelOptions m_Options;
        private IMaskingStrategy m_Strategy;

        #endregion Private Fields

        #region Public Constructors

        public DataMasker(IMaskingStrategy strategy)
        {
            m_Options = new ParallelOptions();
            m_Options.MaxDegreeOfParallelism = Environment.ProcessorCount / 2;
            m_Strategy = strategy;
        }

        public DataMasker(IMaskingStrategy strategy, ParallelOptions options)
        {
            m_Strategy = strategy;
            m_Options = options;
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

            Parallel.ForEach(
                source: table.AsEnumerable(),
                parallelOptions: m_Options,
                body: strategy.ParallelMaskingAction
            );
        }

        private void DoMasking(IMaskingStrategy strategy, IEnumerable<DataRow> source, string columnName)
        {
            strategy.Initialize(source.First().Table, columnName);

            Parallel.ForEach(
                source: source,
                parallelOptions: m_Options,
                body: strategy.ParallelMaskingAction
            );
        }

        #endregion Private Methods
    }
}