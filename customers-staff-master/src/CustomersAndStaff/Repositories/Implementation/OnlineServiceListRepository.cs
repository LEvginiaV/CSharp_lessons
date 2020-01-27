using System;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Models.OnlineRecording;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Repositories.Serializer;
using Market.CustomersAndStaff.Repositories.StoredModels.OnlineRecording;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.SessionTableQueryExtending.PrimitiveStoring;

namespace Market.CustomersAndStaff.Repositories.Implementation
{
    public class OnlineServiceListRepository : IOnlineServiceListRepository
    {
        public OnlineServiceListRepository(
            CassandraStorage<OnlineServiceListStorageElement> storage, 
            ISerializer serializer)
        {
            this.storage = storage;
            this.serializer = serializer;
        }

        public async Task<OnlineService[]> ReadAsync(Guid shopId)
        {
            var element = await storage.FirstOrDefaultAsync(x => x.ShopId == shopId);
            return element == null ? new OnlineService[0] : serializer.Deserialize<OnlineService[]>(element.SerializedList);
        }

        public async Task WriteAsync(Guid shopId, OnlineService[] services)
        {
            var element = new OnlineServiceListStorageElement
                {
                    ShopId = shopId,
                    SerializedList = serializer.Serialize(services),
                };
            await storage.WriteAsync(element);
        }

        private readonly CassandraStorage<OnlineServiceListStorageElement> storage;
        private readonly ISerializer serializer;
    }
}