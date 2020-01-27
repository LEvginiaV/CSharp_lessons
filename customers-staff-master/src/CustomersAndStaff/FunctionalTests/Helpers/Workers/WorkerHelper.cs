using System;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.Repositories.Interface;

namespace Market.CustomersAndStaff.FunctionalTests.Helpers.Workers
{
    public class WorkerHelper : IWorkerHelper
    {
        public WorkerHelper(IWorkerRepository workerRepository)
        {
            this.workerRepository = workerRepository;
        }

        public async Task<Worker> CreateAsync(string name)
        {
            return await workerRepository.CreateAsync(ContextHelper.GetCurrentShopId(), new Worker {FullName = name});
        }

        public async Task UpdateAsync(Guid workerId, Action<Worker> update)
        {
            await workerRepository.UpdateAsync(ContextHelper.GetCurrentShopId(), workerId, update);
        }

        public async Task RemoveAsync(Guid workerId)
        {
            await workerRepository.UpdateAsync(ContextHelper.GetCurrentShopId(), workerId, worker => worker.IsDeleted = true);
        }

        private readonly IWorkerRepository workerRepository;
    }
}