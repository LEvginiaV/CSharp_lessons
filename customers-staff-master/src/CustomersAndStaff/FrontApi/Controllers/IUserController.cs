using Portal.Requisites;

namespace Market.CustomersAndStaff.FrontApi.Controllers
{
    public interface IUserController
    {
        User AuthUser { get; set; }
    }
}