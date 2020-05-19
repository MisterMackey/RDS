using RelationalSubsettingLib.Subsetting;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;

namespace RelationalSubsettingLib.Functions
{
    public class Subset
    {
        private Dictionary<string, Action<string[]>> ModeMapping;
        public Subset()
        {
            ModeMapping = new Dictionary<string, Action<string[]>>()
            {
                {"-CREATE",  Create},
                {"-FACTOR", Factor },
                {"-SETBASE", Setbase },
                {"-HELP", Halp }
            };
        }



        public void Run(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("The subset command requires a mode. Use -help for usage");
                return;
            }
            string mode = args[1].ToUpper();
            if (ModeMapping.ContainsKey(mode))
            {
                ModeMapping[mode](args);
            }
            else
            {
                Console.Error.WriteLine($"{args[1]} is not recognized as a valid mode for the subset command");
            }
        }

        private void Create(string[] options)
        {
            string path;
            FileInfo settingsFile = new FileInfo($"{Environment.CurrentDirectory}\\.rds\\settings.rdssf");
            SubsettingOptions ssOptions = new SubsettingOptions().LoadFromFile(settingsFile.FullName);
            if (options.Length > 3) //wut?
            {
                Console.Error.WriteLine("error: too many arguments");
                return;
            }
            if (options.Length == 3) //path specfied?
            {
                path = options[2];
                if (!Directory.Exists(path))
                {
                    Console.Error.WriteLine($"Path not found: {path}");
                    return;
                }
                ssOptions.TargetPath = path;
                ssOptions.SaveToFile(settingsFile.FullName);
            }
                        
            Subsetter s = new Subsetter(ssOptions);
            s.CreateSubsetAsync().Wait();
        }

        private void Setbase(string[] obj)
        {
            FileInfo settingsFile = new FileInfo($"{Environment.CurrentDirectory}\\.rds\\settings.rdssf");
            SubsettingOptions options = new SubsettingOptions().LoadFromFile(settingsFile.FullName);
            if (obj.Length != 4)//usage: subset setbase file column
            {
                Console.Error.WriteLine("Incorrect amount of arguments specified. see help for usage");
                return;
            }
            string file = obj[2];
            string column = obj[3];
            if (!IsValidInputForSetbase(file, column))
            {
                return;
            }
            options.BaseColumn = column;
            options.BaseFileName = file;
            options.SaveToFile(settingsFile.FullName);
            Console.Out.WriteLine($"Base set to column {column} in {file}");
            return;

        }

        private bool IsValidInputForSetbase(string file, string column)
        {
            string ext = Properties.Settings.DataSourceFileExtension;
            DirectoryInfo rdsdir = new DirectoryInfo($"{Environment.CurrentDirectory}\\.rds");
            var datafileinfo = rdsdir.EnumerateFiles(). //all files
                Where(x => x.Extension.Equals(ext)). //with rds datafileinfo extension
                Select(x => new DataFileInfo().LoadFromFile(x.FullName)). //select the datafileinfo objects that are loaded from those files
                Where(x => x.Info.Name.Equals(file)). //where the original name equals the supplied name
                FirstOrDefault(); //first one or null

            if (datafileinfo == null)
            {
                Console.Error.WriteLine($"{file} could not be found in the rds repository");
                return false;
            }
            if (!datafileinfo.Columns.Contains(column))
            {
                Console.Error.WriteLine($"{column} could not be found in {file}");
                return false;
            }
            return true;

        }

        private void Factor(string[] obj)
        {
            FileInfo settingsFile = new FileInfo($"{Environment.CurrentDirectory}\\.rds\\settings.rdssf");
            SubsettingOptions options = new SubsettingOptions().LoadFromFile(settingsFile.FullName);
            if (obj.Length != 3) //subset factor double
            {
                Console.Error.Write("Incorrect amount of arguments specified. see help for usage");
                return;
            }
            double value;
            if (double.TryParse(obj[2], out value))
            {
                options.Factor = value;
                options.SaveToFile(settingsFile.FullName);
                Console.Out.Write($"Factor of subsetting set to {value}");
                return;
            }
            else
            {
                Console.Error.WriteLine($"Could not parse {obj[2]} to a double value.");
                return;
            }
        }

        private void Halp(string[] obj)
        {
            Console.Out.WriteLine("Usage: rds subset (-create/-factor/-setbase) [path/factor/basefileAndcolumn]");
        }
    }
}
