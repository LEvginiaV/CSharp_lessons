using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders
{
    public class OtherValue : ComponentBase
    {
        public OtherValue(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public TextArea Comment { get; set; }
    }
}