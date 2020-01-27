using System.Threading.Tasks;

using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkerListExtensions;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkingCalendarExtensions;
using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Utils.Extensions;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.WorkingCalendarTests
{
    public class WorkingCalendarPlaceholderTests : WorkingCalendarTestBase
    {
        [Test]
        [Description("Создание графика с подстановкой времени из последнего заполненного дня")]
        public async Task CheckExistingTimePeriodAppearsInEditor()
        {
            var worker = await CreateWorker();

            var middleOfMonth = DateHelper.GetMiddleOfMonth(Now);
            await BuildOneWorkingDay(worker.Id, middleOfMonth.AddDays(-2), GetTimePeriod(10, 13));
            await BuildOneWorkingDay(worker.Id, middleOfMonth.AddDays(-1), GetTimePeriod(8, 22));

            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();

            var editor0 = page.OpenEditor(0, 0).GetEditingView();
            editor0.CheckTimeRange("08:00", "22:00");

            var editor1 = page.OpenEditor(0, middleOfMonth.Day).GetEditingView();
            editor1.CheckTimeRange("08:00", "22:00");
        }

        [Test]
        [Description("Несколько периодов подставляются в редактор корректно")]
        public async Task MultiplePlaceholder()
        {
            var worker = await CreateWorker();

            await BuildOneWorkingDay(worker.Id, 
                                     DateHelper.GetFirstDayOfMonth(Now),
                                     TimePeriod.CreateByHours(10, 11),
                                     TimePeriod.CreateByHours(12, 13),
                                     TimePeriod.CreateByHours(14, 15),
                                     TimePeriod.CreateByHours(16, 17),
                                     TimePeriod.CreateByHours(18, 19)
            );
            
            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();
            var editor = page.OpenEditor(0, 10).GetEditingView();
            
            editor.CheckTimeRange(10, 11, atRow: 0);
            editor.CheckTimeRange(12, 13, atRow: 1);
            editor.CheckTimeRange(14, 15, atRow: 2);
            editor.CheckTimeRange(16, 17, atRow: 3);
            editor.CheckTimeRange(18, 19, atRow: 4);
        }

        [Test]
        [Description("Ночная смена и обычная простовляются корректно")]
        public async Task NightAndNormalTimePeriods()
        {
            var worker = await CreateWorker();
            await BuildOneWorkingDay(worker.Id,
                                     DateHelper.GetFirstDayOfMonth(Now),
                                     TimePeriod.CreateByHours(10, 11),
                                     TimePeriod.CreateByHours(15, 24)
            );
            await BuildOneWorkingDay(worker.Id,
                                     DateHelper.GetFirstDayOfMonth(Now).AddDays(1),
                                     TimePeriod.CreateByHours(0, 7));
            
            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();
            var editor = page.OpenEditor(0, 10).GetEditingView();

            editor.CheckTimeRange(10, 11, atRow: 0);
            editor.CheckTimeRange(15, 7, atRow: 1);
        }

        [Test]
        [Description("Последний день предыдущего месяца и первый день следующего не подставляются как placeholder")]
        public async Task BorderDaysDoesNotAppearAsPlaceholder()
        {
            var worker = await CreateWorker();
            await BuildOneWorkingDay(worker.Id, DateHelper.GetFirstDayOfMonth(Now).AddDays(-1), TimePeriod.CreateByHours(10, 11));
            await BuildOneWorkingDay(worker.Id, DateHelper.GetFirstDayOfNextMonth(Now), TimePeriod.CreateByHours(12, 13));
            
            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();
            var editor = page.OpenEditor(0, 10).GetEditingView();
            
            editor.CheckTimeRange("", "");
        }
    }
}
