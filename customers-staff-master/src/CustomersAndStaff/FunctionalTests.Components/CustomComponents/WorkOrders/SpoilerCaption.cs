using Kontur.Selone.Extensions;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders
{
    public class SpoilerCaption : ComponentBase
    {
        public SpoilerCaption(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public void Click()
        {
            Container.ScrollIntoView();
            Container.Click();
        }
    }
}