using RelationalSubsettingLib;
using System;
using System.Linq;

namespace RelationalDataSubsetting
{
    internal class Program
    {
        #region Private Methods

        private static int Main(string[] args)
        {
            if (!args.Any())
            {
                Console.Out.WriteLine("Yes hello this is console App. Try help to get help on cli arguments");
                return 0;
            }
            else
            {
                ParameterParser parser = new ParameterParser();
                parser.ParseArgument(args);
            }
            return 0;
        }

        #endregion Private Methods
    }
}