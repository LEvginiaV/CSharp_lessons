using System.Threading.Tasks;

using GroboContainer.NUnitExtensions;

using Market.Api.Models.Shops;
using Market.CustomersAndStaff.FunctionalTests.Components.Pages.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.Helpers.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Services.WorkOrders;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.WorkOrderTests
{
    public class WorkOrderTestBase : TestBase
    {
        protected WorkOrderListPage LoadWorkOrderList(Shop shop = null)
        {
            var mainPage = LoadMainPage(shop);
            mainPage.NavigationLayout.OrdersLink.Click();

            return mainPage.GoToPage<WorkOrderListPage>();
        }

        protected async Task<Customer> CreateDefaultCustomer()
        {
            return await customerRepository.CreateAsync(shop.OrganizationId, new Customer
                {
                    Name = "Маша",
                });
        }

        protected async Task CreateDefaultWorkOrder()
        {
            var customer = await CreateDefaultCustomer();
            await workOrderService.CreateNewOrderAsync(shop.Id, WorkOrderBuilder.CreateWithCustomer(customer).Build());
        }

        [Injected]
        protected readonly IWorkOrderService workOrderService;

        [Injected]
        protected readonly ICustomerRepository customerRepository;
    }
}