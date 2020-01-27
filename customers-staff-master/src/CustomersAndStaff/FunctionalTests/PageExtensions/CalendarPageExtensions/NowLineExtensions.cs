using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Calendar;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;

namespace Market.CustomersAndStaff.FunctionalTests.PageExtensions.CalendarPageExtensions
{
    public static class NowLineExtensions
    {
        public static void WaitTop(this NowLine nowLine, int value, int delta)
        {
             nowLine.Top.Wait().InRange(value - delta, value + delta);
        }
    }
}