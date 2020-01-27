using Kontur.Selone.Elements;
using Kontur.Selone.Extensions;
using Kontur.Selone.Properties;

using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Customers;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.Customers
{
    [TidPage("CustomerList")]
    public class CustomerListPage : PersonListPageBase
    {
        public CustomerListPage(IWebDriver searchContext, By @by, IComponentFactory componentFactory)
            : base(searchContext, @by, componentFactory)
        {
        }

        public ElementsCollection<CustomerItem> CustomerItem { get; set; }

        public override IProp<bool> IsPresent => Prop.Create(() => Container.Present().Get() && !Loader.IsPresent.Get(), "CustomersList.IsPresent");
    }
}