using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders
{
    public class HeaderBlock : ComponentBase
    {
        public HeaderBlock(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Button BackButton { get; set; }
        public Input SeriesInput { get; set; }
        public Input NumberInput { get; set; }
        public DatePicker ReceptionDate { get; set; }
        public Dropdown WorkOrderStatus { get; set; }
        public ErrorMessageTooltip ReceptionDateValidation { get; set; }
        public Link RemoveLink { get; set; }
    }
}