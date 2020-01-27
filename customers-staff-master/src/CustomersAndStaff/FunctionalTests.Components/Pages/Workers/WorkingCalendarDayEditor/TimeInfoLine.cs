using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers.WorkingCalendarDayEditor
{
    public class TimeInfoLine : ComponentBase
    {
        public TimeInfoLine(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Label StartTimeText { get; set; }
        public Label EndTimeText { get; set; }
        public Label OverflowText { get; set; }
    }
}