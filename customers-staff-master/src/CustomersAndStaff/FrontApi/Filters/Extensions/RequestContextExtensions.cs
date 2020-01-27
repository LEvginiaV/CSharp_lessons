using Microsoft.AspNetCore.Http;

namespace Market.CustomersAndStaff.FrontApi.Filters.Extensions
{
    public static class RequestContextExtensions
    {
        public static string GetPortalSid(this HttpRequest request)
        {
            return request.Cookies.TryGetValue("auth.sid", out var authSid) ? authSid : null;
        }
    }
}