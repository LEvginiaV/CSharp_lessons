using Kontur.Selone.Extensions;
using Kontur.Selone.Properties;

using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.MainPage;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages
{
    [TidPage("MainPage")]
    public class CustomersAndStaffMainPage : PageBase
    {
        public CustomersAndStaffMainPage(IWebDriver searchContext, By @by, IComponentFactory componentFactory)
            : base(searchContext, @by, componentFactory)
        {
        }

        public NavigationLayout NavigationLayout { get; set; }

        public override IProp<bool> IsPresent => Prop.Create(() => Container.Present().Get(), "MainPage.IsPresent");
    }
}