using Kontur.Selone.Elements;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.CtorAttributes;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.Calendar
{
    public class CalendarRecordTooltip : PortalComponentBase
    {
        public CalendarRecordTooltip(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Label StartTime { get; set; }
        public Label EndTime { get; set; }

        public Link ChangeRecord { get; set; }
        public Link CancelRecord { get; set; }

        public Button ActiveButton { get; set; }
        public Button MessageButton { get; set; }
        public Button CompletedButton { get; set; }
        
        public Label ActiveButtonSelected { get; set; }
        public Label MessageButtonSelected { get; set; }
        public Label CompletedButtonSelected { get; set; }

        public Label CustomerName { get; set; }
        public Label CustomerPhone { get; set; }
        [PercentCtor]
        public Label<decimal?> CustomerDiscount { get; set; }

        public Label Comment { get; set; }

        public ElementsCollection<ServiceItemWithRubleSign> ServiceItem { get; set; }
    }
}