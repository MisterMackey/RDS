using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;

namespace RelationalSubsettingLib.Masking.Tests
{
    [TestClass()]
    public class DataMaskerTests
    {
        #region Public Methods

        [TestMethod()]
        public void DataMaskerConstructorTest()
        {
            DataMasker dm = new DataMasker(new MockStrategy());
        }

        [TestMethod()]
        public void MaskDataRowEnumerableAsync()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("A");
            var r = dt.NewRow();
            r["A"] = "pfdsa";
            dt.Rows.Add(r);
            //so now we got a databable with one row and column.
            DataMasker dm = new DataMasker(new MockStrategy());
            dm.MaskDataRowEnumerableAsync(dt.AsEnumerable(), "A").Wait();
            //make sure it didn't mess with my dt
            Assert.IsNotNull(dt);
            Assert.AreEqual(expected: "pfdsa", actual: dt.Rows[0]["A"]);
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

        #endregion Public Methods

        #region Private Classes

        private class MockStrategy : IMaskingStrategy
        {
            #region Public Properties

            public Action<DataRow> ParallelMaskingAction => mockAction;

            #endregion Public Properties

            #region Public Methods

            public void Initialize(DataTable table, string columnName)
            {
                //do nothing, get called anyway
            }

            #endregion Public Methods

            #region Private Methods

            private void mockAction(DataRow row)
            {
                //do nothing, get paid anyway
            }

            #endregion Private Methods
        }

        #endregion Private Classes
    }
}