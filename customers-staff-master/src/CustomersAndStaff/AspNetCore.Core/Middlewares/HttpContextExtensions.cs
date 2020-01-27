using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Market.CustomersAndStaff.AspNetCore.Core.Middlewares
{
    public static class HttpContextExtensions
    {
        public static (string Controller, string Action) GetControllerAndAction(this HttpContext context)
        {
            var routeData = context.GetRouteData();
            return (TryGetValue(routeData, "controller"), TryGetValue(routeData, "action"));
        }

        private static string TryGetValue(RouteData routeData, string key)
        {
            if(routeData == null)
                return null;
            if(!routeData.Values.TryGetValue(key, out var res))
                return null;
            return res.ToString();
        }
    }
}