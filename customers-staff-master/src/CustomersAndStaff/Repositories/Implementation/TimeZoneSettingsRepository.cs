using System;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Repositories.StoredModels;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.SessionTableQueryExtending.PrimitiveStoring;

namespace Market.CustomersAndStaff.Repositories.Implementation
{
    public class TimeZoneSettingsRepository : ITimeZoneSettingsRepository
    {
        public TimeZoneSettingsRepository(CassandraStorage<TimeZoneSettingsStorageElement> storage)
        {
            this.storage = storage;
        }

        public async Task SetAsync(Guid shopId, Guid timeZoneId)
        {
            await storage.WriteAsync(new TimeZoneSettingsStorageElement
                {
                    ShopId = shopId,
                    TimeZoneId = timeZoneId,
                });
        }

        public async Task<Guid?> GetAsync(Guid shopId)
        {
            return (await storage.FirstOrDefaultAsync(x => x.ShopId == shopId))?.TimeZoneId;
        }

        private readonly CassandraStorage<TimeZoneSettingsStorageElement> storage;
    }
}