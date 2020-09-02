using System;
using System.Collections.Generic;
using System.Text;

namespace RelationalSubsettingLib
{
    
    public enum CommandOptions
    {
        Force,
        Create,
        Delete,
        Factor,
        SetBase,
        Help,
        SetDelimiter,
        d,
        a,
        All,
        AddFile,
        AddTable,
        Add,
        Remove,
        Update,
        List
    }

    public enum MaskingOptions
    {
        Replace, //put a static value in place of old values
        Randomize, //assign random values from a list (list generated from the actual dataset)
        Generate //have some algorithm to make brand new values
    }
}
