using System;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Models.Workers;

namespace Market.CustomersAndStaff.FunctionalTests.Helpers.Workers
{
    public interface IWorkerHelper
    {
        Task<Worker> CreateAsync(string name);
        Task UpdateAsync(Guid workerId, Action<Worker> update);
        Task RemoveAsync(Guid workerId);
    }
}