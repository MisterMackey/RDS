using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace RelationalSubsettingLib.Masking
{
    public class DataMasker : IDataMasker
    {
        private IMaskingStrategy m_Strategy;
        private ParallelOptions m_Options;
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
        public async Task MaskDatatableAsync(DataTable dataTable, string columnName)
        {
            await Task.Factory.StartNew(
                () => DoMasking(m_Strategy, dataTable, columnName));
        }

        private void DoMasking(IMaskingStrategy strategy, DataTable table, string columnName)
        {
            strategy.Initialize(table, columnName);

            Parallel.ForEach(
                source: table.AsEnumerable(), 
                parallelOptions: m_Options, 
                body: strategy.ParallelMaskingAction
            );
        }
    }
}
