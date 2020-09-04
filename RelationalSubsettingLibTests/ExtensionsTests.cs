using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace RelationalSubsettingLib.Tests
{
    [TestClass()]
    public class ExtensionsTests
    {
        #region Private Fields

        private string connstring = "Server = (localdb)\\MSSQLLocalDB; Initial Catalog=master; Integrated Security=true;";

        #endregion Private Fields

        #region Public Methods

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
        public void DetectDelimiterTest()
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
        public void ExportToSqlTableTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ExportToSqlTableTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void InListTest()
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
        public void SaveToFileTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ShuffleTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SplitRowTest()
        {
            Assert.Fail();
        }

        #endregion Public Methods
    }
}