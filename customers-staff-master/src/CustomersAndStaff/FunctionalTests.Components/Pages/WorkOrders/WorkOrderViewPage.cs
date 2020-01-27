using Kontur.Selone.Extensions;
using Kontur.Selone.Properties;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.WorkOrders
{
    [TidPage("WorkOrderView")]
    public class WorkOrderViewPage : PageBase
    {
        public WorkOrderViewPage(IWebDriver searchContext, By @by, IComponentFactory componentFactory)
            : base(searchContext, @by, componentFactory)
        {
        }

        public override IProp<bool> IsPresent => Prop.Create(() => Container.Present().Get(), "WorkOrderView.IsPresent");

        public HeaderBlock HeaderBlock { get; set; }
        public ClientBlock ClientBlock { get; set; }
        public InfoBlock InfoBlock { get; set; }
        public ServicesBlock ServicesBlock { get; set; }
        public ProductsBlock ProductsBlock { get; set; }
        public CustomerProductsBlock CustomerProductsBlock { get; set; }
        public CustomerValuesBlock CustomerValuesBlock { get; set; }
        public TextArea AdditionalText { get; set; }

        public Button SaveButton { get; set; }
        public Link PrintLink { get; set; }
    }
}