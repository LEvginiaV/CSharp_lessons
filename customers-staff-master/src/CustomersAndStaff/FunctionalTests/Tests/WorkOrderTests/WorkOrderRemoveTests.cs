using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.Components.Pages.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.Helpers.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkOrderExtensions;
using Market.CustomersAndStaff.Models.WorkOrders;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.WorkOrderTests
{
    public class WorkOrderRemoveTests : WorkOrderTestBase
    {
        [Test, Description("Удаление")]
        public async Task RemoveTest()
        {
            await CreateDefaultWorkOrder();

            var workOrderListPage = LoadWorkOrderList();
            workOrderListPage.NotIssuedOrders.WorkOrderItem.First().Click();
            var workOrderViewPage = workOrderListPage.GoToPage<WorkOrderViewPage>();

            workOrderViewPage.HeaderBlock.RemoveLink.Click();
            var workOrderRemoveModal = workOrderViewPage.WaitModal<WorkOrderRemoveModal>();
            workOrderRemoveModal.Cancel.Click();
            workOrderViewPage.WaitModalClose<WorkOrderRemoveModal>();

            var workOrder = await workOrderHelper.ReadSingleAsync();
            workOrder.DocumentStatus.Should().Be(WorkOrderDocumentStatus.Saved);

            workOrderViewPage.HeaderBlock.RemoveLink.Click();
            workOrderRemoveModal = workOrderViewPage.WaitModal<WorkOrderRemoveModal>();
            workOrderRemoveModal.Accept.Click();
            workOrderListPage = workOrderViewPage.GoToPage<WorkOrderListPage>();

            workOrderListPage.NotIssuedOrders.WaitEmpty();
            workOrder = await workOrderHelper.ReadSingleAsync();
            workOrder.DocumentStatus.Should().Be(WorkOrderDocumentStatus.Removed);
        }
        
        [Injected]
        private IWorkOrderHelper workOrderHelper;
    }
}