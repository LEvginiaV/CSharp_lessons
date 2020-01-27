using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkerListExtensions;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkingCalendarExtensions;
using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Utils.Extensions;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.WorkingCalendarTests
{
    public class WorkingCalendarManyPeriodsManipulationsTests : WorkingCalendarTestBase
    {
        [Test]
        [Description("Проверяем максимальное количество добавляемых периодов и наличие иконок удаления")]
        public async Task CheckMaxAvailablePeriods()
        {
            await CreateWorker();
            
            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();
            var editor = page.OpenEditor(0, 10).GetEditingView();

            while(editor.TimeRangeLine.First().AddLink.IsPresent.Get())
            {
                editor.TimeRangeLine.First().AddLink.Click();
            }
            
            editor.TimeRangeLine.WaitCount(5);
            editor.TimeRangeLine.WaitAll(x => x.DeleteLink.IsPresent);
        }

        [Test]
        [Description("Удаляем среднее занчение из нескольких периодов, проверяем корректность сохранения и отображения")]
        public async Task DeleteMiddlePeriod()
        {
            var periods = new[]
                {
                    TimePeriod.CreateByHours(10, 11),
                    TimePeriod.CreateByHours(12, 13),
                    TimePeriod.CreateByHours(14, 15),
                    TimePeriod.CreateByHours(16, 17),
                    TimePeriod.CreateByHours(18, 19)
                };
            var expectedPeriods = periods.Take(2).Concat(periods.Skip(3)).ToArray();

            var worker = await CreateWorker();
            await BuildOneWorkingDay(worker.Id, DateHelper.GetFirstDayOfMonth(Now), periods);
            
            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();
            var editor = page.OpenEditor(0, 0).GetEditingView();
            
            editor.TimeRangeLine.ElementAt(2).DeleteLink.Click();
            editor.TimeRangeLine
                  .Select(x => x.ConvertToTimePeriod())
                  .Wait()
                  .ShouldBeEquivalentTo(expectedPeriods);
            editor.ClickSave();

            var day = await workerScheduleRepository.ReadWorkerCalendarDayAsync(shop.Id, DateHelper.GetFirstDayOfMonth(Now), worker.Id);
            day.Records.Select(x => x.Period).Should().BeEquivalentTo(expectedPeriods);
        }
    }
}
