using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace RelationalSubsettingLib.Masking
{
    public interface IDataMasker
    {
        Task MaskDatatableAsync(DataTable dataTable, string columnName);
    }
}
