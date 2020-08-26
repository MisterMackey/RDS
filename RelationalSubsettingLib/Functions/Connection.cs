using RelationalSubsettingLib.Sql;
using System;
using System.Collections.Generic;
using System.Text;

namespace RelationalSubsettingLib.Functions
{
    public class Connection
    {
        private Dictionary<string, Action<string, string>> ModeMapping = new Dictionary<string, Action<string, string>>();

        public Connection()
        {
            ModeMapping.Add("-ADD", ModeAdd);
            ModeMapping.Add("-REMOVE", ModeRemove);
            ModeMapping.Add("-UPDATE", ModeUpdate);
        }
        public void Run(string[] obj)
        {
            if (obj.Length == 2 && obj[1].ToUpper() == "-LIST")
            {
                ModeList();
                return;
            }
            if (obj.Length < 4)
            {
                Console.Error.WriteLine("Not enough arguments supplied: correct usage: rds Connection {mode(-add/-remove/-update)} {alias} {connstring}");
                return;
            }
            string mode = obj[1].ToUpper(); //rds connection [mode] ----
            string arg1 = obj[2];
            string arg2 = obj[3];
            ModeMapping[mode](arg1, arg2);
        }

        private void ModeAdd(string alias, string connectionstring)
        {
            ConnectionAliases al = new ConnectionAliases();
            try
            {
                al.Add(alias, connectionstring);
            }
            catch
            {
                Console.Error.WriteLine("Alias already exists, try connection -Update instead");
                return;
            }
            Console.Out.WriteLine($"added alias with name {alias}");
        }
        private void ModeRemove(string alias, string x)
        {
            ConnectionAliases al = new ConnectionAliases();
            al.Remove(alias);
            Console.Out.WriteLine($"removed alias with name {alias}");
        }
        private void ModeUpdate(string alias, string connectionstring)
        {
            ConnectionAliases al = new ConnectionAliases();
            if (!al.ContainsKey(alias))
            {
                Console.Error.WriteLine($"Key {alias} was not found");
                return;
            }
            al[alias] = connectionstring;
            Console.Out.WriteLine($"Key {alias} was updated");
        }
        private void ModeList()
        {
            ConnectionAliases al = new ConnectionAliases();
            foreach (var key in al.Keys)
            {
                Console.Out.WriteLine($"AliasName: {key}. Connectionstring: {al[key]}");
            }
        }
        
    }
}
