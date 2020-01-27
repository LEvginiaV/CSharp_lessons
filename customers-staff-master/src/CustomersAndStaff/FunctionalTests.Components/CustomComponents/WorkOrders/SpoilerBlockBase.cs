using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders
{
    public class SpoilerBlockBase : ComponentBase
    {
        public SpoilerBlockBase(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public SpoilerCaption SpoilerCaption { get; set; }
    }
}