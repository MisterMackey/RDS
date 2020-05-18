using Microsoft.VisualStudio.TestTools.UnitTesting;
using RelationalSubsettingLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RelationalSubsettingLib.Tests
{
    [TestClass()]
    public class DataFileInfoTests
    {
        [TestMethod()]
        public void SerializeAndDeSerializeAsBaseClass()
        {
            string path = $"{Environment.CurrentDirectory}\\testfileinfofile.txt";

            try
            {
                DataFileInfo dfi = new DataFileInfo();
                dfi.Columns = new string[] { "Col1" };
                dfi.Delimiter = "|";
                DataSourceInformation baseclass = dfi as DataSourceInformation;
                baseclass.SaveToFile(path);
                Assert.IsTrue(File.Exists(path));
                DataSourceInformation loadedFromFile = DataSourceInformation.LoadFromFile(path);
                DataFileInfo dfi2 = loadedFromFile as DataFileInfo;
                Assert.AreEqual(expected: dfi.Delimiter, actual: dfi2.Delimiter);
                Assert.AreEqual(expected: dfi.Columns[0], actual: dfi2.Columns[0]);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }


    }
}