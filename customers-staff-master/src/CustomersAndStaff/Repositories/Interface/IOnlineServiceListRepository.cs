using System;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Models.OnlineRecording;

namespace Market.CustomersAndStaff.Repositories.Interface
{
    public interface IOnlineServiceListRepository
    {
        Task<OnlineService[]> ReadAsync(Guid shopId);
        Task WriteAsync(Guid shopId, OnlineService[] services);
    }
}