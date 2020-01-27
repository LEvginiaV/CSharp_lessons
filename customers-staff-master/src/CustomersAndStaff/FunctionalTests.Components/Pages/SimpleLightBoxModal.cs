using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages
{
    public abstract class SimpleLightBoxModal : ModalBase
    {
        public SimpleLightBoxModal(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Button Accept { get; set; }
        public Button Cancel { get; set; }
    }
}