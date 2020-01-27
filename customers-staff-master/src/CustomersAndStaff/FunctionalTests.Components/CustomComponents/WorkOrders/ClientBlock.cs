using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.CtorAttributes;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders
{
    public class ClientBlock : ComponentBase
    {
        public ClientBlock(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public ComboBox NameSelector { get; set; }
        public ErrorMessageTooltip NameValidation { get; set; }
        public Input NameInput { get; set; }
        [PhoneCtor(false)]
        public Input<string> PhoneInput { get; set; }
        public TextArea Comment { get; set; }
        public Link CancelEdit { get; set; }
        public Link ChangeData { get; set; }
        public Link SelectAnother { get; set; }
        public Label SelectedName { get; set; }
        [PhoneCtor(true)]
        public Label<string> SelectedPhone { get; set; }
        public Label SelectedInfo { get; set; }

        public void ClickEmptySpace()
        {
            Container.Click();
        }
    }
}