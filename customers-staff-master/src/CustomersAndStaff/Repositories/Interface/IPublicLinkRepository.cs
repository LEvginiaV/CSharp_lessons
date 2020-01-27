using System;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Models.OnlineRecording;

namespace Market.CustomersAndStaff.Repositories.Interface
{
    public interface IPublicLinkRepository
    {
        Task<PublicLink> ReadByShopIdAsync(Guid shopId);
        Task<PublicLink> ReadByPublicLinkAsync(string link);
        Task<bool> WriteOrUpdateAsync(PublicLink publicLink);
    }
}