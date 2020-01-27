using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkerListExtensions;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkingCalendarExtensions;
using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Utils.Extensions;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.WorkingCalendarTests
{
    public class WorkingCalendarFillingModeTests : WorkingCalendarTestBase
    {
        [Test]
        [Description("Создание графика 5/2 с ночной и обычной сменой")]
        public async Task CreateFiveByTwoWorkingCalendarWithManyPeriods()
        {
            var worker = await CreateWorker();

            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();

            const int startIdx = 15;
            var editingView = page.OpenEditor(0, startIdx).GetEditingView();
            editingView.SetTimeRange("10:00", "12:00");
            editingView.SetTimeRange("13:00", "09:00", 1);
            editingView.SetCalendarMode(CalendarFillingMode.FiveByTwo);
            editingView.ClickSave();

            var startDate = DateHelper.GetFirstDayOfMonth(Now).AddDays(startIdx);
            CheckMonthFor5By2From(page, startDate);

            for(var date = DateHelper.GetFirstDayOfMonth(Now); date <= DateHelper.GetLastDayOfMonth(Now); date = date.AddDays(1))
            {
                var day = await workerScheduleRepository.ReadWorkerCalendarDayAsync(shop.Id, date, worker.Id);
                var nextDay = await workerScheduleRepository.ReadWorkerCalendarDayAsync(shop.Id, date.AddDays(1), worker.Id);

                var shouldBeEmpty = date < startDate || IsWeekend(CalendarFillingMode.FiveByTwo, date, date.Day - startDate.Day);

                day.Records
                   .Select(x => x.Period)
                   .Count(p => p == TimePeriod.CreateByHours(10, 12))
                   .Should().Be(shouldBeEmpty ? 0 : 1);

                day.Records
                   .Select(x => x.Period)
                   .Count(p => p == TimePeriod.CreateByHours(13, 24))
                   .Should().Be(shouldBeEmpty ? 0 : 1);

                nextDay.Records
                       .Select(x => x.Period)
                       .Count(p => p == TimePeriod.CreateByHours(0, 9))
                       .Should().Be(shouldBeEmpty ? 0 : 1);
            }
        }

        [Test]
        [Description("Расширение хвоста предыдущей ночной смены НЕ тиражируются, тиражируется обычный период")]
        public async Task ExtendNightPeriodAndAddOtherShouldRepeatOtherOnly()
        {
            var worker = await CreateWorker();

            await BuildOneWorkingDay(worker.Id, DateHelper.GetFirstDayOfMonth(Now), TimePeriod.CreateByHours(20, 24));
            await BuildOneWorkingDay(worker.Id, DateHelper.GetFirstDayOfMonth(Now).AddDays(1), TimePeriod.CreateByHours(0, 10));
            
            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();

            var editingView = page.OpenEditor(0, 1).GetEditingView();
            editingView.SetTimeRange("10:00", "11:00");
            editingView.SetTimeRange("12:00", "13:00", 1);
            editingView.SetCalendarMode(CalendarFillingMode.TwoByTwo);
            editingView.ClickSave();

            page.GetCell(0, 0).WaitFilled();
            CheckMonthFor2By2From(page, DateHelper.GetFirstDayOfMonth(Now).AddDays(1));

            var startDate = DateHelper.GetFirstDayOfMonth(Now).AddDays(1);

            var editedDay = await workerScheduleRepository.ReadWorkerCalendarDayAsync(shop.Id, startDate, worker.Id);
            editedDay.Records
                     .Select(x => x.Period)
                     .Count(p => p == TimePeriod.CreateByHours(0, 11))
                     .Should().Be(1);

            editedDay.Records
                     .Select(x => x.Period)
                     .Count(p => p == TimePeriod.CreateByHours(12, 13))
                     .Should().Be(1);

            for(var date = startDate.AddDays(1); date <= DateHelper.GetLastDayOfMonth(Now); date = date.AddDays(1))
            {
                var day = await workerScheduleRepository.ReadWorkerCalendarDayAsync(shop.Id, date, worker.Id);
                var shouldBeEmpty = IsWeekend(CalendarFillingMode.TwoByTwo, date, date.Day - startDate.Day);

                day.Records.Length.Should().Be(shouldBeEmpty ? 0 : 2);

                if(!shouldBeEmpty)
                {
                    day.Records.Select(x => x.Period).Should().BeEquivalentTo(
                        TimePeriod.CreateByHours(10, 11),
                        TimePeriod.CreateByHours(12, 13)
                    );
                }
            }
        }

        [Test]
        [Description("Переодическая ночная смена не модифицирует данные 1 числа следующего месяца")]
        public async Task PeriodicNightPeriodDoesNotAffectNextMonth()
        {
            var worker = await CreateWorker();
            await BuildOneWorkingDay(worker.Id, DateHelper.GetFirstDayOfNextMonth(Now), TimePeriod.CreateByHours(20, 23));
            
            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();
            
            var editingView = page.OpenEditor(0, DateHelper.GetLastDayOfMonth(Now).Day - 2).GetEditingView();
            editingView.SetTimeRange("21:00", "10:00");
            editingView.SetCalendarMode(CalendarFillingMode.TwoByTwo);
            editingView.ClickSave();

            var day = await workerScheduleRepository.ReadWorkerCalendarDayAsync(shop.Id, DateHelper.GetFirstDayOfNextMonth(Now), worker.Id);

            day.Records.Select(x => x.Period).Should().BeEquivalentTo(TimePeriod.CreateByHours(0, 10), TimePeriod.CreateByHours(20, 23));
        }

        [Test]
        [Description("Удаление данных тиражированием стирает все на своем пути в рамках месяца")]
        public async Task PeriodicClearingRemoveAll()
        {
            var worker = await CreateWorker();

            await BuildOneWorkingDay(worker.Id, DateHelper.GetLastDayOfMonth(Now).AddDays(-3), TimePeriod.CreateByHours(21, 23));
            await BuildOneWorkingDay(worker.Id, DateHelper.GetLastDayOfMonth(Now), TimePeriod.CreateByHours(20, 24));
            await BuildOneWorkingDay(worker.Id, DateHelper.GetFirstDayOfNextMonth(Now), TimePeriod.CreateByHours(0, 11), TimePeriod.CreateByHours(20, 23));
            
            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();
            
            var editingView = page.OpenEditor(0, 5).GetEditingView();
            editingView.SetTimeRange("", "", 0);
            editingView.SetTimeRange("", "", 1);
            editingView.SetTimeRange("", "", 2);
            editingView.SetTimeRange("", "", 3);
            editingView.SetTimeRange("", "", 4);
            editingView.SetCalendarMode(CalendarFillingMode.FiveByTwo);
            editingView.ClickSave();
            
            CheckWholeMonth(page, Now, CalendarFillingMode.Weekend);
            
            var month = await workerScheduleRepository.ReadShopCalendarMonthAsync(shop.Id, Now);

            month.ShopCalendarDays
                 .SelectMany(x => x.WorkerCalendarDays)
                 .Where(x => x.WorkerId == worker.Id)
                 .All(x => x.Records.Length == 0)
                 .Should()
                 .BeTrue();
            
            var nextMonthFirstDay = await workerScheduleRepository.ReadWorkerCalendarDayAsync(shop.Id, DateHelper.GetFirstDayOfNextMonth(Now), worker.Id);
            nextMonthFirstDay.Records.Select(x => x.Period).Should().BeEquivalentTo(TimePeriod.CreateByHours(20, 23));
        }
    }
}
