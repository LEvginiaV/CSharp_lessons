using System;

using Microsoft.AspNetCore.Routing;

namespace Market.CustomersAndStaff.FrontApi.Filters.Extensions
{
    public static class RouteDataExtensions
    {
        public static Guid? GetGuidFromRouteDataParameter(this RouteData routeData, string paramName)
        {
            if(!routeData.Values.TryGetValue(paramName, out var rawResult))
            {
                return null;
            }

            if(!(rawResult is string stringResult))
            {
                return null;
            }

            return Guid.TryParse(stringResult, out var result) ? result : (Guid?)null;
        }
    }
}