using Market.CustomersAndStaff.FunctionalTests.Components.Pages.WorkOrders;

namespace Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkOrderExtensions
{
    public static class WorkOrderViewPageExtensions
    {
        public static WorkOrderListPage SaveAndGoToList(this WorkOrderViewPage page)
        {
            page.SaveButton.Click();
            return page.GoToPage<WorkOrderListPage>();
        }

        public static WorkOrderListPage GoBackToList(this WorkOrderViewPage page)
        {
            page.HeaderBlock.BackButton.Click();
            return page.GoToPage<WorkOrderListPage>();
        }
    }
}