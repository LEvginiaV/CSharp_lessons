using System.Linq;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.Helpers;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.Models.WorkOrders;

namespace Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkOrderExtensions
{
    public static class OrderStatusSelectorExtensions
    {
        public static void SelectByValue(this OrderStatusSelector orderStatusSelector, WorkOrderStatus status)
        {
            orderStatusSelector.Overlay.Container.Click();
            orderStatusSelector.Selector.ItemListPresent.Wait().EqualTo(true);
            orderStatusSelector.Selector.GetMenuItemList<Button>().First(x => x.Text.Get() == status.GetDescription()).Click();
        }

        public static void WaitValue(this OrderStatusSelector orderStatusSelector, WorkOrderStatus status)
        {
            orderStatusSelector.Selector.Text.Wait().EqualTo(status.GetDescription());
        }
    }
}