using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace RelationalSubsettingLib.Masking
{
    public interface IMaskingStrategy
    {
        void Initialize(DataTable table, string columnName);

        Action<DataRow> ParallelMaskingAction { get; }
    }
}
