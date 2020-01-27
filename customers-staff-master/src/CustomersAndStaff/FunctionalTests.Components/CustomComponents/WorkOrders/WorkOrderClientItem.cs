using Kontur.Selone.Selectors.Css;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.PropTransformations;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders
{
    public class WorkOrderClientItem : ComponentBase
    {
        public WorkOrderClientItem(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
            Name = new Label(this.Container, new CssBy().WithTid("Name"));
            Phone = new Label<string>(Container, new CssBy().WithTid("Phone"), new PhonePropTransformation(true, "Нет телефона"));
        }

        public Label Name { get; set; }
        public Label<string> Phone { get; set; }

        public void Click()
        {
            Container.Click();
        }
    }
}