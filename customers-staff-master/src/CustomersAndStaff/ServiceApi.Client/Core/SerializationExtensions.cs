using System.Text;

using Kontur.Clusterclient.Core.Model;

using Newtonsoft.Json;

namespace Market.CustomersAndStaff.ServiceApi.Client.Core
{
    public static class SerializationExtensions
    {
        internal static T DeserializeDto<T>(this ClusterResult result)
        {
            result.CheckResponseSuccessful();
            return JsonConvert.DeserializeObject<T>(result.GetContentAsUtf8String(), DtoJsonSerializerSettings.Settings);
        }

        internal static byte[] SerializeDto<T>(this T dto)
        {
            var serializeObject = JsonConvert.SerializeObject(dto, DtoJsonSerializerSettings.Settings);
            return Encoding.UTF8.GetBytes(serializeObject);
        }
    }
}