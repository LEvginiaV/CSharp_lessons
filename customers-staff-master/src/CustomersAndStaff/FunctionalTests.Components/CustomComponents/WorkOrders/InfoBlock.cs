using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.CtorAttributes;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders
{
    public class InfoBlock : ComponentBase
    {
        public InfoBlock(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        [PhoneCtor(false)]
        public Input<string> Phone { get; set; }
        public Dropdown Worker { get; set; }
        public Input WarrantyNumber { get; set; }
        public DatePicker CompletionDatePlanned { get; set; }
        public DatePicker CompletionDateFact { get; set; }
        public ErrorMessageTooltip CompletionDatePlannedValidation { get; set; }
        public ErrorMessageTooltip CompletionDateFactValidation { get; set; }
    }
}