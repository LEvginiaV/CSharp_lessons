using System;
using System.Threading.Tasks;

namespace Market.CustomersAndStaff.Repositories.Interface
{
    public interface ITimeZoneSettingsRepository
    {
        Task SetAsync(Guid shopId, Guid timeZoneId);
        Task<Guid?> GetAsync(Guid shopId);
    }
}