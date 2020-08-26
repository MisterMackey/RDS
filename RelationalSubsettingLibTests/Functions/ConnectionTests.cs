using Microsoft.VisualStudio.TestTools.UnitTesting;
using RelationalSubsettingLib.Functions;
using RelationalSubsettingLib.Sql;
using System;
using System.Collections.Generic;
using System.Text;

namespace RelationalSubsettingLib.Functions.Tests
{
    [TestClass()]
    public class ConnectionTests
    {
        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            ConnectionAliases c = new ConnectionAliases();
            c.Clear();
        }
        [TestMethod()]
        public void ConnectionTest()
        {
            var x = new Connection();
        }

        [TestMethod()]
        public void ModeAddAndRemoveAndUpdateTest()
        {
            //putting them all into one as they depend on eachother anyway
            //expected result, connection object will save the added connection and be able to retrieve it
            var c = new Connection();
            string[] args = new string[] { "Connection", "-Add", "Test", "FakeConnString" };
            c.Run(args);
            //checking the underlying object to see if it worked
            ConnectionAliases ca = new ConnectionAliases();
            Assert.IsTrue(ca.ContainsKey("Test"));
            //update
            args = new string[] { "Connection", "-Update", "Test", "NewFakeConnString" };
            c.Run(args);
            ca = new ConnectionAliases(); //force refresh
            Assert.AreEqual(expected: "NewFakeConnString", actual: ca["Test"]);
            //remove
            args = new string[] { "Connection", "-Remove", "Test", "FakeConnString" };
            c.Run(args);
            ca = new ConnectionAliases(); //force refresh
            Assert.IsFalse(ca.ContainsKey("Test"));
        }

    }
}