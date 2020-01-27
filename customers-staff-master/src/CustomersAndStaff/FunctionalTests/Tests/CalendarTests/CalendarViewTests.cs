using System.Threading.Tasks;

using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.CalendarPageExtensions;
using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Utils.Extensions;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.CalendarTests
{
    public class CalendarViewTests : CalendarTestBase
    {
        [Test]
        [Description("Пустой список сотрудников в календаре записи")]
        public void EmptyCalendar()
        {
            var page = LoadMainPage().GoToCalendarPage();
            page.WorkerColumn.Count.Wait().EqualTo(0);
            page.EmptyCalendarMessage.WaitPresence();
        }

        [Test]
        [Description("Фильтрация 'Все сотрудники/Работающие'")]
        public async Task WorkingCalendarFilter()
        {
            await CreateWorker();
            var workerId = await CreateWorker();
            await CreateOneWorkingDay(workerId, DateHelper.Tomorrow(), TimePeriod.CreateByHours(10, 16));
            
            var page = GoToTomorrowCalendarPage();

            page.CheckWorkingFilter(WorkerFilter.AllWorkers)
                .CheckWorkerColumnCount(2)
                .SetWorkingFilter(WorkerFilter.OnlyWithWorkingDays)
                .CheckWorkerColumnCount(1);
        }

        [Test]
        [Description("Первоначальная настройка времени календаря, смена часового пояса приводит к перемещению красной линии текущего времени")]
        public async Task RedLineByTimeZoneTest()
        {
            await CreateWorker();
            
            var page = LoadMainPage().GoToCalendarPage();
            page.DataLoaded.WaitPresence();

            page.OpenTimeZoneEditor()
                .ChangeTimeZone(3)
                .SaveAndClose();
            
            page.NowLine.WaitPresence();
            var top = page.NowLine.Top.Get();
            
            page.OpenTimeZoneEditor()
                .ChangeTimeZone(5)
                .SaveAndClose();

            page.NowLine.WaitTop(top + 72, 2); // 72 - магическая константа в пикселях, отвечающая за
                                                           // смещение красной полоски при смене часового пояса
        }
    }
}
