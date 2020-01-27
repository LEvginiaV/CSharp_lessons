using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers.WorkingCalendarDayEditor
{
    public class DayEditor : PortalComponentBase
    {
        public DayEditor(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Label HeaderDate { get; set; }
        public Label HeaderDayOfWeek { get; set; }
        
        public DayEditingBlock EditingView { get; set; }
        public DayInfoBlock InfoView { get; set; }
    }
}