using System;
using System.Threading.Tasks;

using FluentAssertions.Extensions;

using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.Components.Pages.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkOrderExtensions;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.WorkOrderTests
{
    public class WorkOrderNumberTests : WorkOrderTestBase
    {
        [Test, Description("Увеличение номера, смена серии")]
        public async Task IncrementNumberTest()
        {
            await CreateDefaultCustomer();

            var workOrderListPage = LoadWorkOrderList();
            var workOrderViewPage = workOrderListPage.CreateNewOrder();
            workOrderViewPage.HeaderBlock.CheckOrderNumber("АА", "000001");

            RefreshPage();
            workOrderViewPage.HeaderBlock.CheckOrderNumber("АА", "000002");
            workOrderListPage = workOrderViewPage.GoBackToList();

            workOrderViewPage = workOrderListPage.CreateNewOrder();
            workOrderViewPage.HeaderBlock.CheckOrderNumber("АА", "000003");
            workOrderViewPage.HeaderBlock.SetOrderNumber("АЯ", "999999");
            workOrderViewPage.ClientBlock.NameSelector.SelectByIndex(0);
            workOrderViewPage.InfoBlock.CompletionDatePlanned.SetValue(DateTime.Today + 1.Days());
            workOrderListPage = workOrderViewPage.SaveAndGoToList();

            workOrderViewPage = workOrderListPage.CreateNewOrder();
            workOrderViewPage.HeaderBlock.CheckOrderNumber("БА", "000001");
            workOrderViewPage.HeaderBlock.SetOrderNumber("ЯЯ", "999999");
            workOrderViewPage.ClientBlock.NameSelector.SelectByIndex(0);
            workOrderViewPage.InfoBlock.CompletionDatePlanned.SetValue(DateTime.Today + 1.Days());
            workOrderListPage = workOrderViewPage.SaveAndGoToList();

            workOrderViewPage = workOrderListPage.CreateNewOrder();
            workOrderViewPage.HeaderBlock.CheckOrderNumber("АА", "000004");
        }

        [Test, Description("Удаление ЗН не влияет на высвобождение забронированного номера")]
        public async Task RemoveOrderTest()
        {
            await CreateDefaultWorkOrder();

            var workOrderListPage = LoadWorkOrderList();
            var workOrderViewPage = workOrderListPage.GoToNotIssuedOrder();
            workOrderViewPage.HeaderBlock.CheckOrderNumber("АА", "000001");

            workOrderViewPage.HeaderBlock.RemoveLink.Click();
            var modal = workOrderViewPage.WaitModal<WorkOrderRemoveModal>();
            modal.Accept.Click();
            workOrderListPage = workOrderViewPage.GoToPage<WorkOrderListPage>();
            workOrderViewPage = workOrderListPage.CreateNewOrder();
            workOrderViewPage.HeaderBlock.CheckOrderNumber("АА", "000002");
        }
    }
}