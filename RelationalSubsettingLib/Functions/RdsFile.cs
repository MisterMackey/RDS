﻿using RelationalSubsettingLib.Properties;
using RelationalSubsettingLib.Sources;
using RelationalSubsettingLib.Sql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RelationalSubsettingLib.Functions
{
    /// <summary>
    /// corresponds to File param but i don't wanna call it File otherwise it must do battle against System.IO.File in
    /// the autocorrect
    /// </summary>
    public class RdsFile
    {
        #region Private Fields

        private Dictionary<string, Action<string[]>> ModeMapping;

        #endregion Private Fields

        #region Public Constructors

        public RdsFile()
        {
            ModeMapping = new Dictionary<string, Action<string[]>>()
            {
                {"-d", modeSetDelimiter },
                {"-SETDELIMITER", modeSetDelimiter },
                {"-ADDFILE", modeAddFile },
                {"-ADDTABLE", modeAddTable },
                {"-MASK", modeMask }
            };
        }

        #endregion Public Constructors

        #region Public Methods

        public void Run(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("The File command requires a mode. Use -help for usage");
                return;
            }
            string mode = args[1].ToUpper();
            ModeMapping[mode](args);
        }

        #endregion Public Methods

        #region Private Methods

        private IEnumerable<DataFileInfo> GetFilesBasedOnRegex(string regex)
        {
            Regex rg = new Regex(regex);
            string ext = Properties.Settings.DataSourceFileExtension;
            DirectoryInfo rdsinfo = new DirectoryInfo($"{Environment.CurrentDirectory}\\.rds");
            var DataFileInfos = rdsinfo.EnumerateFiles($"*{ext}").
                Select(x => new DataFileInfo().LoadFromFile(x.FullName)).
                Where(x => rg.IsMatch(x.Info.Name));
            return DataFileInfos;
        }

        private void modeAddFile(string[] obj)
        {
            //correct use: rds File -AddFile {filepath} [delim]
            if (obj.Length < 3 || obj.Length > 4)
            {
                Console.Error.WriteLine("Correct usage: rds File -AddFile {filePath} [delimiter]");
                return;
            }
            string path = obj[2];
            if (!File.Exists(path))
            {
                Console.Error.WriteLine($"Could not find file {path}");
                return;
            }
            FileInfo inf = new FileInfo(path);
            DataFileInfo DFInfo = new DataFileInfo(inf);
            if (obj.Length == 4)
            {
                DFInfo.Delimiter = obj[3];
                ReDetermineColumns(DFInfo);
            }
            Save(DFInfo);
            Console.Out.WriteLine($"Added file {path}.\r\nDelimiter: {DFInfo.Delimiter}");
        }

        private void modeAddTable(string[] obj)
        {
            if (obj.Length != 5)
            {
                Console.Error.WriteLine("Correct usage: rds File -AddTable {Schemaname} {TableName} {ConnectionAlias}");
                return;
            }
            //verify that a correct ConnectionAlias has been provided
            string alias = obj[4];
            ConnectionAliases aliases = new ConnectionAliases();
            if (!aliases.ContainsKey(alias))
            {
                Console.Error.WriteLine($"Could not locate connection alias by name of {alias}. Use rds connection -Add {{alias}} {{connectionstring}} to add an alias");
                return;
            }

            SourceTableInfo source = new SourceTableInfo(aliases[alias], obj[2], obj[3]);
            string filename = $"{Environment.CurrentDirectory}\\.rds\\{source.SourceName}{Settings.DataSourceFileExtension}";
            source.SaveToFile(filename);
        }

        private void modeMask(string[] obj)
        {
            throw new NotImplementedException();
        }

        private void modeSetDelimiter(string[] obj)
        {
            //correct usage: rds File -d {filename} {delim}. if -a is given instead of filename
            //set given delimter for ALL files. filename can be regex pattern
            if (obj.Length != 4)
            {
                Console.Error.WriteLine("Correct usage: rds File -d {filenameRegex | -a} {delim}");
                return;
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

        private void ReDetermineColumns(DataFileInfo f)
        {
            using (StreamReader reader = new StreamReader(f.Info.FullName))
            {
                string firstLine = reader.ReadLine();
                f.Columns = firstLine.Delimit(f.Delimiter, null);
            }
        }

        private void Save(DataFileInfo f)
        {
            string ext = Properties.Settings.DataSourceFileExtension;
            string settingsdir = $"{Environment.CurrentDirectory}\\.rds";
            int idx = f.Info.Name.LastIndexOf('.');
            string nameWithoutExtension = f.Info.Name.Substring(0, idx);
            f.SaveToFile($"{settingsdir}\\{nameWithoutExtension}{ext}");
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

        #endregion Private Methods
    }
}