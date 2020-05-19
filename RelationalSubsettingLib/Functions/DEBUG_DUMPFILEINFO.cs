using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RelationalSubsettingLib.Functions
{
    public class DEBUG_DUMPFILEINFO
    {
        public void Run()
        {
            DirectoryInfo rds = new DirectoryInfo(Environment.CurrentDirectory + "\\.rds");
            string ext = Properties.Settings.DataSourceFileExtension;
            var files = rds.EnumerateFiles().Where(f => f.Extension.Contains(ext));
            foreach (var file in files)
            {
                string text = File.ReadAllText(file.FullName);
                Console.Out.WriteLine($"{file.Name}: ");
                Console.Out.WriteLine(text);
            }
        }
        
    }
}
