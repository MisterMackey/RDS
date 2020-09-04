using System;
using System.IO;
using System.Linq;

namespace RelationalSubsettingLib.Functions
{
    public class DEBUG_DUMPRELATIONINFO
    {
        #region Public Methods

        public void Run()
        {
            DirectoryInfo rds = new DirectoryInfo(Environment.CurrentDirectory + "\\.rds");
            var files = rds.EnumerateFiles().Where(f => f.Extension.Contains("rdskrf"));
            foreach (var file in files)
            {
                string text = File.ReadAllText(file.FullName);
                Console.Out.WriteLine($"{file.Name}: ");
                Console.Out.WriteLine(text);
            }
        }

        #endregion Public Methods
    }
}