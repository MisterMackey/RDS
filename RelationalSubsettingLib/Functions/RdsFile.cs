using System;
using System.Collections.Generic;
using System.Text;

namespace RelationalSubsettingLib.Functions
{
    /// <summary>
    /// corresponds to File param but i don't wanna call it File otherwise it must do battle
    /// against System.IO.File in the autocorrect
    /// </summary>
    public class RdsFile
    {
        private Dictionary<string, Action<string[]>> ModeMapping;
        public void Run(string[] args)
        {

        }
    }
}
