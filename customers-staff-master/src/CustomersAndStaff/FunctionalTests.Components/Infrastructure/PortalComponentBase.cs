using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure
{
    public abstract class PortalComponentBase : ComponentBase
    {
        protected PortalComponentBase(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }
    }
}