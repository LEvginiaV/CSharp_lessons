using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;

namespace Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkerListExtensions
{
    public static class CalendarCellExtensions
    {
        public static void Click(this CalendarCell cell)
        {
            cell.Container.Click();
        }
        
        public static void WaitFilled(this CalendarCell cell, bool filled, string errorMessage = "")
        {
            if(filled)
            {
                cell.Filled.WaitPresence(componentDescription : "заполненный день " + errorMessage);
                cell.Empty.WaitAbsence(componentDescription : "заполненный день " + errorMessage);
            }
            else
            {
                cell.Empty.WaitPresence(componentDescription: "незаполненный день " + errorMessage);
                cell.Filled.WaitAbsence(componentDescription: "незаполненный день " + errorMessage);
            }
        }

        public static void WaitFilled(this CalendarCell cell, string errorMessage = "") => cell.WaitFilled(true, errorMessage);

        public static void WaitNotFilled(this CalendarCell cell, string errorMessage = "") => cell.WaitFilled(false, errorMessage);
    }
}