using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers.WorkingCalendarDayEditor
{
    public class TimeRangeLine : ComponentBase
    {
        public Input StartTime { get; set; }
        public Input EndTime { get; set; }
        
        public ErrorMessageTooltip ErrorMessageTooltipStart { get; set; }
        public ErrorMessageTooltip ErrorMessageTooltipEnd { get; set; }

        public Label OverflowText { get; set; }
        public Link AddLink { get; set; }
        public Link DeleteLink { get; set; }

        public TimeRangeLine(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }
    }
}