using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Market.CustomersAndStaff.Repositories.Serializer
{
    public class JsonSerializer : ISerializer
    {
        public byte[] Serialize<T>(T obj)
        {
            var str = JsonConvert.SerializeObject(obj, settings);
            return Encoding.UTF8.GetBytes(str);
        }

        public T Deserialize<T>(byte[] bytes)
        {
            var str = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(str, settings);
        }

        private readonly JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new JsonConverter[] {new StringEnumConverter(true)},
                TypeNameHandling = TypeNameHandling.Auto,
            };
    }
}