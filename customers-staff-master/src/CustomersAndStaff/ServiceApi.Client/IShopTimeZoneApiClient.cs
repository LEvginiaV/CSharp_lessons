using System;
using System.Threading.Tasks;

namespace Market.CustomersAndStaff.ServiceApi.Client
{
    public interface IShopTimeZoneApiClient
    {
        Task<TimeSpan?> Get(Guid shopId);
    }
}