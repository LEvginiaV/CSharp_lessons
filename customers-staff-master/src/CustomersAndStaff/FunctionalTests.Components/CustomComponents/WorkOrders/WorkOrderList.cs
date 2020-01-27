using Kontur.Selone.Elements;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders
{
    public class WorkOrderList : ComponentBase
    {
        public WorkOrderList(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public ElementsCollection<WorkOrderItem> WorkOrderItem { get; set; }
    }
}