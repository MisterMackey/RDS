using RelationalSubsettingLib.Subsetting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RelationalSubsettingLib.Functions
{
    public class Init
    {
        #region Public Methods

        public void Run()
        {
            string[] extensionWhiteList = new string[]
            {
                ".txt"
                ,".csv"
            };
            string ext = Properties.Settings.DataSourceFileExtension;
            string settingsFileName = Properties.Settings.RdsSubsettingSettingsFileName;
            string keyrelationFileName = Properties.Settings.KeyRelationshipFileName;
            string rdsDir = Properties.Settings.RdsDirectoryName;
            string dir = Environment.CurrentDirectory;
            Console.Out.WriteLine($"Initializing in {dir}");
            DirectoryInfo directoryInfo = new DirectoryInfo(dir);
            var settingsdir = directoryInfo.CreateSubdirectory($".\\{rdsDir}");
            settingsdir.Attributes = settingsdir.Attributes | FileAttributes.Hidden; //set folder to hidden
            var files = directoryInfo.EnumerateFiles().
                Where(leFile => leFile.Extension.InList(extensionWhiteList)).
                ToList();
            foreach (var f in files)
            {
                Console.Out.WriteLine($"Found file {f.Name}");
                DataFileInfo df = new DataFileInfo(f);
                int idx = f.Name.LastIndexOf('.');
                string nameWithoutExtension = f.Name.Substring(0, idx);
                df.SaveToFile($"{settingsdir.FullName}\\{nameWithoutExtension}{ext}");
            }
            List<KeyRelationship> keyRelationships = new List<KeyRelationship>();
            keyRelationships.SaveToFile($"{settingsdir.FullName}\\{keyrelationFileName}");
            SubsettingOptions subsettingOptions = new SubsettingOptions();
            subsettingOptions.TargetPath = dir + "\\subset";
            Directory.CreateDirectory(subsettingOptions.TargetPath);
            subsettingOptions.SaveToFile($"{settingsdir.FullName}\\{settingsFileName}");
        }

        #endregion Public Methods
    }
}