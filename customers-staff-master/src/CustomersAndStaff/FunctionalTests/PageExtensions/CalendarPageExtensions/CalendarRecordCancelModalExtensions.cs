using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Calendar;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;

namespace Market.CustomersAndStaff.FunctionalTests.PageExtensions.CalendarPageExtensions
{
    public static class CalendarRecordCancelModalExtensions
    {
        public static CalendarRecordCancelModal SelectNotComeReason(this CalendarRecordCancelModal modal)
        {
            modal.NotCome.Container.Click();
            return modal;
        }

        public static CalendarRecordCancelModal ClickCancelRecord(this CalendarRecordCancelModal modal, bool shouldSuccess = true)
        {
            modal.CancelRecord.Click();
            if(shouldSuccess) modal.WaitAbsence();
            return shouldSuccess ? null : modal;
        }

        public static void ClickDoNotCancelRecord(this CalendarRecordCancelModal modal)
        {
            modal.DoNotCancelRecord.Click();
            modal.WaitAbsence();
        }

        public static CalendarRecordCancelModal CheckValidationMessage(this CalendarRecordCancelModal modal, string message)
        {
            modal.ErrorMessage.WaitText(message, errorMessage: "ожидаем тест валидации в модале отмены записи");
            return modal;
        }
    }
}
