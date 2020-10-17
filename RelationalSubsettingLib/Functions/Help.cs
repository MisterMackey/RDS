using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace RelationalSubsettingLib.Functions
{
    public class Help
    {
        #region Public Methods

        public void Run()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("RDS version information:");
            var assembly = typeof(Help).Assembly;
            sb.AppendLine(assembly.FullName);
            sb.AppendLine($"Running from {assembly.Location}");
            Console.Out.WriteLine(sb.ToString());
        }

        #endregion Public Methods
    }
}