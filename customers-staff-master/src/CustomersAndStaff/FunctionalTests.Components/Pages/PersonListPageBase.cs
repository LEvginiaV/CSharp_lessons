using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages
{
    public abstract class PersonListPageBase : PageBase
    {
        protected PersonListPageBase(IWebDriver searchContext, By @by, IComponentFactory componentFactory)
            : base(searchContext, @by, componentFactory)
        {
        }

        public Button AddButton { get; set; }
        public Input SearchInput { get; set; }
        public EmptyPersonList EmptyPersonList { get; set; }
        public Paging Paging { get; set; }
        public Label Loader { get; set; }
    }
}