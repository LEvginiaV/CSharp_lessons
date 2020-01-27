using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure
{
    public class ModalBase : PortalComponentBase
    {
        public ModalBase(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }
    }
}