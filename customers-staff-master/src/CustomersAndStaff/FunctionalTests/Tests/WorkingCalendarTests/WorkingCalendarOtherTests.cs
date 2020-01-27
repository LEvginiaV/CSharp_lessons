using System;
using System.Linq;
using System.Threading.Tasks;

using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkerListExtensions;
using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.Utils.Extensions;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.WorkingCalendarTests
{
    public class WorkingCalendarOtherTests : WorkingCalendarTestBase
    {
        [Test]
        [Description("Отображение рабочих часов и количества рабочих дней")]
        public async Task CheckDaysAndHoursCounter()
        {
            #region Create worker and working days

            var worker = new[] {new Worker {FullName = "Василий"}};
            var created = await workerRepository.CreateManyAsync(shop.Id, worker);

            var firstDayThisMonth = DateHelper.GetFirstDayOfMonth(Now);
            var middleThisMonth = DateHelper.GetMiddleOfMonth(Now);
            var lastDayThisMonth = DateHelper.GetLastDayOfMonth(Now);

            var lastDayPrevMonth = firstDayThisMonth.AddDays(-1);
            var firstDayNextMonth = lastDayThisMonth.AddDays(1);

            await BuildOneWorkingDay(created[0].Id, lastDayPrevMonth, TimePeriod.CreateByHours(12, 13));

            await BuildOneWorkingDay(created[0].Id, firstDayThisMonth, TimePeriod.CreateByHours(12, 14));
            await BuildOneWorkingDay(created[0].Id, middleThisMonth, TimePeriod.CreateByHours(12, 14));
            await BuildOneWorkingDay(created[0].Id, lastDayThisMonth, TimePeriod.CreateByHours(12, 14));

            await BuildOneWorkingDay(created[0].Id, firstDayNextMonth, new TimePeriod(TimeSpan.FromHours(12), TimeSpan.FromMinutes(14 * 60 + 30)));

            #endregion

            LoadMainPage()
                .GoToWorkerListPage()
                .OpenCalendar()
                .GoToPrevMonth()
                .CheckSingleWorkerCounters("1 ч", "1 дн")
                .GoToNextMonth()
                .CheckSingleWorkerCounters("6 ч", "3 дн")
                .GoToNextMonth()
                .CheckSingleWorkerCounters("2 ч 30 мин", "1 дн");
        }

        [Test]
        [Description("Отсутствие удаленного сотрудника в календаре")]
        public async Task DeletedWorkedDoesNotAppear()
        {
            var workers = new[]
                {
                    new Worker {FullName = "Иван Удаленный", IsDeleted = true},
                    new Worker {FullName = "Василий"}
                };
            await workerRepository.CreateManyAsync(shop.Id, workers);

            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();
            page.CheckWorkerItems(workers.Where(x => !x.IsDeleted).ToArray());
        }

        [Test]
        [Description("Пейджинг")]
        public async Task Paging()
        {
            var workers = Enumerable.Range(0, 31).Select(x => new Worker {FullName = $"Лжедмитрий {PadInt2(x)}"}).ToArray();

            await workerRepository.CreateManyAsync(shop.Id, workers);
            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();

            page.CheckWorkerItems(workers.Take(30).ToArray());

            page.Paging.ForwardLink.Click();

            page.CheckWorkerItems(workers.Skip(30).ToArray());
        }

        [Test]
        [Description("Ограничение даты в будущее")]
        public async Task NextMonthLimit()
        {
            await workerRepository.CreateAsync(shop.Id, new Worker {FullName = "Василий"});
            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();

            for(int i = 0; i < 12; i++)
            {
                page.GoToNextMonth();
            }

            page.MonthName.WaitText($"{Now.GetFullMonthName()} {Now.Year + 1}");

            page.NextMonth.Disabled.Wait().EqualTo(true);
            page.GoToNextMonth();
            page.MonthName.WaitText($"{Now.GetFullMonthName()} {Now.Year + 1}");
        }

        [Test]
        [Description("Сортировка списка сотрудников")]
        public async Task SortingWorkersTest()
        {
            var workers = new[]
                {
                    new Worker {FullName = "Я сотрудник", Position = "А должность"},
                    new Worker {FullName = "А сотрудник", Position = "г должность"},
                    new Worker {FullName = "в сотрудник", Position = "Г должность"},
                    new Worker {FullName = "А сотрудник", Position = null},
                    new Worker {FullName = "Я сотрудник", Position = null},
                };
            
            await workerRepository.CreateManyAsync(shop.Id, workers.Reverse().ToArray());

            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();
            page.CheckWorkerItems(workers);
        }

        private static string PadInt2(int val) => val > 9 ? $"{val}" : $"0{val}";
    }
}
