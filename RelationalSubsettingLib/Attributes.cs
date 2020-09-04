using System;

namespace RelationalSubsettingLib
{
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    internal sealed class RequiresInitializedRepositoryAttribute : Attribute
    {
        // See the attribute guidelines at http://go.microsoft.com/fwlink/?LinkId=85236

        #region Public Constructors

        // This is a positional argument
        public RequiresInitializedRepositoryAttribute()
        {
        }

        #endregion Public Constructors
    }

    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    internal sealed class ValidOptionsAttribute : Attribute
    {
        // See the attribute guidelines at http://go.microsoft.com/fwlink/?LinkId=85236

        #region Public Fields

        public CommandOptions ValidOptions;

        #endregion Public Fields

        #region Public Constructors

        // This is a positional argument
        public ValidOptionsAttribute(CommandOptions validOptions)
        {
            ValidOptions = validOptions;
        }

        #endregion Public Constructors
    }
}