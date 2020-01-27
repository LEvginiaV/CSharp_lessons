using System;
using System.Threading.Tasks;

using AutoMapper;

using Market.CustomersAndStaff.Models.OnlineRecording;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Repositories.StoredModels.OnlineRecording;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.SessionTableQueryExtending.PrimitiveStoring;
using SKBKontur.Catalogue.CassandraUtils.DistributedLock.Locker;

namespace Market.CustomersAndStaff.Repositories.Implementation
{
    public class PublicLinkRepository : IPublicLinkRepository
    {
        public PublicLinkRepository(
            CassandraStorage<ShopIdToPublicLinkStorageElement> shopIdToLinkStorage,
            CassandraStorage<PublicLinkToShopIdStorageElement> linkToShopIdStorage,
            IMapper mapper,
            ILocker locker)
        {
            this.shopIdToLinkStorage = shopIdToLinkStorage;
            this.linkToShopIdStorage = linkToShopIdStorage;
            this.mapper = mapper;
            this.locker = locker;
        }

        public async Task<PublicLink> ReadByShopIdAsync(Guid shopId)
        {
            var link = (await shopIdToLinkStorage.FirstOrDefaultAsync(x => x.ShopId == shopId))?.Link;

            if(link == null)
            {
                return null;
            }

            var storageElement = await linkToShopIdStorage.FirstOrDefaultAsync(x => x.Link == link);

            if(storageElement == null)
            {
                throw new InvalidProgramException($"{link} link for shopId {shopId} not found");
            }

            return mapper.Map<PublicLink>(storageElement);
        }

        public async Task<PublicLink> ReadByPublicLinkAsync(string link)
        {
            link = link.ToLowerInvariant();
            var storageElement = await linkToShopIdStorage.FirstOrDefaultAsync(x => x.Link == link);
            
            return mapper.Map<PublicLink>(storageElement);
        }

        public async Task<bool> WriteOrUpdateAsync(PublicLink publicLink)
        {
            using(await locker.LockAsync(GetLockId(publicLink.Link)))
            {
                publicLink.Link = publicLink.Link.ToLowerInvariant();
                var previousLinkToShopId = await linkToShopIdStorage.FirstOrDefaultAsync(x => x.Link == publicLink.Link);
                var previousShopIdToLink = await shopIdToLinkStorage.FirstOrDefaultAsync(x => x.ShopId == publicLink.ShopId);

                if (previousLinkToShopId != null && previousLinkToShopId.ShopId != publicLink.ShopId)
                {
                    return false;
                }

                var linkToShopId = mapper.Map<PublicLinkToShopIdStorageElement>(publicLink);
                var shopIdToLink = mapper.Map<ShopIdToPublicLinkStorageElement>(linkToShopId);

                await linkToShopIdStorage.WriteAsync(linkToShopId);
                await shopIdToLinkStorage.WriteAsync(shopIdToLink);

                if(previousShopIdToLink != null && previousShopIdToLink.Link != publicLink.Link)
                {
                    await linkToShopIdStorage.DeleteAsync(x => x.Link == previousShopIdToLink.Link);
                }

                return true;
            }
        }

        private static string GetLockId(string link)
        {
            return $"PublicLink/{link}";
        }

        private readonly CassandraStorage<ShopIdToPublicLinkStorageElement> shopIdToLinkStorage;
        private readonly CassandraStorage<PublicLinkToShopIdStorageElement> linkToShopIdStorage;
        private readonly ILocker locker;
        private readonly IMapper mapper;
    }
}