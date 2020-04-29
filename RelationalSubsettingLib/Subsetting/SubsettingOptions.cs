using System;
using System.Collections.Generic;
using System.Text;

namespace RelationalSubsettingLib.Subsetting
{
    public class SubsettingOptions
    {
        public string TargetPath { get; set; }
        public string BaseFileName { get; set; }
        public string BaseColumn { get; set; }
        public double Factor { get; set; }
    }
}
