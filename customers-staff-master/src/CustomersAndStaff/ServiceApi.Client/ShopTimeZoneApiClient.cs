using System;
using System.Threading.Tasks;

using Kontur.Clusterclient.Core.Model;

using Market.CustomersAndStaff.ServiceApi.Client.Core;

namespace Market.CustomersAndStaff.ServiceApi.Client
{
    public class ShopTimeZoneApiClient : IShopTimeZoneApiClient
    {
        public ShopTimeZoneApiClient(ISimpleClusterClient clusterClient)
        {
            this.clusterClient = clusterClient;
        }

        public async Task<TimeSpan?> Get(Guid shopId)
        {
            var result = await clusterClient.SendAsync(
                             Request.Get(Url(shopId))
                         );
            return result.DeserializeDto<TimeSpan?>();
        }

        private static Uri Url(Guid shopId)
        {
            return new RequestUrlBuilder()
                   .AppendToPath(shopId)
                   .AppendToPath("timeZone")
                   .Build();
        }

        private readonly ISimpleClusterClient clusterClient;
    }
}