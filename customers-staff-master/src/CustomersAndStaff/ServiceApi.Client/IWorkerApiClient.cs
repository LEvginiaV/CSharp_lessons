using System;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Models.Workers;

namespace Market.CustomersAndStaff.ServiceApi.Client
{
    public interface IWorkerApiClient
    {
        Task<Worker> CreateAsync(Guid shopId, Worker worker);
        Task<Worker[]> CreateManyAsync(Guid shopId, Worker[] workers);
        Task UpdateAsync(Guid shopId, Guid workerId, Worker worker);
        Task<Worker> ReadAsync(Guid shopId, Guid workerId);
        Task<Worker[]> ReadByOrganizationAsync(Guid shopId, bool includeDeleted = false);
        Task<int> GetVersionAsync(Guid shopId);
    }
}