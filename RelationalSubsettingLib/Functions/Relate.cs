using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RelationalSubsettingLib.Functions
{
    public class Relate
    {
        #region Public Methods

        public void Run(string primaryFile, string primaryKey, string foreignFile, string foreignKey)
        {
            string keyrelationFileName = Properties.Settings.KeyRelationshipFileName;
            string strRdsDir = Environment.CurrentDirectory + "\\.rds";
            DirectoryInfo rdsDirInfo = new DirectoryInfo(strRdsDir);
            string ext = Properties.Settings.DataSourceFileExtension;
            var files = rdsDirInfo.EnumerateFiles().Where(f => f.Extension.Contains(ext));
            List<DataSourceInformation> listOfInformation = new List<DataSourceInformation>();
            foreach (var file in files)
            {
                listOfInformation.Add(DataSourceInformation.LoadFromFile(file.FullName));
            }
            //check of both sources have their info defined in the rds directory and have the required column
            var primary = listOfInformation.FirstOrDefault(
                sauce => sauce.SourceName.Equals(primaryFile));
            var foreign = listOfInformation.FirstOrDefault(
                sauce => sauce.SourceName.Equals(foreignFile));
            bool IsInErrorState = false;
            if (primary == null)
            {
                IsInErrorState = true;
                Console.Error.WriteLine($"{primaryFile} could not be found in the rds directory, either the file is missing or is not initialized");
            }
            else if (!primary.Columns.Contains(primaryKey))
            {
                IsInErrorState = true;
                Console.Error.WriteLine($"{primaryFile} was found but does not contain column {primaryKey}.");
            }
            if (foreign == null)
            {
                IsInErrorState = true;
                Console.Error.WriteLine($"{foreignFile} could not be found in the rds directory, either the file is missing or is not initialized");
            }
            else if (!foreign.Columns.Contains(foreignKey))
            {
                IsInErrorState = true;
                Console.Error.WriteLine($"{foreignFile} was found but does not contain column {foreignKey}.");
            }

            if (IsInErrorState)
            {
                Console.Error.WriteLine("An error was encountered and the program could not continue");
                return;
            }
            //TODO: add validation (primary key specified is not a foreign key set in a different relationship or other nonsense)
            //TODO: add method to clean up keyrelationfile without resetting the whole repo
            Key prim = new Key(primaryFile, primaryKey);
            Key fore = new Key(foreignFile, foreignKey);
            KeyRelationship rel = new KeyRelationship(prim, fore);
            List<KeyRelationship> keyRelationships = new List<KeyRelationship>().LoadFromFile($"{strRdsDir}\\{keyrelationFileName}");
            if (keyRelationships.Contains(value: rel))
            {
                Console.Error.WriteLine("This relationship is already defined");
                return;
            }
            keyRelationships.Add(rel);
            keyRelationships.SaveToFile($"{strRdsDir}\\keyrelations.rdskrf");
            Console.Out.WriteLine($"Relationship set. Primary key: \"{primaryKey}\" in file \"{primaryFile}\". Foreign key: \"{foreignKey}\" in file \"{foreignFile}\"");
            return;
        }

        #endregion Public Methods
    }
}