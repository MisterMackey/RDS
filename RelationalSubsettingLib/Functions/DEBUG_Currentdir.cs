using System;

namespace RelationalSubsettingLib.Functions
{
    public class DEBUG_Currentdir
    {
        #region Public Methods

        public void Run()
        {
            Console.Out.WriteLine(Environment.CurrentDirectory);
            return;
        }

        #endregion Public Methods
    }
}