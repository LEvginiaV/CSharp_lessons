using Market.Api.Models.Shops;

namespace Market.CustomersAndStaff.FrontApi.Controllers
{
    public interface IShopController
    {
        Shop Shop { get; set; }
    }
}