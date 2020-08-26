using Microsoft.VisualStudio.TestTools.UnitTesting;
using RelationalSubsettingLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace RelationalSubsettingLib.Tests
{
    [TestClass()]
    public class ExtensionsTests
    {
        private string connstring = "Server = (localdb)\\MSSQLLocalDB; Initial Catalog=master; Integrated Security=true;";

        [TestMethod()]
        public void ExportToSqlTableTest()
        {
            DataTable dt = new DataTable("hurpTest");
            dt.Columns.Add("TestColumn");
            for (int i = 0; i < 10; i++)
            {
                var r = dt.NewRow();
                r[0] = "Hurp";
                dt.Rows.Add(r);
            }
            dt.ExportToSqlTable(connstring, "dbo.hurpTest", true);
            dt.ExportToSqlTable(connstring, "dbo.hurpTest", false);
        }

        [TestMethod()]
        public void DelimitTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DelimitTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SaveToFileTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void LoadFromFileTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void NotInListTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void InListTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SplitRowTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DetectDelimiterTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ShuffleTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ExportToDelimitedTextTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ExportToDelimitedTextTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ExportToSqlTableTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ExportToSqlTableTest2()
        {
            Assert.Fail();
        }
    }
}