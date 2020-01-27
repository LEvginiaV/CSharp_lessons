using Kontur.Selone.Extensions;
using Kontur.Selone.Properties;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.CtorAttributes;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;
using Market.CustomersAndStaff.Models.Customers;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.Customers
{
    [TidPage("CustomerPage")]
    public class CustomerPage : PageBase
    {
        public CustomerPage(IWebDriver searchContext, By @by, IComponentFactory componentFactory)
            : base(searchContext, @by, componentFactory)
        {
        }

        public PersonHeader PersonHeader { get; set; }
        public Link Email { get; set; }
        [PercentCtor(true)]
        public Label<decimal?> Discount { get; set; }
        public Label CustomId { get; set; }
        [BirthdayCtor]
        public Label<Birthday> Birthday { get; set; }
        public TextArea AdditionalInfo { get; set; }
        public Link BackButton { get; set; }

        public override IProp<bool> IsPresent => Prop.Create(() => Container.Present().Get(), "CustomerPage.IsPresent");
    }
}