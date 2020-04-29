using System;
using System.Collections.Generic;
using System.Text;

namespace RelationalSubsettingLib.Functions
{
    public class DEBUG_Currentdir
    {
        public void Run()
        {
            Console.Out.WriteLine(Environment.CurrentDirectory);
            return;

        }
    }
}
