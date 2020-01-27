using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers.WorkingCalendarDayEditor;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;

namespace Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkingCalendarExtensions
{
    public static class DayEditorExtensions
    {
        public static DayEditingBlock GetEditingView(this DayEditor editor)
        {
            if(editor.InfoView.IsPresent.Get())
            {
                editor.InfoView.Edit.Click();
            }
            editor.EditingView.WaitPresence(componentDescription: $"ожидаем присутствия {nameof(editor.EditingView)}");
            return editor.EditingView;
        }

        public static DayInfoBlock GetInfoView(this DayEditor editor)
        {
            editor.InfoView.WaitPresence(componentDescription: $"ожидаем присутствия {nameof(editor.InfoView)}");
            return editor.InfoView;
        }

        public static void Close(this DayEditor editor)
        {
            if(editor.InfoView.IsPresent.Get())
            {
                editor.InfoView.Edit.Click();
                editor.EditingView.WaitPresence();
            }
            editor.EditingView.Cancel.Click();
            editor.WaitAbsence(componentDescription: $"{nameof(DayEditor)}");
        }

        public static void ClickMakeDayOff(this DayEditor editor)
        {
            editor.InfoView.WaitPresence(componentDescription: $"{nameof(editor.InfoView)}");
            editor.InfoView.Remove.Click();
            editor.WaitAbsence(componentDescription: $"{nameof(DayEditor)}");
        }
    }
}