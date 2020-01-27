using System.Net;
using System.Text;

using Kontur.Clusterclient.Core.Model;

namespace Market.CustomersAndStaff.ServiceApi.Client.Core
{
    public static class ClusterResultExtensions
    {
        public static string GetContentAsUtf8String(this ClusterResult result)
        {
            return Encoding.UTF8.GetString(result.Response.Content.ToArray());
        }

        public static void CheckResponseSuccessful(this ClusterResult result)
        {
            if(!result.Response.IsSuccessful)
                throw new HttpResponseException((HttpStatusCode)(int)result.Response.Code, result.GetContentAsUtf8String());
        }
    }
}