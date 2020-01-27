using Kontur.Selone.Elements;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers.WorkingCalendarDayEditor
{
    public class DayInfoBlock : ComponentBase
    {
        public DayInfoBlock(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public ElementsCollection<TimeInfoLine> TimeInfoLine { get; set; }

        public Button Edit { get; set; }
        public Button Remove { get; set; }
    }
}