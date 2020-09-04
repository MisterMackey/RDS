using RelationalSubsettingLib.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RelationalSubsettingLib
{
    public class ParameterParser
    {
        #region Private Fields

        private Dictionary<string, Action<string[]>> ParameterMapping;

        #endregion Private Fields

        #region Public Constructors

        public ParameterParser()
        {
            ParameterMapping = new Dictionary<string, Action<string[]>>()
            {
                {"HELP", paramHELP }
                ,{"DEBUG_CURRENTDIRECTORY", paramDEBUG_CURRENTDIR}
                ,{"INIT", paramINIT}
                ,{"CLEAN", paramCLEAN}
                ,{"LIST", paramLIST}
                ,{"STATUS", paramSTATUS}
                ,{"RELATE", paramRELATE}
                ,{"DEBUG_DUMPFILEINFO", paramDEBUG_DUMPFILEINFO}
                ,{"DEBUG_DUMPRELATIONINFO", paramDEBUG_DUMPRELATIONINFO}
                ,{"SUBSET", paramSUBSET}
                ,{"FILE", paramFILE}
                ,{"CONNECTION", paramCONNECTION}
                ,{"MASK", paramMask}
            };
        }

        #endregion Public Constructors

        #region Public Methods

        public void ParseArgument(string[] args)
        {
            string param = args[0].ToUpper();
            if (param[0].Equals('/') || param[0].Equals('\\'))
            {
                Console.Out.WriteLine($"This isn't win32, stop putting slashes on stuff bruv");
            }
            if (!ParameterMapping.ContainsKey(param))
            {
                Console.Out.WriteLine($"{param} is not recognized as a valid command.");
                return;
            }
            else
            {
                MethodInfo info = ParameterMapping[param].Method;
                if (IsValidRepositoryState(info) && IsValidOptions(info, args))
                {
                    ParameterMapping[param](args);
                    return;
                }
                else
                {
                    Console.Error.WriteLine("Error while executing. Repository not initialized or invalid arguments.");
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static void paramCLEAN(string[] obj)
        {
            Clean clean = new Clean();
            clean.Run();
        }

        private static void paramDEBUG_CURRENTDIR(string[] inf)
        {
            DEBUG_Currentdir dir = new DEBUG_Currentdir();
            dir.Run();
        }

        private static void paramHELP(string[] inf)
        {
            Help help = new Help();
            help.Run();
        }

        private static void paramINIT(string[] obj)
        {
            Init init = new Init();
            init.Run();
        }

        [RequiresInitializedRepository()]
        private static void paramLIST(string[] obj)
        {
            throw new NotImplementedException();
        }

        [RequiresInitializedRepository()]
        private static void paramRELATE(string[] args)
        {
            if (args.Count() != 5) //the relate command, primary file + key, foreign file + key
            {
                Console.Out.WriteLine("incorrect usage of relate. Correct usage: rds relate primaryfile.txt primarykeycolumn foreignfile.txt foreignkeycolumn");
            }
            else
            {
                Relate r = new Relate();
                r.Run(args[1], args[2], args[3], args[4]);
            }
        }

        [RequiresInitializedRepository()]
        private static void paramSTATUS(string[] obj)
        {
            throw new NotImplementedException();
        }

        private bool IsInitialized()
        {
            return Directory.Exists($"{Environment.CurrentDirectory}\\.rds");
        }

        private bool IsValidOptions(MethodInfo method, string[] args)
        {
            IEnumerable<ValidOptionsAttribute> validOptionsAttributes = (IEnumerable<ValidOptionsAttribute>)method.GetCustomAttributes(typeof(ValidOptionsAttribute));
            var validOptions = validOptionsAttributes.Select(x => x.ValidOptions);
            IEnumerable<string> passedOptions = args.Where(x => x.StartsWith("-"));
            foreach (string passedOption in passedOptions)
            {
                string optionWithoutMinusSign = passedOption.Substring(1);
                CommandOptions commandOption;
                if (Enum.TryParse(optionWithoutMinusSign, true, out commandOption))
                {
                    if (validOptions.Any(x => x.Equals(commandOption)))
                    {
                        continue;
                    }
                    else
                    {
                        Console.Error.WriteLine($"{optionWithoutMinusSign} is not a valid option for this command");
                        return false;
                    }
                }
                else
                {
                    Console.Error.WriteLine($"{optionWithoutMinusSign} is not a valid option.. at all");
                    return false;
                }
            }
            return true;
        }

        private bool IsValidRepositoryState(MethodInfo method)
        {
            var attributes = method.CustomAttributes;
            if (attributes.Any(
                att => att.AttributeType == typeof(RequiresInitializedRepositoryAttribute)))
            {
                return IsInitialized();
            }
            else
            {
                return true;
            }
        }

        [RequiresInitializedRepository(),
            ValidOptions(CommandOptions.Add),
            ValidOptions(CommandOptions.Remove),
            ValidOptions(CommandOptions.Update),
            ValidOptions(CommandOptions.List)]
        private void paramCONNECTION(string[] obj)
        {
            Connection c = new Connection();
            c.Run(obj);
        }

        [RequiresInitializedRepository()]
        private void paramDEBUG_DUMPFILEINFO(string[] obj)
        {
            DEBUG_DUMPFILEINFO d = new DEBUG_DUMPFILEINFO();
            d.Run();
        }

        [RequiresInitializedRepository()]
        private void paramDEBUG_DUMPRELATIONINFO(string[] obj)
        {
            DEBUG_DUMPRELATIONINFO d = new DEBUG_DUMPRELATIONINFO();
            d.Run();
        }

        [RequiresInitializedRepository(),
            ValidOptions(CommandOptions.d),
            ValidOptions(CommandOptions.SetDelimiter),
            ValidOptions(CommandOptions.a),
            ValidOptions(CommandOptions.All),
            ValidOptions(CommandOptions.AddFile),
            ValidOptions(CommandOptions.AddTable)]
        private void paramFILE(string[] obj)
        {
            RdsFile rdsFile = new RdsFile();
            rdsFile.Run(obj);
        }

        [RequiresInitializedRepository(),
            ValidOptions(CommandOptions.Add),
            ValidOptions(CommandOptions.Remove)]
        private void paramMask(string[] obj)
        {
            Mask m = new Mask();
            m.Run(obj);
        }

        [RequiresInitializedRepository()
            , ValidOptions(CommandOptions.Create)
            , ValidOptions(CommandOptions.Factor)
            , ValidOptions(CommandOptions.SetBase)
            , ValidOptions(CommandOptions.Help)]
        private void paramSUBSET(string[] obj)
        {
            Subset s = new Subset();
            s.Run(obj);
        }

        #endregion Private Methods
    }
}