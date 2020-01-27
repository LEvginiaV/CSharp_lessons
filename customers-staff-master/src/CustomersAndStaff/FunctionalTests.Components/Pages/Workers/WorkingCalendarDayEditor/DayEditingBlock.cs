using Kontur.Selone.Elements;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers.WorkingCalendarDayEditor
{
    public class DayEditingBlock : ComponentBase
    {
        public DayEditingBlock(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Label FakeBlock { get; set; }
        public ElementsCollection<TimeRangeLine> TimeRangeLine { get; set; }
        
        public Dropdown CalendarMode { get; set; }

        public Button Save { get; set; }
        public Button Cancel { get; set; }
    }
}