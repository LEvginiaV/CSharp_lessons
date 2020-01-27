using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders
{
    public class OrderStatusSelector : ComponentBase
    {
        public OrderStatusSelector(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Dropdown Selector { get; set; }
        public Label Overlay { get; set; }
    }
}