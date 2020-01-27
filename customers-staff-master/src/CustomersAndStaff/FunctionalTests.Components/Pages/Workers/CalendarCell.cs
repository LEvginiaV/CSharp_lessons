using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers
{
    public class CalendarCell : ComponentBase
    {
        public CalendarCell(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Label Filled { get; set; }
        public Label Empty { get; set; }
    }
}