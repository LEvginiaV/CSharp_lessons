using System.Linq;

using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers;

namespace Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkerListExtensions
{
    public static class CalendarRowExtensions 
    {
        public static void ClickOnCell(this CalendarRow row, int idx) => row.CalendarCell.ElementAt(idx).Click();
    }
}