using Kontur.Selone.Elements;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers
{
    public class CalendarRow : ComponentBase
    {
        public CalendarRow(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }
        
        public ElementsCollection<CalendarCell> CalendarCell { get; set; }
    }
}