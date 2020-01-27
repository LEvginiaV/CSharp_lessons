using Microsoft.AspNetCore.Routing;

namespace Market.CustomersAndStaff.OnlineApi.Filters.Extensions
{
    public static class RouteDataExtensions
    {
        public static string GetParam(this RouteData routeData, string paramName)
        {
            if (!routeData.Values.TryGetValue(paramName, out var rawResult))
            {
                return null;
            }

            if (!(rawResult is string stringResult))
            {
                return null;
            }

            return stringResult;
        }
    }
}