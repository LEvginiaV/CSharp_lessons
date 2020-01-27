using System;
using System.Collections.Generic;
using System.Linq;

namespace Market.CustomersAndStaff.FrontApi.Filters.Extensions
{
    public static class PortalStringExtensions
    {
        public static string[] ParsePortalStringsList(this string portalString)
        {
            return (portalString ?? string.Empty).Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);
        }

        public static Guid[] ParsePortalGuidsSet(this string portalString)
        {
            var guidsSet = new HashSet<Guid>();
            foreach(var guidString in portalString.ParsePortalStringsList())
            {
                if(Guid.TryParse(guidString, out var guid))
                    guidsSet.Add(guid);
            }

            return guidsSet.ToArray();
        }
    }
}