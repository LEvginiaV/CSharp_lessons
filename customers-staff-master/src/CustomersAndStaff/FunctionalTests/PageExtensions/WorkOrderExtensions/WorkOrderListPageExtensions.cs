using System.Linq;

using Market.CustomersAndStaff.FunctionalTests.Components.Pages.WorkOrders;

namespace Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkOrderExtensions
{
    public static class WorkOrderListPageExtensions
    {
        public static WorkOrderViewPage GoToIssuedOrder(this WorkOrderListPage page, int index = 0)
        {
            page.IssuedOrders.WorkOrderItem.ElementAt(index).Click();
            return page.GoToPage<WorkOrderViewPage>();
        }

        public static WorkOrderViewPage GoToNotIssuedOrder(this WorkOrderListPage page, int index = 0)
        {
            page.NotIssuedOrders.WorkOrderItem.ElementAt(index).Click();
            return page.GoToPage<WorkOrderViewPage>();
        }

        public static WorkOrderViewPage CreateNewOrder(this WorkOrderListPage page)
        {
            page.AddButton.Click();
            return page.GoToPage<WorkOrderViewPage>();
        }
    }
}