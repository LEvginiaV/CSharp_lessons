using System.Linq;

using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;

namespace Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkOrderExtensions
{
    public static class WorkOrderListExtensions
    {
        public static void WaitItemNamesEquivalentTo(this WorkOrderList workOrderList, params string[] names)
        {
            workOrderList.WorkOrderItem.Select(x => x.Name.Text).Wait().ShouldBeEquivalentTo(names);
        }

        public static void WaitEmpty(this WorkOrderList workOrderList)
        {
            workOrderList.WorkOrderItem.Count.Wait().EqualTo(0);
        }
    }
}