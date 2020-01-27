using System;
using System.Linq;
using System.Threading.Tasks;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers;
using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers.WorkingCalendarDayEditor;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkerListExtensions;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkingCalendarExtensions;
using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Utils.Extensions;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.WorkingCalendarTests
{
    public class WorkingCalendarTestBase : TestBase
    {
        protected async Task BuildOneWorkingDay(Guid workerId, DateTime date, params TimePeriod[] periods)
        {
            var workingDay = new WorkerCalendarDay<WorkerScheduleRecord>
                {
                    WorkerId = workerId,
                    Date = date,
                    Records = periods.Select(p => new WorkerScheduleRecord {Period = p}).ToArray()
                };
            await workerScheduleRepository.WriteAsync(shop.Id, new [] {workingDay});
        }

        protected async Task BuildCalendar2By2(Guid workerId, DateTime startFrom, TimePeriod timePeriod)
        {
            var firstDayOfMonth = DateHelper.GetFirstDayOfMonth(startFrom);
            var lastDayOfMonth = DateHelper.GetLastDayOfMonth(startFrom);
            var startIdx = startFrom.Day - 1;
            
            var workingRecord = new[] {new WorkerScheduleRecord {Period = timePeriod}};
            
            var workingDays = Enumerable.Range(0, lastDayOfMonth.Day).Select(idx => new WorkerCalendarDay<WorkerScheduleRecord>
                {
                    Date = firstDayOfMonth.AddDays(idx),
                    Records = idx >= startIdx && (idx - startIdx) / 2 % 2 == 0 ? workingRecord : new WorkerScheduleRecord[0],
                    WorkerId = workerId
                }).ToArray();

            await workerScheduleRepository.WriteAsync(shop.Id, workingDays);
        }

        protected void CheckMonthFor5By2From(WorkerListPage page, DateTime startDate)
            => CheckCalendarRange(page, startDate, DateHelper.GetLastDayOfMonth(startDate), CalendarFillingMode.FiveByTwo);

        protected void CheckMonthFor2By2From(WorkerListPage page, DateTime startDate)
            => CheckCalendarRange(page, startDate, DateHelper.GetLastDayOfMonth(startDate), CalendarFillingMode.TwoByTwo);

        protected void CheckWholeMonth(WorkerListPage page, DateTime month, CalendarFillingMode mode) 
            => CheckCalendarRange(page, DateHelper.GetFirstDayOfMonth(month), DateHelper.GetLastDayOfMonth(month), mode);

        protected void CheckCalendarRange(WorkerListPage page, DateTime from, DateTime to, CalendarFillingMode mode)
        {
            from = from.Date;
            to = to.Date;
            if(from.Month != to.Month || from.Year != to.Year)
            {
                throw new Exception("Dates should have same Month and Year");
            }

            if(mode == CalendarFillingMode.OnlySelectedDay && from != to)
            {
                throw new Exception($"Can't check one day in provided range: {from:dd.MM.yyyy} - {to:dd.MM.yyyy}");
            }
            
            Console.Write($"CheckCalendarRange [{from:dd.MM.yyyy} - {to:dd.MM.yyyy}] with mode: {mode}. WorkDaysMask: ");

            var firstDay = DateHelper.GetFirstDayOfMonth(from);
            var startOfRange = from.Day - 1;

            for(var i = from.Day - 1; i < to.Day; i++)
            {
                var relativeDate = firstDay.AddDays(i);
                var isWeekend = IsWeekend(mode, relativeDate, i - startOfRange);
                Console.Write(isWeekend ? "-" : "+");
                page.GetCell(0, i).WaitFilled(!isWeekend, $"в индексе {i}");
            }
        }

        protected static bool IsWeekend(CalendarFillingMode mode, DateTime date, int relativeIdx)
        {
            if(mode == CalendarFillingMode.Weekend) return true;
            if(mode == CalendarFillingMode.OnlySelectedDay) return false;
            if(mode == CalendarFillingMode.FiveByTwo)
            {
                return date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday;
            }
            if(mode == CalendarFillingMode.TwoByTwo)
            {
                return relativeIdx / 2 % 2 != 0;
            }
            return true;
        }

        protected TimePeriod GetDefaultTimePeriod() => GetTimePeriod(9, 18);
        
        protected TimePeriod GetTimePeriod(double begin, double end) => new TimePeriod(TimeSpan.FromHours(begin), TimeSpan.FromHours(end));

        protected void OpenValidation(DayEditingBlock editingView, PresenceBy presenceBy)
        {
            if (presenceBy == PresenceBy.Submit)
            {
                editingView.Save.Click();
            }
            else
            {
                editingView.ClickOnFakeBlock();
            }
        }

        protected void CheckValidationMessage(DayEditingBlock editingView, string message, ErrorAt errorAt, int atRow = 0)
        {
            var timeRange = editingView.GetTimeRangeBlockAtIndex(atRow);
            if (errorAt == ErrorAt.Both)
            {
                timeRange.StartTime.MouseOver();
                timeRange.CheckErrorMessage(message, false);
                editingView.ClickOnFakeBlock();
                timeRange.EndTime.MouseOver();
                timeRange.CheckErrorMessage(message, true);
            }
            else
            {
                timeRange.MouseOverTimeInput(errorAt == ErrorAt.Right);
                timeRange.CheckErrorMessage(message, errorAt == ErrorAt.Right);
            }

            editingView.ClickOnFakeBlock();
        }
        
        protected async Task<Worker> CreateWorker(string name = "Василий")
        {
            return await workerRepository.CreateAsync(shop.Id, new Worker {FullName = name});
        }
        
        protected DateTime Now = DateTime.Now;

        protected enum ErrorAt
        {
            Left,
            Right,
            Both,
        }

        protected enum PresenceBy
        {
            Unfocus,
            Submit,
        }

        [Injected]
        protected readonly IWorkerRepository workerRepository;

        [Injected]
        protected readonly ICalendarRepository<WorkerScheduleRecord> workerScheduleRepository;
    }
}