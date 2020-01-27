using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkerListExtensions;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkingCalendarExtensions;
using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.Utils.Extensions;

using NUnit.Framework;

using CalendarFillingMode = Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkingCalendarExtensions.CalendarFillingMode;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.WorkingCalendarTests
{
    public class WorkingCalendarCreationTests : WorkingCalendarTestBase
    {
        [Test]
        [Description("Создание графика. Рабочий день отмечается в календаре. Время нестандартное")]
        public async Task CreateSingleDayShouldBeVisible()
        {
            const string startTime = "10:19";
            const string endTime = "21:48";

            var workers = new[] {new Worker {FullName = "Василий"}};
            await workerRepository.CreateManyAsync(shop.Id, workers);

            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();

            page.CheckWorkerItems(workers);

            var cellIdx = Now.Day - 1;
            page.GetCell(0, cellIdx).WaitNotFilled();

            var editingView = page.OpenEditor(0, cellIdx).GetEditingView();

            editingView.SetTimeRange(startTime, endTime);
            editingView.ClickSave();

            page.GetCell(0, cellIdx).WaitFilled();
            var editor = page.OpenEditor(0, cellIdx);

            var dateText = Now.GetDayAndFullMonth();
            var dayOfWeekName = Now.GetDayOfWeekName().ToLower();

            editor.HeaderDate.WaitText(dateText);
            editor.HeaderDayOfWeek.WaitText(dayOfWeekName);

            editor.GetInfoView().CheckStartEndTime(startTime, endTime);
        }

        [Description("Проверяем создание рабочего с краевыми значениями времен")]
        [TestCase("00:00", "24:00", 0, 24)]
        [TestCase("24:00", "00:00", 0, 24)]
        [TestCase("00:00", "00:00", 0, 24)]
        [TestCase("24:00", "24:00", 0, 24)]

        [TestCase("24:00", "12:00", 0, 12)]
        [TestCase("12:00", "00:00", 12, 24)]
        public async Task EdgeTimesInPeriod(string startTime, string endTime, int expectedStart, int expectedEnd)
        {
            var worker = await CreateWorker();
            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();
            var editingView = page.OpenEditor(0, 0).GetEditingView();

            editingView.SetTimeRange(startTime, endTime);
            editingView.ClickSave();

            var createdDay = await workerScheduleRepository.ReadWorkerCalendarDayAsync(shop.Id, DateHelper.GetFirstDayOfMonth(Now), worker.Id);
            createdDay.Records.Length.Should().Be(1);
            createdDay.Records.First().Period.Should().BeEquivalentTo(TimePeriod.CreateByHours(expectedStart, expectedEnd));
        }

        [Test]
        [Description("Создание графика 2/2")]
        public async Task CreateTwoByTwoWorkingCalendar()
        {
            await CreateWorker();

            var page = LoadMainPage().GoToWorkerListPage();
            page.OpenCalendar();
            page.MonthName.WaitText(Now.GetFullMonthName());

            const int startIdx = 15;
            var editingView = page.OpenEditor(0, startIdx).GetEditingView();
            editingView.SetTimeRange();
            editingView.SetCalendarMode(CalendarFillingMode.TwoByTwo);
            editingView.ClickSave();

            CheckMonthFor2By2From(page, DateHelper.GetFirstDayOfMonth(Now).AddDays(startIdx));
        }

        [Test]
        [Description("Создание графика 5/2")]
        public async Task CreateFiveByTwoWorkingCalendar()
        {
            await CreateWorker();

            var page = LoadMainPage().GoToWorkerListPage();
            page.OpenCalendar();

            const int startIdx = 15;
            var editingView = page.OpenEditor(0, startIdx).GetEditingView();
            editingView.SetTimeRange();
            editingView.SetCalendarMode(CalendarFillingMode.FiveByTwo);
            editingView.ClickSave();

            CheckMonthFor5By2From(page, DateHelper.GetFirstDayOfMonth(Now).AddDays(startIdx));
        }

        [Test]
        [Description("Создание графика, когда в базе два сотрудника: один уже с графиком и они не пересекаются.")]
        public async Task CreateSingleDayWhenTwoWorkers()
        {
            #region Create workers

            var workers = new[] {new Worker {FullName = "Василий"}, new Worker {FullName = "Петр"}};
            var createdWorkers = await workerRepository.CreateManyAsync(shop.Id, workers);

            var middleOfMonth = DateHelper.GetMiddleOfMonth(Now);
            var calendarDay = new WorkerCalendarDay<WorkerScheduleRecord>
                {
                    WorkerId = createdWorkers[0].Id,
                    Date = middleOfMonth,
                    Records = new[]
                        {
                            new WorkerScheduleRecord
                                {
                                    Period = GetTimePeriod(8, 22)
                                }
                        }
                };
            await workerScheduleRepository.WriteAsync(shop.Id, calendarDay);

            #endregion

            var page = LoadMainPage().GoToWorkerListPage();
            page.OpenCalendar();
            page.CheckWorkerItems(workers);

            const int startHours = 10;
            const int endHours = 17;
            var editingView = page.OpenEditor(1, Now.Day - 1).GetEditingView();
            editingView.SetTimeRange($"{startHours:00}:00", $"{endHours:00}:00");
            editingView.ClickSave();

            var createdDay = await workerScheduleRepository.ReadWorkerCalendarDayAsync(shop.Id, middleOfMonth, createdWorkers[0].Id);
            createdDay.Records.Single().Period.Should()
                      .BeEquivalentTo(GetTimePeriod(8, 22));
            
            createdDay = await workerScheduleRepository.ReadWorkerCalendarDayAsync(shop.Id, Now, createdWorkers[1].Id);
            createdDay.Records.Single().Period.Should()
                      .BeEquivalentTo(GetTimePeriod(startHours, endHours));
        }

        [Test]
        [Description("Периодический график строится на месяц, следующий месяц не задет")]
        public async Task CreationPeriodicalDoesNotAffectNextMonth()
        {
            var worker = await CreateWorker();
            var firstDatePrevMonth = DateHelper.GetFirstDayOfPreviousMonth(Now);
            await BuildCalendar2By2(worker.Id, firstDatePrevMonth, GetDefaultTimePeriod());

            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar().GoToPrevMonth();

            CheckMonthFor2By2From(page, firstDatePrevMonth);

            page.GoToNextMonth();

            CheckWholeMonth(page, Now, CalendarFillingMode.Weekend);
        }

        [Test]
        [Description("Создание графика с отменой создания")]
        public async Task TryCreateButCancelShouldNotCreateAnything()
        {
            await CreateWorker();

            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();
            var todayCellIdx = Now.Day - 1;
            page.GetCell(0, todayCellIdx).WaitNotFilled("ожидаем что сегодня не рабочий день");
            var editingView = page.OpenEditor(0, todayCellIdx).GetEditingView();
            editingView.SetTimeRange("10:00", "21:00");
            editingView.SetCalendarMode(CalendarFillingMode.TwoByTwo);
            editingView.ClickCancel();
            page.GetCell(0, todayCellIdx).WaitNotFilled("ожидаем что сегодня не рабочий день");
        }

        [Test]
        [Description("Валидация полей рабочего времени")]
        public async Task SingleTimeRangeValidation()
        {
            await CreateWorker();

            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();

            var editor = page.OpenEditor(0, Now.Day - 1).GetEditingView();

            editor.SetTimeRange("1", "10:00");
            OpenValidation(editor, PresenceBy.Unfocus);
            CheckValidationMessage(editor, "В земных сутках нет такого времени", ErrorAt.Left);
            editor.SetTimeRange("10:00", "1");
            OpenValidation(editor, PresenceBy.Unfocus);
            CheckValidationMessage(editor, "В земных сутках нет такого времени", ErrorAt.Right);

            editor.SetTimeRange("11", "10:00");
            OpenValidation(editor, PresenceBy.Unfocus);
            CheckValidationMessage(editor, "В земных сутках нет такого времени", ErrorAt.Left);
            editor.SetTimeRange("10:00", "11");
            OpenValidation(editor, PresenceBy.Unfocus);
            CheckValidationMessage(editor, "В земных сутках нет такого времени", ErrorAt.Right);

            editor.SetTimeRange("11:1", "10:00");
            OpenValidation(editor, PresenceBy.Unfocus);
            CheckValidationMessage(editor, "В земных сутках нет такого времени", ErrorAt.Left);
            editor.SetTimeRange("10:00", "11:1");
            OpenValidation(editor, PresenceBy.Unfocus);
            CheckValidationMessage(editor, "В земных сутках нет такого времени", ErrorAt.Right);

            editor.SetTimeRange("25:11", "10:00");
            OpenValidation(editor, PresenceBy.Unfocus);
            CheckValidationMessage(editor, "В земных сутках нет такого времени", ErrorAt.Left);
            editor.SetTimeRange("10:00", "25:11");
            OpenValidation(editor, PresenceBy.Unfocus);
            CheckValidationMessage(editor, "В земных сутках нет такого времени", ErrorAt.Right);

            editor.SetTimeRange("11:75", "10:00");
            OpenValidation(editor, PresenceBy.Unfocus);
            CheckValidationMessage(editor, "В земных сутках нет такого времени", ErrorAt.Left);
            editor.SetTimeRange("10:00", "11:75");
            OpenValidation(editor, PresenceBy.Unfocus);
            CheckValidationMessage(editor, "В земных сутках нет такого времени", ErrorAt.Right);

            editor.SetTimeRange("", "10:00");
            OpenValidation(editor, PresenceBy.Submit);
            CheckValidationMessage(editor, "Укажите время", ErrorAt.Left);
            editor.SetTimeRange("10:00", "");
            OpenValidation(editor, PresenceBy.Submit);
            CheckValidationMessage(editor, "Укажите время", ErrorAt.Right);

            editor.SetTimeRange("10:00", "10:14");
            OpenValidation(editor, PresenceBy.Submit);
            CheckValidationMessage(editor, "Нельзя сделать запись короче 15 минут", ErrorAt.Both);
            
            editor.SetTimeRange("00:00", "00:14");
            OpenValidation(editor, PresenceBy.Submit);
            CheckValidationMessage(editor, "Нельзя сделать запись короче 15 минут", ErrorAt.Both);
            
            editor.SetTimeRange("24:00", "00:14");
            OpenValidation(editor, PresenceBy.Submit);
            CheckValidationMessage(editor, "Нельзя сделать запись короче 15 минут", ErrorAt.Both);
            
            editor.SetTimeRange("23:46", "24:00");
            OpenValidation(editor, PresenceBy.Submit);
            CheckValidationMessage(editor, "Нельзя сделать запись короче 15 минут", ErrorAt.Both);
            
            editor.SetTimeRange("23:46", "00:00");
            OpenValidation(editor, PresenceBy.Submit);
            CheckValidationMessage(editor, "Нельзя сделать запись короче 15 минут", ErrorAt.Both);
        }

        [Test]
        [Description("Создание графика в текущем месяце + 12 месяцев")]
        public async Task CreateInSameMonthNextYear()
        {
            var workers = new[] {new Worker {FullName = "Василий"}};
            await workerRepository.CreateManyAsync(shop.Id, workers);

            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();
            for(var i = 0; i < 12; i++) page.GoToNextMonth();

            var editor = page.OpenEditor(0, 0).GetEditingView();
            editor.SetTimeRange("10:00", "21:00");
            editor.SetCalendarMode(CalendarFillingMode.FiveByTwo);
            editor.ClickSave();

            CheckMonthFor5By2From(page, DateHelper.GetFirstDayOfMonth(Now.AddYears(1)));
        }

        [Test]
        [Description("Сортировка сотрудников по должности, потом по имени")]
        public async Task SortingTest()
        {
            #region Create workers

            var workers = new[]
                {
                    new Worker {FullName = "Я сотрудник", Position = "А должность"},
                    new Worker {FullName = "А сотрудник", Position = "г должность"},
                    new Worker {FullName = "в сотрудник", Position = "Г должность"},
                    new Worker {FullName = "А сотрудник"},
                    new Worker {FullName = "Я сотрудник"},
                };
            await workerRepository.CreateManyAsync(shop.Id, workers.AsEnumerable().Reverse().ToArray());

            #endregion

            var page = LoadMainPage().GoToWorkerListPage();
            page.CheckWorkerItems(workers);
        }
        
        [Test]
        [Description("Валидация полей рабочего времени - несколько периодов")]
        public async Task MultilineTimeRangeValidation()
        {
            await CreateWorker();

            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();

            var editor = page.OpenEditor(0, Now.Day - 1).GetEditingView();

            editor.SetTimeRange("1", "10:00");
            OpenValidation(editor, PresenceBy.Unfocus);
            CheckValidationMessage(editor, "В земных сутках нет такого времени", ErrorAt.Left);

            editor.SetTimeRange("10:00", "1", 1);
            OpenValidation(editor, PresenceBy.Unfocus);
            CheckValidationMessage(editor, "В земных сутках нет такого времени", ErrorAt.Right, 1);

            editor.SetTimeRange("11", "10:00", 2);
            OpenValidation(editor, PresenceBy.Unfocus);
            CheckValidationMessage(editor, "В земных сутках нет такого времени", ErrorAt.Left, 2);

            editor.SetTimeRange("10:00", "", 3);
            OpenValidation(editor, PresenceBy.Submit);
            CheckValidationMessage(editor, "Укажите время", ErrorAt.Right, 3);

            editor.SetTimeRange("10:00", "10:14", 4);
            OpenValidation(editor, PresenceBy.Submit);
            CheckValidationMessage(editor, "Нельзя сделать запись короче 15 минут", ErrorAt.Both, 4);
        }

        [Test]
        [Description("Сохраняем 2 периода времени, не пересекаются, без ночной смены")]
        public async Task CreateSingleDayTwoRecordsShouldBeVisible()
        {
            const string startTime0 = "10:00";
            const string endTime0 = "12:00";

            const string startTime1 = "13:00";
            const string endTime1 = "21:00";

            var worker = await CreateWorker();

            var page = LoadMainPage().GoToWorkerListPage();
            page.OpenCalendar();

            var cellIdx = Now.Day - 1;
            page.GetCell(0, cellIdx).WaitNotFilled();

            var editingView = page.OpenEditor(0, cellIdx).GetEditingView();

            editingView.SetTimeRange(startTime0, endTime0);
            editingView.SetTimeRange(startTime1, endTime1, 1);
            editingView.ClickSave();

            var workerDay = await workerScheduleRepository.ReadWorkerCalendarDayAsync(shop.Id, Now.Date, worker.Id);

            workerDay.Records.Should().BeEquivalentTo(
                new WorkerScheduleRecord {Period = TimePeriod.CreateByHours(10, 12)},
                new WorkerScheduleRecord {Period = TimePeriod.CreateByHours(13, 21)}
            );
        }
        
        [Test]
        [Description("Сохраняем третий период времени, объединяющий два других в один, в рамках одного дня")]
        public async Task CreateRecordWithJoiningTwoPeriods()
        {
            var worker = await CreateWorker();
            await BuildOneWorkingDay(worker.Id, Now.Date, TimePeriod.CreateByHours(10, 12), TimePeriod.CreateByHours(13, 21));

            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();

            var cellIdx = Now.Day - 1;
            page.GetCell(0, cellIdx).WaitFilled();

            var editingView = page.OpenEditor(0, cellIdx).GetEditingView();

            editingView.SetTimeRange("11:00", "13:35", 4);
            editingView.ClickSave();

            var workerDay = await workerScheduleRepository.ReadWorkerCalendarDayAsync(shop.Id, Now.Date, worker.Id);

            workerDay.Records.Should().BeEquivalentTo(
                new WorkerScheduleRecord {Period = TimePeriod.CreateByHours(10, 21)}
            );
        }
    }
}
