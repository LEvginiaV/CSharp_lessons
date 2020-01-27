using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Market.CustomersAndStaff.ServiceApi.Client.Core
{
    internal static class DtoJsonSerializerSettings
    {
        static DtoJsonSerializerSettings()
        {
            Settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
#pragma warning disable 618
                    Converters = new JsonConverter[] {new StringEnumConverter(true)}
#pragma warning restore 618
                };
        }

        public static JsonSerializerSettings Settings { get; }
    }
}