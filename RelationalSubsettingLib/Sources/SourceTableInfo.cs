using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace RelationalSubsettingLib.Sources
{
    public class SourceTableInfo : DataSourceInformation
    {
        public override string ConcreteType => "SourceTableInfo";

        public override string SourceName => throw new NotImplementedException();

        public override string FullyQualifiedName => throw new NotImplementedException();

        public override void LoadToDataTable(DataTable table)
        {
            throw new NotImplementedException();
        }
    }
}
