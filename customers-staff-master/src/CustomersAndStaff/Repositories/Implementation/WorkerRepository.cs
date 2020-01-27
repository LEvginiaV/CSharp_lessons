using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Repositories.StoredModels;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.SessionTableQueryExtending.PrimitiveStoring;
using SKBKontur.Catalogue.CassandraUtils.DistributedLock.Locker;

namespace Market.CustomersAndStaff.Repositories.Implementation
{
    public class WorkerRepository : IWorkerRepository
    {
        public WorkerRepository(
            CassandraStorage<WorkerStorageElement> storage,
            IMapper mapper,
            ILocker locker,
            ICounterRepository counterRepository)
        {
            this.storage = storage;
            this.mapper = mapper;
            this.locker = locker;
            this.counterRepository = counterRepository;
        }

        public async Task<Worker> CreateAsync(Guid shopId, Worker worker)
        {
            using(await locker.LockAsync(GetLockId(shopId)).ConfigureAwait(false))
            {
                worker.ShopId = shopId;
                worker.Code = (await GetNextNumbersAsync(shopId, 1)).First();
                worker.Id = Guid.NewGuid();
                await storage.WriteAsync(ToStorageElement(worker)).ConfigureAwait(false);
                await IncrementVersion(shopId).ConfigureAwait(false);
                return worker;
            }
        }

        public async Task<Worker[]> CreateManyAsync(Guid shopId, Worker[] workers)
        {
            using(await locker.LockAsync(GetLockId(shopId)).ConfigureAwait(false))
            {
                var numbers = await GetNextNumbersAsync(shopId, workers.Length);
                workers = workers.Zip(numbers, (x, y) =>
                    {
                        x.Code = y;
                        x.Id = Guid.NewGuid();
                        x.ShopId = shopId;
                        return x;
                    }).ToArray();
                await storage.WriteAsync(workers.Select(ToStorageElement)).ConfigureAwait(false);
                await IncrementVersion(shopId).ConfigureAwait(false);
                return workers;
            }
        }

        public async Task UpdateAsync(Guid shopId, Guid workerId, Action<Worker> update)
        {
            using(await locker.LockAsync(GetLockId(shopId)).ConfigureAwait(false))
            {
                var storageElement = await storage.FirstOrDefaultAsync(x => x.ShopId == shopId && x.Id == workerId);
                if(storageElement == null)
                {
                    throw new KeyNotFoundException($"worker for shop {shopId} and id {workerId} not found");
                }

                var worker = ToModel(storageElement);
                update(worker);

                worker.Id = storageElement.Id;
                worker.Code = storageElement.Code;
                worker.ShopId = shopId;

                await storage.WriteAsync(ToStorageElement(worker)).ConfigureAwait(false);
                await IncrementVersion(shopId).ConfigureAwait(false);
            }
        }

        public async Task<Worker> ReadAsync(Guid shopId, Guid workerId)
        {
            return ToModel(await storage.FirstOrDefaultAsync(x => x.ShopId == shopId && x.Id == workerId).ConfigureAwait(false));
        }

        public async Task<Worker[]> ReadByShopAsync(Guid shopId, bool includeDeleted = false)
        {
            return (await storage.WhereAsync(x => x.ShopId == shopId)
                                 .ConfigureAwait(false))
                   .Select(ToModel)
                   .Where(x => includeDeleted || !x.IsDeleted)
                   .ToArray();
        }

        public IEnumerable<Worker> ReadAll()
        {
            return storage.Table.SetPageSize(1000).Execute().Select(ToModel);
        }

        public Task<int> GetVersionAsync(Guid shopId)
        {
            return counterRepository.GetCurrentAsync(GetVersionKey(shopId));
        }

        private Task IncrementVersion(Guid organizationId)
        {
            return counterRepository.IncrementAsync(GetVersionKey(organizationId));
        }

        private async Task<IEnumerable<int>> GetNextNumbersAsync(Guid organizationId, int length)
        {
            var counter = await counterRepository.IncrementAsync(GetCounterKey(organizationId), length).ConfigureAwait(false);
            return Enumerable.Range(counter, length);
        }

        private WorkerStorageElement ToStorageElement(Worker worker)
        {
            return mapper.Map<WorkerStorageElement>(worker);
        }

        private Worker ToModel(WorkerStorageElement storageElement)
        {
            return mapper.Map<Worker>(storageElement);
        }

        private string GetLockId(Guid shopId)
        {
            return $"worker/{shopId}";
        }

        private string GetCounterKey(Guid shopId)
        {
            return $"counter/worker/{shopId}";
        }

        private string GetVersionKey(Guid shopId)
        {
            return $"version/worker/{shopId}";
        }

        private readonly CassandraStorage<WorkerStorageElement> storage;
        private readonly IMapper mapper;
        private readonly ILocker locker;
        private readonly ICounterRepository counterRepository;
    }
}