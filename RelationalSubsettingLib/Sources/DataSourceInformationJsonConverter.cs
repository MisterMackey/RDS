using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RelationalSubsettingLib.Sources;
using System;

namespace RelationalSubsettingLib
{
    public class DataSourceInformationJsonConverter : JsonConverter
    {
        #region Public Properties

        public override bool CanWrite => false;

        #endregion Public Properties

        #region Private Fields

        private static JsonSerializerSettings CustomSerializerSettings = new JsonSerializerSettings() { ContractResolver = new DataSourceInformationJsonConverterContractResolver() };

        #endregion Private Fields

        #region Public Methods

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

        //somehow this should not matter

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }

    //putting these together since they belong together
    public class DataSourceInformationJsonConverterContractResolver : DefaultContractResolver //holy .... is this a java class?
    {
        #region Protected Methods

        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof(DataSourceInformation).IsAssignableFrom(objectType) && !objectType.IsAbstract)
            {
                return null; //this prevents a stackoverflow when serializing/deserializing the abstract base class
            }
            return base.ResolveContractConverter(objectType);
        }

        #endregion Protected Methods
    }
}