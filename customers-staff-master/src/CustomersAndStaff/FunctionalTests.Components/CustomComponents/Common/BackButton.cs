using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common
{
    public class BackButton : ComponentBase
    {
        public BackButton(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public void Click()
        {
            Container.Click();
        }
    }
}