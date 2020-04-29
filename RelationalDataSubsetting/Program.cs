using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RelationalSubsettingLib;
using System.Text;
using System.Threading.Tasks;

namespace RelationalDataSubsetting
{
    class Program
    {
        static int Main(string[] args)
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
    }

}
