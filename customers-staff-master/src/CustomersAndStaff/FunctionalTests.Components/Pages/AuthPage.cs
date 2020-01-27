using Kontur.Selone.Extensions;
using Kontur.Selone.Properties;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages
{
    [TidPage("Auth")]
    public class AuthPage : PageBase
    {
        public AuthPage(IWebDriver searchContext, By @by, IComponentFactory componentFactory)
            : base(searchContext, @by, componentFactory)
        {
        }

        public Input ShopIdInput { get; set; }
        public Input AuthSidInput { get; set; }
        public Button SendButton { get; set; }
        public override IProp<bool> IsPresent => Prop.Create(() => Container.Present().Get(), "AuthPage.IsPresent");
    }
}