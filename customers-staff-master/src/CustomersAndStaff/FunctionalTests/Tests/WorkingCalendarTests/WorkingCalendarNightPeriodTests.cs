using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Market.CustomersAndStaff.FunctionalTests.Helpers;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkerListExtensions;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkingCalendarExtensions;
using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Utils.Extensions;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.WorkingCalendarTests
{
    public class WorkingCalendarNightPeriodTests : WorkingCalendarTestBase
    {
        [Test]
        [Description("Сохраняем ночную смену")]
        [TestCase(22, 10, TestName = "<24 hours")]
        [TestCase(22, 22, TestName = "=24 hours")]
        public async Task CreateNightWorkingPeriod(int start, int end)
        {
            var worker = await CreateWorker();

            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();

            var editingView = page.OpenEditor(0, 0).GetEditingView();

            editingView.SetTimeRange($"{start}:00", $"{end}:00");
            editingView.ClickSave();
            
            page.GetCell(0, 0).WaitFilled();
            page.GetCell(0, 1).WaitNotFilled();

            var day0 = await workerScheduleRepository.ReadWorkerCalendarDayAsync(shop.Id, DateHelper.GetFirstDayOfMonth(Now), worker.Id);
            var day1 = await workerScheduleRepository.ReadWorkerCalendarDayAsync(shop.Id, DateHelper.GetFirstDayOfMonth(Now).AddDays(1), worker.Id);

            day0.Records.Select(x => x.Period).Should().BeEquivalentTo(TimePeriod.CreateByHours(start, 24));
            day1.Records.Select(x => x.Period).Should().BeEquivalentTo(TimePeriod.CreateByHours(0, end));
        }
        
        [Test]
        [Description("Сохраняем ночную смену длительностью более 24 часов")]
        public async Task CreateNightWorkingPeriodLonger24H()
        {
            await CreateWorker();

            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();

            var editingView = page.OpenEditor(0, 0).GetEditingView();

            editingView.SetTimeRange("10:00", "10:00");
            editingView.SetTimeRange("09:00", "10:00", atRow: 1);
            editingView.ClickSave();
            
            page.GetCell(0, 0).WaitFilled();
            page.GetCell(0, 1).WaitFilled();

            page.OpenEditor(0, 0).GetInfoView().CheckStartEndTime("09:00", "24:00");
            page.OpenEditor(0, 1).GetInfoView().CheckStartEndTime("00:00", "10:00");
        }

        [Test]
        [Description("Ночная и обычная смена - проверка счетчиков")]
        public async Task CheckCounters()
        {
            await CreateWorker();
            
            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();

            var editingView = page.OpenEditor(0, 0).GetEditingView();

            editingView.SetTimeRange("22:00", "10:00");
            editingView.SetTimeRange("09:00", "10:00", atRow: 1);
            editingView.ClickSave();
            
            page.GetCell(0, 0).WaitFilled();
            page.GetCell(0, 1).WaitNotFilled();

            var infoView = page.OpenEditor(0, 0).GetInfoView();
            infoView.CheckStartEndTime("09:00", "10:00");
            infoView.CheckStartEndTime("22:00", "10:00", atRow: 1);

            page.CheckSingleWorkerCounters("13 ч", "1 дн");
        }

        [Test]
        [Description("Расширяем ночную смену с начала (меньше 24 ч) с проверкой счётчиков")]
        public async Task ExtendNightTimePeriodFromStart()
        {
            var worker = await CreateWorker();
            await BuildOneWorkingDay(worker.Id, 
                                     DateHelper.GetFirstDayOfMonth(Now), 
                                     TimePeriod.CreateByHours(20, 24));
            await BuildOneWorkingDay(worker.Id, 
                                     DateHelper.GetFirstDayOfMonth(Now).AddDays(1), 
                                     TimePeriod.CreateByHours(0, 10));
            
            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();

            var editingView = page.OpenEditor(0, 0).GetEditingView();
            editingView.SetTimeRange("19:00", "20:00", 1);
            editingView.ClickSave();
            
            page.GetCell(0, 0).WaitFilled();
            page.GetCell(0, 1).WaitNotFilled();
            
            page.CheckSingleWorkerCounters("15 ч", "1 дн");
        }
        
        [Test]
        [Description("Расширяем ночную смену с конца, в новом дне (меньше 24 ч) с проверкой счётчиков")]
        public async Task ExtendNightTimePeriodFromEnd()
        {
            var worker = await CreateWorker();
            await BuildOneWorkingDay(worker.Id, 
                                     DateHelper.GetFirstDayOfMonth(Now), 
                                     TimePeriod.CreateByHours(20, 24));
            await BuildOneWorkingDay(worker.Id, 
                                     DateHelper.GetFirstDayOfMonth(Now).AddDays(1), 
                                     TimePeriod.CreateByHours(0, 10));
            
            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();

            var editingView = page.OpenEditor(0, 1).GetEditingView();
            editingView.SetTimeRange("10:00", "11:00");
            editingView.ClickSave();
            
            page.GetCell(0, 0).WaitFilled();
            page.GetCell(0, 1).WaitNotFilled();
            
            page.CheckSingleWorkerCounters("15 ч", "1 дн");
        }
        
        [Test]
        [Description("Расширяем ночную смену с хвоста (больше 24 ч) с проверкой счётчиков")]
        public async Task ExtendNightTimePeriodFromEndLonger24H()
        {
            var worker = await CreateWorker();
            await BuildOneWorkingDay(worker.Id, 
                                     DateHelper.GetFirstDayOfMonth(Now), 
                                     TimePeriod.CreateByHours(20, 24));
            await BuildOneWorkingDay(worker.Id, 
                                     DateHelper.GetFirstDayOfMonth(Now).AddDays(1), 
                                     TimePeriod.CreateByHours(0, 10));
            
            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();

            var editingView = page.OpenEditor(0, 1).GetEditingView();
            editingView.SetTimeRange("10:00", "22:00");
            editingView.ClickSave();
            
            page.GetCell(0, 0).WaitFilled();
            page.GetCell(0, 1).WaitNotFilled();
            
            page.CheckSingleWorkerCounters("26 ч", "2 дн");
        }
        
        [Test]
        [Description("Объединяем соседние ночные смены - получаем разбиение на 3 дня + поверка счётчиков")]
        public async Task UnionNightWorkingDays()
        {
            var worker = await CreateWorker();
            await BuildOneWorkingDay(worker.Id, 
                                     DateHelper.GetFirstDayOfMonth(Now), 
                                     TimePeriod.CreateByHours(20, 24));
            await BuildOneWorkingDay(worker.Id, 
                                     DateHelper.GetFirstDayOfMonth(Now).AddDays(1), 
                                     TimePeriod.CreateByHours(0, 10), 
                                     TimePeriod.CreateByHours(20, 24));
            await BuildOneWorkingDay(worker.Id, 
                                     DateHelper.GetFirstDayOfMonth(Now).AddDays(2), 
                                     TimePeriod.CreateByHours(0, 10));
            
            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();

            var editingView = page.OpenEditor(0, 1).GetEditingView();
            editingView.SetTimeRange("10:00", "20:00", 1);
            editingView.ClickSave();
            
            page.GetCell(0, 0).WaitFilled();
            page.GetCell(0, 1).WaitFilled();
            page.GetCell(0, 2).WaitFilled();
            
            page.CheckSingleWorkerCounters("38 ч", "3 дн");

            page.OpenEditor(0, 0).GetInfoView().CheckStartEndTime("20:00", "24:00");
            page.OpenEditor(0, 1).GetInfoView().CheckStartEndTime("00:00", "24:00");
            page.OpenEditor(0, 2).GetInfoView().CheckStartEndTime("00:00", "10:00");
        }
        
        [Test]
        [Description("Проверяем подпись следующего дня под временем ночной смены")]
        public async Task OverflowTitles()
        {
            await CreateWorker();
            
            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();

            // В рамках одного месяца
            var editingView = page.OpenEditor(0, 0).GetEditingView();
            editingView.SetTimeRange("12:00", "10:00");
            
            var overflowTitle = DateHelper.GetFirstDayOfMonth(Now).AddDays(1).GetDayAndFullMonth();
            editingView.CheckTimeRange("12:00", "10:00", overflowTitle.Substring(0, 5));
            editingView.ClickSave();
            
            page.GetCell(0, 0).WaitFilled();
            page.OpenEditor(0, 0).GetInfoView().CheckStartEndTime("12:00", "10:00", overflowTitle);
            
            // Следующий месяц
            var colIdx = DateHelper.GetLastDayOfMonth(Now).Day - 1;
            var editingView2 = page.OpenEditor(0, colIdx).GetEditingView();
            editingView2.SetTimeRange("12:00", "10:00");

            var overflowTitleNextMonth = DateHelper.GetFirstDayOfNextMonth(Now).GetDayAndFullMonth();
            editingView.CheckTimeRange("12:00", "10:00", overflowTitleNextMonth.Substring(0, 5));
            editingView.ClickSave();
            
            page.GetCell(0, colIdx).WaitFilled();
            page.OpenEditor(0, colIdx).GetInfoView().CheckStartEndTime("12:00", "10:00", overflowTitleNextMonth);
        }

        [Test]
        [Description("Корректное отображение 6 периодов, при ограничении максимум 5 шт")]
        public async Task TrickySixPeriods()
        {
            await CreateWorker();
            var page = LoadMainPage().GoToWorkerListPage().OpenCalendar();
            
            var editingView = page.OpenEditor(0, 0).GetEditingView();
            editingView.SetTimeRange("23:00", "01:00");
            editingView.ClickSave();

            editingView = page.OpenEditor(0, 1).GetEditingView();
            editingView.SetTimeRange("02:00", "02:30", 0);
            editingView.SetTimeRange("03:00", "03:30", 1);
            editingView.SetTimeRange("04:00", "04:30", 2);
            editingView.SetTimeRange("05:00", "05:30", 3);
            editingView.SetTimeRange("06:00", "06:30", 4);
            editingView.ClickSave();
            
            editingView = page.OpenEditor(0, 0).GetEditingView();
            editingView.SetTimeRange("00:30", "23:30", 1);
            editingView.ClickSave();

            var editor = page.OpenEditor(0, 1);
            var infoView = editor.GetInfoView();

            WaiterHelper.WaitUntil(() => 
                {
                    if(infoView.TimeInfoLine.Count.Get() == 6) 
                        return true;
                    editor.GetEditingView().ClickCancel();
                    editor = page.OpenEditor(0, 1);
                    infoView = editor.GetInfoView();
                    return false;
                });
            
            infoView.CheckStartEndTime("00:00", "01:00", atRow: 0);
            infoView.CheckStartEndTime("02:00", "02:30", atRow: 1);
            infoView.CheckStartEndTime("03:00", "03:30", atRow: 2);
            infoView.CheckStartEndTime("04:00", "04:30", atRow: 3);
            infoView.CheckStartEndTime("05:00", "05:30", atRow: 4);
            infoView.CheckStartEndTime("06:00", "06:30", atRow: 5);

            editingView = editor.GetEditingView();
            
            editingView.CheckTimeRange("00:00", "01:00", atRow: 0);
            editingView.CheckTimeRange("02:00", "02:30", atRow: 1);
            editingView.CheckTimeRange("03:00", "03:30", atRow: 2);
            editingView.CheckTimeRange("04:00", "04:30", atRow: 3);
            editingView.CheckTimeRange("05:00", "05:30", atRow: 4);
            editingView.CheckTimeRange("06:00", "06:30", atRow: 5);
        }
    }
}
