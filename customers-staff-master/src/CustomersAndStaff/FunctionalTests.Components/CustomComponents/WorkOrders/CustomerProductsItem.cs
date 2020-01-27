using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders
{
    public class CustomerProductsItem : ComponentBase
    {
        public CustomerProductsItem(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Input Name { get; set; }
        public Input Quantity { get; set; }
        public Link RemoveLink { get; set; }

        public ErrorMessageTooltip QuantityValidation { get; set; }
    }
}