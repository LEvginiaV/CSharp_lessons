using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Models.Workers;

namespace Market.CustomersAndStaff.Repositories.Interface
{
    public interface IWorkerRepository
    {
        Task<Worker> CreateAsync(Guid shopId, Worker worker);
        Task<Worker[]> CreateManyAsync(Guid shopId, Worker[] workers);
        Task UpdateAsync(Guid shopId, Guid workerId, Action<Worker> update);
        Task<Worker> ReadAsync(Guid shopId, Guid workerId);
        Task<Worker[]> ReadByShopAsync(Guid shopId, bool includeDeleted = false);
        IEnumerable<Worker> ReadAll();

        Task<int> GetVersionAsync(Guid shopId);
    }
}