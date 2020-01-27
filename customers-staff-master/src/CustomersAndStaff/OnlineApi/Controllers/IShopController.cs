using Market.Api.Models.Shops;

namespace Market.CustomersAndStaff.OnlineApi.Controllers
{
    public interface IShopController
    {
        Shop Shop { set; get; }
    }
}