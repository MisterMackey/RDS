using System;
using System.Collections.Generic;
using System.Text;

namespace RelationalSubsettingLib
{
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    sealed class RequiresInitializedRepositoryAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        

        // This is a positional argument
        public RequiresInitializedRepositoryAttribute()
        {
           
        }

    }

    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    sealed class ValidOptionsAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236

        public CommandOptions ValidOptions;
        // This is a positional argument
        public ValidOptionsAttribute(CommandOptions validOptions)
        {
            ValidOptions = validOptions;
        }

    }
}
