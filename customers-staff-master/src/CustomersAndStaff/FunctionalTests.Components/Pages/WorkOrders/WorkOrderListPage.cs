using Kontur.Selone.Extensions;
using Kontur.Selone.Properties;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.WorkOrders
{
    [TidPage("WorkOrderList")]
    public class WorkOrderListPage : PageBase
    {
        public WorkOrderListPage(IWebDriver searchContext, By @by, IComponentFactory componentFactory)
            : base(searchContext, @by, componentFactory)
        {
        }

        public override IProp<bool> IsPresent => Prop.Create(() => Container.Present().Get() && AddButton.IsPresent.Get(), "WorkOrderList.IsPresent");

        public WorkOrderList NotIssuedOrders { get; set; }
        public WorkOrderList IssuedOrders { get; set; }
        public Button AddButton { get; set; }
    }
}