using System.Linq;

using Kontur.Selone.Extensions;

using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Calendar;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;

namespace Market.CustomersAndStaff.FunctionalTests.PageExtensions.CalendarPageExtensions
{
    public static class WorkerColumnExtensions
    {
        public static void ClickOnRecord(this WorkerColumn column, int idx)
        {
            column.Record.Count.Wait($"ожидаем отображения хотя бы {idx + 1} записей").MoreOrEqual(idx + 1);
            column.Record.ElementAt(idx).Container.Click();
        }
        
        public static void ClickOnHourDelimiter(this WorkerColumn column, int hours)
        {
            var above = true;
            if(hours >= 23)
            {
                hours = 22;
                above = false;
            }
            var line = column.HourDelimiter.ElementAt(hours).Container;
            line.ScrollIntoView();
            line.Action((actions, element) => actions.MoveToElement(element)
                                                     .MoveByOffset(0, above ? -30 : 5)
                                                     .Click());
        }
    }
}
