using System;

using Kontur.Clusterclient.Core.Model;

namespace Market.CustomersAndStaff.ServiceApi.Client.Extensions
{
    public static class RequestUrlBuilderExtensions
    {
        public static RequestUrlBuilder AppendDateToPath(this RequestUrlBuilder builder, DateTime date)
        {
            return builder.AppendToPath(date.Date.ToString("yyyy-MM-dd"));
        }
    }
}