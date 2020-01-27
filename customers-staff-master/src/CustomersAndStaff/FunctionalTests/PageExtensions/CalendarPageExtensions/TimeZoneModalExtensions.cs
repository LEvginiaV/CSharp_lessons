using System.Linq;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Calendar;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;

namespace Market.CustomersAndStaff.FunctionalTests.PageExtensions.CalendarPageExtensions
{

    public static class TimeZoneModalExtensions
    {
        public static TimeZoneModal ChangeTimeZone(this TimeZoneModal modal, int utcHour)
        {
            modal.TimeZoneLoader.WaitAbsence();
            modal.TimeZoneSelect.WaitPresence();
            modal.TimeZoneSelect.Click();
            modal.TimeZoneSelect.GetMenuItemList<Label>().Count.Wait().EqualTo(11);
            modal.TimeZoneSelect.GetMenuItemList<Label>().Single(x => x.Text.Get().StartsWith($"UTC+{utcHour}")).Container.Click();
            return modal;
        }

        public static void SaveAndClose(this TimeZoneModal modal)
        {
            modal.Accept.Click();
            modal.WaitAbsence();
        }

        public static void Close(this TimeZoneModal modal)
        {
            modal.Cancel.Click();
            modal.WaitAbsence();
        }
    }
}