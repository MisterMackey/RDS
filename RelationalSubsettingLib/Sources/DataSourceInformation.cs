using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RelationalSubsettingLib
{
    [JsonConverter(typeof(DataSourceInformationJsonConverter))]
    public abstract class DataSourceInformation
    {
        //required as the static extension requires an initialized object, which is one thing an abstract class won't give you...
        public static DataSourceInformation LoadFromFile(string filePath)
        {
            string txt = File.ReadAllText(filePath);
            DataSourceInformation ret = JsonConvert.DeserializeObject<DataSourceInformation>(txt);
            return ret;
        }
        public abstract string ConcreteType { get; }
    }
}
