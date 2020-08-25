using Microsoft.VisualStudio.TestTools.UnitTesting;
using RelationalSubsettingLib.Properties;
using RelationalSubsettingLib.Sql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;

namespace RelationalSubsettingLib.Sql.Tests
{
    [TestClass()]
    public class ConnectionAliasesTests
    {
        [TestInitialize()]
        public void TestInit()
        {
            //ensure there is no pre-existing settings file when a test in this file runs
            string fname = Settings.ConnectionAliasFileName;
            string folder = Directory.GetCurrentDirectory() + @"\.rds";
            string file = folder + "\\" + fname;
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
        [TestMethod()]
        public void AddAndGetTest()
        {
            ConnectionAliases c = new ConnectionAliases();
            //the following should defintely not crash
            Assert.IsFalse(c.ContainsKey("somekey"));
            //now when we add a key it should make the file
            string fname = Settings.ConnectionAliasFileName;
            string file = Directory.GetCurrentDirectory() + @"\.rds\" + fname;
            c["NewKey"] = "new connstring";
            Assert.IsTrue(File.Exists(file));
            //also this should obivously work
            Assert.IsTrue(c.ContainsKey("NewKey"));

            //finally, we should be abl eto get this key out on a new object also
            c = null;
            c = new ConnectionAliases();
            Assert.IsTrue(c.ContainsKey("NewKey"));
            Assert.AreEqual(expected: "new connstring", actual: c["NewKey"]);
        }



    }
}