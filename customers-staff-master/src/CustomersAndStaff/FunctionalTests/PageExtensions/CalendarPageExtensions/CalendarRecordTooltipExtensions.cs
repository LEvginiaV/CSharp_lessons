using System.Linq;

using FluentAssertions;

using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Calendar;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;

namespace Market.CustomersAndStaff.FunctionalTests.PageExtensions.CalendarPageExtensions
{
    public static class CalendarRecordTooltipExtensions
    {
        public static void ClickCancel(this CalendarRecordTooltip tooltip)
        {
            tooltip.CancelRecord.Click();
        }

        public static void SetActiveState(this CalendarRecordTooltip tooltip)
        {
            tooltip.ActiveButton.Click();
            tooltip.WaitAbsence();
        }
        
        public static void SetMessageState(this CalendarRecordTooltip tooltip)
        {
            tooltip.MessageButton.Click();
            tooltip.WaitAbsence();
        }
        
        public static void SetCompletedState(this CalendarRecordTooltip tooltip)
        {
            tooltip.CompletedButton.Click();
            tooltip.WaitAbsence();
        }

        public static void CheckServiceItems(this CalendarRecordTooltip tooltip, params (string name, decimal? price)[] serviceItems)
        {
            tooltip.ServiceItem.Wait()
                       .It(items => items
                                    .Select(x => (
                                                     x.ServiceName.Text.Get(),
                                                     x.ServicePrice.IsPresent.Get() ? x.ServicePrice.Value.Get() : (decimal?)null
                                                 )
                                    )
                                    .Should()
                                    .BeEquivalentTo(serviceItems)
                       );
        }

        public static CalendarRecordTooltip CheckIsActiveState(this CalendarRecordTooltip tooltip) => tooltip.CheckSelectedButton(SelectedButtonType.Active);
        public static CalendarRecordTooltip CheckIsMessageState(this CalendarRecordTooltip tooltip) => tooltip.CheckSelectedButton(SelectedButtonType.Message);
        public static CalendarRecordTooltip CheckIsCompletedState(this CalendarRecordTooltip tooltip) => tooltip.CheckSelectedButton(SelectedButtonType.Completed);
        
        private static CalendarRecordTooltip CheckSelectedButton(this CalendarRecordTooltip tooltip, SelectedButtonType type)
        {
            tooltip.ActiveButtonSelected.WaitPresence(
                type == SelectedButtonType.Active,
                $"ожидаем {(type == SelectedButtonType.Active ? "наличие" : "отсутствие")} выбранной кнопки 'Записали'"
            );

            tooltip.MessageButtonSelected.WaitPresence(
                type == SelectedButtonType.Message, 
                $"ожидаем {(type == SelectedButtonType.Message ? "наличие" : "отсутствие")} выбранной кнопки 'Напомнили'"
            );
            
            tooltip.CompletedButtonSelected.WaitPresence(
                type == SelectedButtonType.Completed, 
                $"ожидаем {(type == SelectedButtonType.Completed ? "наличие" : "отсутствие")} выбранной кнопки 'Выполнили'"
            );
            
            return tooltip;
        }
        
        private enum SelectedButtonType
        {
            Active,
            Message,
            Completed
        }
    }
}
