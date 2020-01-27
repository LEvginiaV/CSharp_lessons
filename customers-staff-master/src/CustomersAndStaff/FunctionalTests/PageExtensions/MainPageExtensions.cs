using Market.CustomersAndStaff.FunctionalTests.Components.Pages;
using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Calendar;
using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers;

namespace Market.CustomersAndStaff.FunctionalTests.PageExtensions
{
    public static class MainPageExtensions
    {
        public static WorkerListPage GoToWorkerListPage(this CustomersAndStaffMainPage customersAndStaffMainPage)
        {
            customersAndStaffMainPage.NavigationLayout.WorkersLink.Click();
            return customersAndStaffMainPage.GoToPage<WorkerListPage>();
        }

        public static CalendarPage GoToCalendarPage(this CustomersAndStaffMainPage customersAndStaffMainPage)
        {
            customersAndStaffMainPage.NavigationLayout.CalendarLink.Click();
            return customersAndStaffMainPage.GoToPage<CalendarPage>();
        }
    }
}
