using Newtonsoft.Json.Serialization;
using RelationalSubsettingLib.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RelationalSubsettingLib.Functions
{
    /// <summary>
    /// this class will handle a variety of stuff to do with masking information in the files that are the be subsetted.
    /// Although masking is applied on sources i've made this into a seperate file in order to keep the RdsFile class a
    /// bit mor efocused.
    /// </summary>
    public class Mask
    {
        #region Private Fields

        private Dictionary<string, Action<string[]>> m_ModeMapping;

        #endregion Private Fields

        #region Public Constructors

        public Mask()
        {
            m_ModeMapping = new Dictionary<string, Action<string[]>>()
            {
                {"-ADD", modeApply}
                ,{"-REMOVE", modeRemove }
            };
        }

        #endregion Public Constructors

        #region Public Methods

        public void Run(string[] args)
        {
            //gonna experiment with stripping the unecessary things out of the argument list
            //usage : rds Mask {mode} [some options]
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Mode not specified, correct usage: rds Mask {mode} [options].");
                return;
            }
            string mode = args[1].ToUpper();
            string[] options = args.Skip(2).ToArray();
            m_ModeMapping[mode](options);
        }

        #endregion Public Methods

        #region Private Methods

        private void modeApply(string[] obj)
        {
            //required options: name of source (filename or schema.table), the column to apply it on, and the type of mask to apply (must be defined in the maskingoptions enum in the enumerations file)
            //optional option: a method to use with the masking strategy, check the actual maskingstrategies to see how the method is used (for replace the 'method' is simply the string value to use in the replacement action
            if (!obj.Length.Between(3, 4))
            {
                Console.Error.WriteLine("Argument error, correct usage: rds mask -ADD {sourcename} {masktype} [method]");
            }
            string filename = obj[0];
            string colName = obj[1];
            string type = obj[2];
            string method = obj.ElementAtOrDefault(3); //null values will be passed on, this is intended
            MaskingOptions typeEnum = (MaskingOptions)Enum.Parse(typeof(MaskingOptions), type, ignoreCase: true);
            Tuple<MaskingOptions, string> dictValue = new Tuple<MaskingOptions, string>(typeEnum, method);

            var dirinf = new DirectoryInfo($"{Directory.GetCurrentDirectory()}\\{Settings.RdsDirectoryName}");
            var sourceInf = dirinf.EnumerateFiles().Where(
                file => file.Name == filename).FirstOrDefault();
            if (sourceInf == null)
            {
                Console.Error.Write($"File with name {filename} could not be found.");
                return;
            }
            DataSourceInformation dsi = DataSourceInformation.LoadFromFile(sourceInf.FullName);
            dsi.MaskingInformation.Add(colName, dictValue);
            dsi.SaveToFile(sourceInf.FullName);
        }

        private void modeRemove(string[] obj)
        {
            //required option: name of source and the column name
            if (!(obj.Length == 2))
            {
                Console.Error.WriteLine("Argument error, correct usage: rds mask -remove {sourcename} {columnname}");
                return;
            }
            string filename = obj[0];
            string colName = obj[1];
            var dirinf = new DirectoryInfo($"{Directory.GetCurrentDirectory()}\\{Settings.RdsDirectoryName}");
            var sourceInf = dirinf.EnumerateFiles().Where(
                file => file.Name == filename).FirstOrDefault();
            if (sourceInf == null)
            {
                Console.Error.Write($"File with name {filename} could not be found.");
                return;
            }
            DataSourceInformation dsi = DataSourceInformation.LoadFromFile(sourceInf.FullName);
            dsi.MaskingInformation.Remove(colName);
            dsi.SaveToFile(sourceInf.FullName);
        }

        #endregion Private Methods
    }
}