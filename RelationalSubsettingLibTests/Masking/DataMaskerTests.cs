using Microsoft.VisualStudio.TestTools.UnitTesting;
using RelationalSubsettingLib.Masking;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace RelationalSubsettingLib.Masking.Tests
{
    [TestClass()]
    public class DataMaskerTests
    {
        [TestMethod()]
        public void DataMaskerConstructorTest()
        {
            DataMasker dm = new DataMasker(new MockStrategy());
            dm = new DataMasker(new MockStrategy(), new System.Threading.Tasks.ParallelOptions());
        }


        [TestMethod()]
        public void MaskDatatableAsyncTest()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("A");
            var r = dt.NewRow();
            r["A"] = "pfdsa";
            dt.Rows.Add(r);
            //so now we got a databable with one row and column.
            DataMasker dm = new DataMasker(new MockStrategy());
            dm.MaskDatatableAsync(dt, "A").Wait();
            //make sure it didn't mess with my dt
            Assert.IsNotNull(dt);
            Assert.AreEqual(expected: "pfdsa", actual: dt.Rows[0]["A"]);
        }

        private class MockStrategy : IMaskingStrategy
        {
            public Action<DataRow> ParallelMaskingAction => mockAction;


            public void Initialize(DataTable table, string columnName)
            {
                //do nothing, get called anyway
            }

            private void mockAction(DataRow row)
            {
                //do nothing, get paid anyway
            }
        }
    }

}