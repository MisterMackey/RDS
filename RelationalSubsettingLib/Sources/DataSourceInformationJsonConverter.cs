using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RelationalSubsettingLib.Sources;
using System;
using System.Collections.Generic;
using System.Text;

namespace RelationalSubsettingLib
{
    public class DataSourceInformationJsonConverter : JsonConverter
    {
        static JsonSerializerSettings CustomSerializerSettings = new JsonSerializerSettings() { ContractResolver = new DataSourceInformationJsonConverterContractResolver() };

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(DataSourceInformation));
        }

        
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            string ctype = jObject["ConcreteType"].Value<string>();
            switch (ctype)
            {
                case "DataFileInfo":
                    var dfi = JsonConvert.DeserializeObject<DataFileInfo>(jObject.ToString(), CustomSerializerSettings);
                    return dfi;
                case "SourceTableInfo":
                    var sti = JsonConvert.DeserializeObject<SourceTableInfo>(jObject.ToString(), CustomSerializerSettings);
                    return sti;
                default:
                    throw new FormatException($"{jObject["ConcreteType"].Value<string>()} is not a known format for a data source");
            }
        }
        public override bool CanWrite => false; //somehow this should not matter

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); 
        }
    }

    //putting these together since they belong together
    public class DataSourceInformationJsonConverterContractResolver : DefaultContractResolver //holy .... is this a java class?
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof(DataSourceInformation).IsAssignableFrom(objectType) && !objectType.IsAbstract)
            {
                return null; //this prevents a stackoverflow when serializing/deserializing the abstract base class
            }
            return base.ResolveContractConverter(objectType);
        }
    }
}
