using System.Data;
using System.Threading.Tasks;

namespace RelationalSubsettingLib.Masking
{
    public interface IDataMasker
    {
        #region Public Methods

        Task MaskDatatableAsync(DataTable dataTable, string columnName);

        #endregion Public Methods
    }
}