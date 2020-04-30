using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace RelationalSubsettingLib.Functions
{
    /// <summary>
    /// corresponds to File param but i don't wanna call it File otherwise it must do battle
    /// against System.IO.File in the autocorrect
    /// </summary>
    public class RdsFile
    {
        private Dictionary<string, Action<string[]>> ModeMapping;

        public RdsFile()
        {
            ModeMapping = new Dictionary<string, Action<string[]>>()
            {
                {"-d", modeSetDelimiter },
                {"-SetDelimiter", modeSetDelimiter }
            };
        }



        public void Run(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("The File command requires a mode. Use -help for usage");
                return;
            }
            string mode = args[1];
            ModeMapping[mode](args);
        }

        private void modeSetDelimiter(string[] obj)
        {
            //correct usage: rds File -d {filename} {delim}. if -a is given instead of filename
            //set given delimter for ALL files. filename can be regex pattern
            if (obj.Length != 4)
            {
                Console.Error.WriteLine("Correct usage: rds File -d {filenameRegex | -a} {delim}");
            }
            string regex;
            if (obj[2] == "-a" || obj[2] == "All")
            {
                regex = ".+";
            }
            else
            {
                regex = obj[2];
            }
            string delim = obj[3];
            SetDelimiterForFiles(regex, delim);
        }

        private void SetDelimiterForFiles(string regex, string delim)
        {
            var files = GetFilesBasedOnRegex(regex);
            foreach (var f in files)
            {
                Console.Out.WriteLine($"changing delimiter of {f.Info.Name} to {delim}.");
                f.Delimiter = delim;
                ReDetermineColumns(f);
                Save(f);
            }
        }

        private void Save(DataFileInfo f)
        {
            string settingsdir = $"{Environment.CurrentDirectory}\\.rds";
            int idx = f.Info.Name.LastIndexOf('.');
            string nameWithoutExtension = f.Info.Name.Substring(0, idx);
            f.SaveToFile($"{settingsdir}\\{nameWithoutExtension}.rdsfif");
        }

        private void ReDetermineColumns(DataFileInfo f)
        {
            using (StreamReader reader = new StreamReader(f.Info.FullName))
            {
                string firstLine = reader.ReadLine();                
                f.Columns = firstLine.Delimit(f.Delimiter, null);                
            }
        }

        private IEnumerable<DataFileInfo> GetFilesBasedOnRegex(string regex)
        {
            Regex rg = new Regex(regex);
            DirectoryInfo rdsinfo = new DirectoryInfo($"{Environment.CurrentDirectory}\\.rds");
            var DataFileInfos = rdsinfo.EnumerateFiles("*.rdsfif").
                Select(x => new DataFileInfo().LoadFromFile(x.FullName)).
                Where(x => rg.IsMatch(x.Info.Name));
            return DataFileInfos;
        }
    }
}
