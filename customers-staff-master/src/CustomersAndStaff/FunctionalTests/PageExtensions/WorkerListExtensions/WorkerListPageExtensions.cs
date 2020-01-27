using System.Linq;

using FluentAssertions;

using Kontur.RetryableAssertions.Extensions;

using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers;
using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers.WorkingCalendarDayEditor;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkingCalendarExtensions;
using Market.CustomersAndStaff.Models.Workers;

using MoreLinq;

namespace Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkerListExtensions
{
    
    public static class WorkerListPageExtensions
    {
        public static WorkerListPage OpenCalendar(this WorkerListPage page)
        {
            page.ChartTab.Click();
            page.LoadingHover.WaitAbsence();
            return page;
        }

        public static WorkerListPage GoToPrevMonth(this WorkerListPage page)
        {
            page.PrevMonth.Click();
            page.LoadingHover.WaitAbsence();
            return page;
        }

        public static WorkerListPage GoToNextMonth(this WorkerListPage page)
        {
            page.NextMonth.Click();
            page.LoadingHover.WaitAbsence();
            return page;
        }

        public static DayEditor OpenEditor(this WorkerListPage page, int rowIdx, int colIdx)
        {
            page.CalendarRow.ElementAt(rowIdx).ClickOnCell(colIdx);
            page.WorkersDayEditor.WaitPresence(componentDescription : "редактор времени");
            return page.WorkersDayEditor;
        }

        public static bool CheckStartAndEndTime(this WorkerListPage page, int rowIdx, int colIdx, string start, string end, string overflow = null)
        {
            return page.CheckStartAndEndTime(rowIdx, colIdx, (start : start, end : end, overflow : overflow));
        }

        public static bool CheckStartAndEndTime(this WorkerListPage page, int rowIdx, int colIdx, params (string start, string end, string overflow)[] times)
        {
            var editor = page.OpenEditor(rowIdx, colIdx);
            var infoView = editor.GetInfoView();
            infoView.TimeInfoLine.WaitCount(times.Length);
            var ok = true;
            infoView.TimeInfoLine.ForEach((line, idx) =>
                {
                    ok = ok && line.StartTimeText.Text.Get().Equals(times[idx].start);
                    ok = ok && line.EndTimeText.Text.Get().Equals(times[idx].end);
                    if(line.OverflowText.IsPresent.Get())
                        ok = ok && line.OverflowText.Text.Get().Equals(times[idx].overflow);
                    else
                        ok = ok && string.IsNullOrEmpty(times[idx].overflow);
                });
            editor.Close();
            return ok;
        }

        public static CalendarCell GetCell(this WorkerListPage page, int rowIdx, int colIdx)
        {
            page.CalendarRow.Count.Wait().MoreOrEqual(rowIdx + 1);
            return page.CalendarRow.ElementAt(rowIdx).CalendarCell.ElementAt(colIdx);
        }

        public static void CheckWorkerItems(this WorkerListPage page, Worker[] workers)
        {
            page.WorkerItem
                .Select(x => x.ToActualData())
                .Wait()
                .ShouldBeEquivalentTo(workers, o => o.Including(x => x.FullName)
                                                     .Including(x => x.Position)
                                                     .WithStrictOrdering());
        }

        public static WorkerListPage CheckSingleWorkerCounters(this WorkerListPage page, string hours, string days)
        {
            page.WorkerItem
                .Select(x => (x.DaysCounter.Text.Get(), x.HoursCounter.Text.Get()))
                .Wait()
                .ShouldBeEquivalentTo(new [] {(days, hours)});

            return page;
        }
    }
}
