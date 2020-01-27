using System.Threading.Tasks;

using Market.CustomersAndStaff.FunctionalTests.Infrastructure;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.WorkOrderTests
{
    public class WorkOrderOtherTests : WorkOrderTestBase
    {
        [Test, Description("Хранение")]
        public async Task TwoSalesPointOneOrganizationTest()
        {
            await CreateDefaultWorkOrder();
            var shop2 = await CreateShop(shop.OrganizationId, shop.DepartmentId);

            var workOrderListPage = LoadWorkOrderList();
            workOrderListPage.NotIssuedOrders.WorkOrderItem.Count.Wait().EqualTo(1);

            workOrderListPage = LoadWorkOrderList(shop2);
            workOrderListPage.NotIssuedOrders.WorkOrderItem.Count.Wait().EqualTo(0);
        }
    }
}