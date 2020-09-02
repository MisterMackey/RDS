using Microsoft.VisualStudio.TestTools.UnitTesting;
using RelationalSubsettingLib.Masking;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RelationalSubsettingLib.Masking.Tests
{
    [TestClass()]
    public class ReplaceMaskingStrategyTests
    {
        [TestMethod()]
        public void InitializeTest()
        {
            ReplaceMaskingStrategy r = new ReplaceMaskingStrategy();
            DataTable dt = new DataTable();
            dt.Columns.Add("A");
            //invalid columnanem should fail
            Assert.ThrowsException<ArgumentException>(
                () => r.Initialize(dt, "B"));
            r.Initialize(dt, "A");
        }


        [TestMethod()]
        public void ParallelMaskingActionTest()
        {
            //init with explicit replacement value
            ReplaceMaskingStrategy r = new ReplaceMaskingStrategy("x");
            //grab the method without properly initiazling (aka putting a columnname down)
            Assert.ThrowsException<InvalidOperationException>(()
                => r.ParallelMaskingAction);
            // now do the needful and see if we get the method out
            DataTable dt = new DataTable();
            dt.Columns.Add("A");
            r.Initialize(dt, "A");
            var x = r.ParallelMaskingAction;
            Assert.IsInstanceOfType(x, typeof(Action<DataRow>));
            //lets also see if the method is doing stuff properly
            var row = dt.NewRow();
            row["A"] = "Y";
            dt.Rows.Add(row);
            Assert.IsTrue("Y".Equals(dt.Rows[0]["A"]));
            x.Invoke(dt.Rows[0]);
            Assert.IsTrue("x".Equals(dt.Rows[0]["A"]));

        }
    }
}