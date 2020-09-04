using System;
using System.IO;

namespace RelationalSubsettingLib.Functions
{
    public class Clean
    {
        #region Public Methods

        public void Run()
        {
            string path = Environment.CurrentDirectory + "//.rds";
            if (!Directory.Exists(path))
            {
                Console.Out.WriteLine("Nothing to clean");
                return;
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            directoryInfo.Delete(true);
            Console.Out.WriteLine("Repository cleaned up");
            return;
        }

        #endregion Public Methods
    }
}