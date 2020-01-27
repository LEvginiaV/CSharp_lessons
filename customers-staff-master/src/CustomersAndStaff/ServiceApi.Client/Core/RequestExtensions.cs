using Kontur.Clusterclient.Core.Model;

namespace Market.CustomersAndStaff.ServiceApi.Client.Core
{
    public static class RequestExtensions
    {
        public static Request WithContentTypeApplicationJsonHeader(this Request request)
        {
            return request.WithContentTypeHeader("application/json");
        }
    }
}