using System;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Repositories.StoredModels;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.SessionTableQueryExtending.PrimitiveStoring;

namespace Market.CustomersAndStaff.Repositories.Implementation
{
    public class CounterRepository : ICounterRepository
    {
        public CounterRepository(CassandraStorage<CounterStorageElement> storage)
        {
            this.storage = storage;
        }

        public async Task<int> IncrementAsync(string key, int step = 1)
        {
            if(step < 1)
            {
                throw new ArgumentException($"Value should be more then 0", nameof(step));
            }

            var currentValue = await GetCurrentAsync(key);
            await storage.WriteAsync(new CounterStorageElement {Key = key, Counter = currentValue + step});

            return currentValue;
        }

        public async Task<int> GetCurrentAsync(string key)
        {
            return (await storage.FirstOrDefaultAsync(x => x.Key == key))?.Counter ?? 1;
        }

        private readonly CassandraStorage<CounterStorageElement> storage;
    }
}