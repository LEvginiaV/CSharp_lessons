using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Market.CustomersAndStaff.FunctionalTests.Helpers;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkerListExtensions;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkingCalendarExtensions;
using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.Utils.Extensions;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.WorkingCalendarTests
{
    public class WorkingCalendarEditingTests : WorkingCalendarTestBase
    {
        [Test]
        [Description("Рабочий день превращаем в выходной с помощью кнопки Сделать выходным")]
        public async Task ClearSimpleWorkingDay()
        {
            var worker = await CreateWorker();
            await BuildOneWorkingDay(worker.Id, Now, GetDefaultTimePeriod());

            var page = LoadMainPage().GoToWorkerListPage();
            page.OpenCalendar();
            var dayIdx = Now.Day - 1;
            page.OpenEditor(0, dayIdx).ClickMakeDayOff();
            page.GetCell(0, dayIdx).WaitNotFilled();
        }

        [Test]
        [Description("Рабочий день (с ночной сменой и хвостом) превращаем в выходной с помощью кнопки Сделать выходным")]
        public async Task ClearDifficultWorkingDay()
        {
            // Проверяем, что удаляем ночную смену - не остаётся хвост.
            // Не удаляет ночную смену, если чистим день с хвостом от ночной смены
            var worker = await CreateWorker();
            await BuildOneWorkingDay(worker.Id,
                                     DateHelper.GetFirstDayOfMonth(Now),
                                     TimePeriod.CreateByHours(22, 24));
            await BuildOneWorkingDay(worker.Id,
                                     DateHelper.GetFirstDayOfMonth(Now).AddDays(1),
                                     TimePeriod.CreateByHours(0, 10),
                                     TimePeriod.CreateByHours(18, 24)
            );
            await BuildOneWorkingDay(worker.Id,
                                     DateHelper.GetFirstDayOfMonth(Now).AddDays(2),
                                     TimePeriod.CreateByHours(0, 9),
                                     TimePeriod.CreateByHours(12, 14)
            );

            var page = LoadMainPage().GoToWorkerListPage();
            page.OpenCalendar();
            page.OpenEditor(0, 1).ClickMakeDayOff();
            page.GetCell(0, 1).WaitNotFilled();

            var day0 = await workerScheduleRepository.ReadWorkerCalendarDayAsync(shop.Id, DateHelper.GetFirstDayOfMonth(Now), worker.Id);
            var day1 = await workerScheduleRepository.ReadWorkerCalendarDayAsync(shop.Id, DateHelper.GetFirstDayOfMonth(Now).AddDays(1), worker.Id);
            var day2 = await workerScheduleRepository.ReadWorkerCalendarDayAsync(shop.Id, DateHelper.GetFirstDayOfMonth(Now).AddDays(2), worker.Id);

            day0.Records.Select(x => x.Period).Should().BeEquivalentTo(TimePeriod.CreateByHours(22, 24));
            day1.Records.Select(x => x.Period).Should().BeEquivalentTo(TimePeriod.CreateByHours(0, 10));
            day2.Records.Select(x => x.Period).Should().BeEquivalentTo(TimePeriod.CreateByHours(12, 14));
        }

        [Test]
        [Description("Рабочий день превращаем в выходной, удаляя время")]
        public async Task ClearWorkingDayByDeletingTime()
        {
            var worker = new[] {new Worker {FullName = "Василий"}};
            var created = await workerRepository.CreateManyAsync(shop.Id, worker);
            await BuildOneWorkingDay(created[0].Id, Now, GetDefaultTimePeriod());

            var page = LoadMainPage().GoToWorkerListPage();
            page.OpenCalendar();
            var dayIdx = Now.Day - 1;
            var editor = page.OpenEditor(0, dayIdx).GetEditingView();
            editor.SetTimeRange("", "");
            editor.ClickSave();
            page.GetCell(0, dayIdx).WaitNotFilled();
        }

        [Test]
        [Description("Изменение рабочего времени")]
        public async Task UpdateTime()
        {
            var worker = new[] {new Worker {FullName = "Василий"}};
            var created = await workerRepository.CreateManyAsync(shop.Id, worker);
            await BuildOneWorkingDay(created[0].Id, Now, GetTimePeriod(10, 21));
            var dayIdx = Now.Day - 1;
            
            var page = LoadMainPage().GoToWorkerListPage();
            page.OpenCalendar();
            var editingView = page.OpenEditor(0, dayIdx).GetEditingView();
            editingView.SetTimeRange("12:30", "18:00");
            editingView.ClickSave();
            WaiterHelper.WaitUntil(() => page.CheckStartAndEndTime(0, dayIdx, "12:30", "18:00"));
        }

        [Test]
        [Description("Изменение периодического графика работы на другой периодический")]
        public async Task ChangeFrom2By2To5By2()
        {
            #region Create worker

            var worker = new[] {new Worker {FullName = "Василий"}};
            var created = await workerRepository.CreateManyAsync(shop.Id, worker);
            var firstDateThisMonth = DateHelper.GetFirstDayOfMonth(Now);
            var lastDatePrevMonth = firstDateThisMonth.AddDays(-1);
            var firstDatePrevMonth = DateHelper.GetFirstDayOfMonth(lastDatePrevMonth);

            await BuildCalendar2By2(created[0].Id, firstDatePrevMonth, GetTimePeriod(10, 22));

            #endregion

            var page = LoadMainPage().GoToWorkerListPage();
            page.OpenCalendar();
            page.GoToPrevMonth();

            var editingView = page.OpenEditor(0, 0).GetEditingView();
            editingView.SetTimeRange("10:00", "21:00");
            editingView.SetCalendarMode(CalendarFillingMode.FiveByTwo);
            editingView.ClickSave();

            CheckMonthFor5By2From(page, firstDatePrevMonth);
        }

        [Test]
        [Description("Изменение одного дня на периодический график работы")]
        public async Task ChangeFromSingleDayTo2By2()
        {
            #region Create worker

            var worker = new[] {new Worker {FullName = "Василий"}};
            var created = await workerRepository.CreateManyAsync(shop.Id, worker);

            var middleOfMonth = DateHelper.GetMiddleOfMonth(Now);
            await BuildOneWorkingDay(created[0].Id, middleOfMonth, GetTimePeriod(10, 22));

            #endregion

            var page = LoadMainPage().GoToWorkerListPage();
            page.OpenCalendar();

            var editingView = page.OpenEditor(0, middleOfMonth.Day - 1).GetEditingView();
            editingView.SetTimeRange("10:00", "21:00");
            editingView.SetCalendarMode(CalendarFillingMode.TwoByTwo);
            editingView.ClickSave();

            CheckMonthFor2By2From(page, middleOfMonth);
        }

        [Test]
        [Description("Изменение периодического графика работы на один день")]
        public async Task UpdateOneDayIn2By2()
        {
            #region Create worker

            var worker = new[] {new Worker {FullName = "Василий"}};
            var created = await workerRepository.CreateManyAsync(shop.Id, worker);

            var firstDay = DateHelper.GetFirstDayOfMonth(Now);
            var firstDayPrevMonth = DateHelper.GetFirstDayOfMonth(firstDay.AddDays(-1));

            await BuildCalendar2By2(created[0].Id, firstDayPrevMonth, GetTimePeriod(9, 18));

            #endregion

            var page = LoadMainPage().GoToWorkerListPage();
            page.OpenCalendar();
            page.GoToPrevMonth();

            var editingView = page.OpenEditor(0, 1).GetEditingView();
            editingView.SetTimeRange("10:00", "21:00");
            editingView.ClickSave();

            page.OpenEditor(0, 0).GetInfoView().CheckStartEndTime("09:00", "18:00");
            WaiterHelper.WaitUntil(() => page.CheckStartAndEndTime(0, 1, "10:00", "21:00"));
            page.GetCell(0, 3).WaitNotFilled();
        }

        [Test]
        [Description("Редактирование графика работы в будущее: частично 2/2, частично 5/2")]
        public async Task PartialUpdateFrom2By2To5By2()
        {
            #region Create worker

            var worker = new[] {new Worker {FullName = "Василий"}};
            var created = await workerRepository.CreateManyAsync(shop.Id, worker);

            var firstDay = DateHelper.GetFirstDayOfMonth(Now);
            var lastDay = DateHelper.GetLastDayOfMonth(firstDay);

            await BuildCalendar2By2(created[0].Id, firstDay, GetTimePeriod(9, 18));

            #endregion
            
            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();
            var editingView = page.OpenEditor(0, 14).GetEditingView();
            
            editingView.SetTimeRange("12:00", "20:00");
            editingView.SetCalendarMode(CalendarFillingMode.FiveByTwo);
            editingView.ClickSave();
            
            CheckCalendarRange(page, firstDay, firstDay.AddDays(13), CalendarFillingMode.TwoByTwo);
            CheckCalendarRange(page, firstDay.AddDays(14), lastDay, CalendarFillingMode.FiveByTwo);

            var editor0 = page.OpenEditor(0, 13);
            editor0.GetInfoView().CheckStartEndTime("09:00", "18:00");
            editor0.Close();
            
            var editor1 = page.OpenEditor(0, GetFirstWeekday(14));
            editor1.GetInfoView().CheckStartEndTime("12:00", "20:00");
            editor1.Close();
        }

        private static int GetFirstWeekday(int startIndex)
        {
            var firstDay = DateHelper.GetFirstDayOfMonth(DateTime.Now);
            for(var i = 0; i < 3; i++)
            {
                var shifted = firstDay.AddDays(startIndex + i);
                if(shifted.DayOfWeek != DayOfWeek.Saturday && shifted.DayOfWeek != DayOfWeek.Sunday)
                {
                    return startIndex + i;
                }
            }
            return startIndex + 3;
        }
    }
}