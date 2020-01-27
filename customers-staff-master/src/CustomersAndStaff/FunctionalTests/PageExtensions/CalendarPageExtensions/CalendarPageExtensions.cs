using System.Linq;

using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Calendar;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;

namespace Market.CustomersAndStaff.FunctionalTests.PageExtensions.CalendarPageExtensions
{
    public static class CalendarPageExtensions
    {
        public static CalendarPage GoToNextDay(this CalendarPage page)
        {
            page.DataLoaded.WaitPresence(componentDescription: "индикатор загрузки (скрытый, технический)");
            page.NextDay.Click();
            page.DataLoaded.WaitPresence(componentDescription: "индикатор загрузки (скрытый, технический)");
            return page;
        }
        
        public static CalendarPage GoToPrevDay(this CalendarPage page)
        {
            page.DataLoaded.WaitPresence(componentDescription: "индикатор загрузки (скрытый, технический)");
            page.PrevDay.Click();
            page.DataLoaded.WaitPresence(componentDescription: "индикатор загрузки (скрытый, технический)");
            return page;
        }

        public static CalendarPage CheckWorkerColumnCount(this CalendarPage page, int count)
        {
            page.WorkerColumn.Count.Wait($"ожидаем отображения {count} слобцов сотрудников").EqualTo(count);
            return page;
        }

        public static CalendarPage CheckRecordsCount(this CalendarPage page, int colIdx, int count)
        {
            page.WorkerColumn.ElementAt(colIdx).Record.Count.Wait($"ждем количество записей {count} - в колонке с idx: {colIdx}").EqualTo(count);
            return page;
        }

        public static CalendarRecordModal OpenCalendarModalAtHour(this CalendarPage page, int colIdx, int hours)
        {
            page.WorkerColumn.ElementAt(colIdx).ClickOnHourDelimiter(hours);
            return page.WaitModal<CalendarRecordModal>();
        }
        
        public static CalendarRecordTooltip ShowRecordTooltip(this CalendarPage page, int colIdx, int recordIdx)
        {
            page.WorkerColumn.Count.Wait($"ожидаем отображения хотя бы {colIdx + 1} колонок на странице").MoreOrEqual(colIdx + 1);
            page.WorkerColumn.ElementAt(colIdx).ClickOnRecord(recordIdx);
            page.RecordTooltip.WaitPresence(componentDescription: "тултип записи");
            return page.RecordTooltip;
        }

        public static CalendarRecordModal ChangeRecord(this CalendarPage page, int colIdx, int recordIdx)
        {
            var tooltip = page.ShowRecordTooltip(colIdx, recordIdx);
            tooltip.ChangeRecord.Click();
            return page.WaitModal<CalendarRecordModal>();
        }

        public static CalendarPage CheckNameOnRecord(this CalendarPage page, string name, int colIdx, int recordIdx)
        {
            var message = $"ожидаем имя: {name} в column index: {colIdx}, record index: {recordIdx}";
            page.WorkerColumn.ElementAt(colIdx).Record.ElementAt(recordIdx).Text.Wait(message).Contains(name);
            return page;
        }

        public static CalendarPage CheckWorkingFilter(this CalendarPage page, WorkerFilter filterType)
        {
            page.WorkingFilter.Buttons.Count.Wait($"ожидаем появления двух кнопок").EqualTo(2);
            page.WorkingFilter.GetButton((int)filterType)
                         .Checked
                         .Wait($"ожидаем включенный фильтр {filterType.ToString()}").EqualTo(true);
            return page;
        }

        public static CalendarPage SetWorkingFilter(this CalendarPage page, WorkerFilter filterType)
        {
            page.WorkingFilter.Buttons.Count.Wait($"ожидаем появления двух кнопок").EqualTo(2);
            page.WorkingFilter.GetButton((int)filterType).Click();
            return page.CheckWorkingFilter(filterType);
        }
        
        public static TimeZoneModal OpenTimeZoneEditor(this CalendarPage page)
        {
            page.TimeZoneLink.WaitPresence();
            page.TimeZoneLink.Click();
            return page.WaitModal<TimeZoneModal>();
        }
    }

    public enum WorkerFilter
    {
        AllWorkers = 0,
        OnlyWithWorkingDays = 1
    }
}
