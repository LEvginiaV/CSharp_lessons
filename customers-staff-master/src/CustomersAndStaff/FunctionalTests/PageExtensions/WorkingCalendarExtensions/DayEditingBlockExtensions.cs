using System;
using System.Linq;

using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers.WorkingCalendarDayEditor;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.Models.Common;

namespace Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkingCalendarExtensions
{
    public static class DayEditingBlockExtensions
    {
        public static void SetTimeRange(this DayEditingBlock block, string start = "10:00", string end = "21:00", int atRow = 0)
        {
            while(block.TimeRangeLine.Count.Get() <= atRow && block.TimeRangeLine.First().AddLink.IsPresent.Get())
            {
                block.TimeRangeLine.First().AddLink.Click();
            }
            var timeRange = block.GetTimeRangeBlockAtIndex(atRow);
            timeRange.StartTime.ResetRawValue(start);
            timeRange.EndTime.ResetRawValue(end);
        }

        public static void CheckTimeRange(this DayEditingBlock block, int start, int end, string overflow = null, int atRow = 0)
        {
            block.CheckTimeRange($"{(start > 9 ? "" : "0")}{start}:00", $"{(end > 9 ? "" : "0")}{end}:00", overflow, atRow);
        }

        public static void CheckTimeRange(this DayEditingBlock block, string start, string end, string overflow = null, int atRow = 0)
        {
            var timeRange = block.GetTimeRangeBlockAtIndex(atRow);
            timeRange.StartTime.WaitText(start);
            timeRange.EndTime.WaitText(end);
            
            if(!string.IsNullOrEmpty(overflow))
                timeRange.OverflowText.WaitText(overflow);
                
            if(overflow == "") 
                timeRange.OverflowText.WaitAbsence();
        }

        public static void SetCalendarMode(this DayEditingBlock block, CalendarFillingMode mode)
        {
            block.CalendarMode.SelectByIndex((int)mode);
        }

        public static void ClickSave(this DayEditingBlock block)
        {
            block.Save.Click();
            block.WaitAbsence();
        }

        public static void ClickCancel(this DayEditingBlock block)
        {
            block.Cancel.Click();
            block.WaitAbsence();
        }

        public static void CheckErrorMessage(this TimeRangeLine line, string text, bool rightSide = true)
        {
            if(rightSide)
            {
                line.ErrorMessageTooltipStart.WaitAbsence(componentDescription: $"{nameof(line.ErrorMessageTooltipStart)}");
                line.ErrorMessageTooltipEnd.WaitPresence(componentDescription: $"{nameof(line.ErrorMessageTooltipEnd)}");
                line.ErrorMessageTooltipEnd.ErrorMessage.WaitText(text, $"ожидаем сообщение валидации в {nameof(line.ErrorMessageTooltipEnd)}");
            }
            else
            {
                line.ErrorMessageTooltipStart.WaitPresence(componentDescription: $"{nameof(line.ErrorMessageTooltipStart)}");
                line.ErrorMessageTooltipEnd.WaitAbsence(componentDescription: $"{nameof(line.ErrorMessageTooltipEnd)}");
                line.ErrorMessageTooltipStart.ErrorMessage.WaitText(text, $"ожидаем сообщение валидации в {nameof(line.ErrorMessageTooltipStart)}");
            }
        }
        
        public static void MouseOverTimeInput(this TimeRangeLine line, bool rightSide = true)
        {
            if(rightSide)
            {
                line.EndTime.MouseOver();
            }
            else
            {
                line.StartTime.MouseOver();
            }
        }

        public static void ClickOnFakeBlock(this DayEditingBlock block)
        {
            block.FakeBlock.Container.Click();
        }

        public static TimeRangeLine GetTimeRangeBlockAtIndex(this DayEditingBlock block, int index)
        {
            block.TimeRangeLine.Count.Wait().MoreOrEqual(index + 1);
            return block.TimeRangeLine.ElementAt(index);
        }

        public static TimePeriod ConvertToTimePeriod(this TimeRangeLine line)
        {
            var start = line.StartTime.Text.Get();
            var startH = int.Parse(start.Substring(0, 2));
            var startM = int.Parse(start.Substring(3));
            
            var end = line.EndTime.Text.Get();
            var endH = int.Parse(end.Substring(0, 2));
            var endM = int.Parse(end.Substring(3));
            
            return new TimePeriod
                {
                    StartTime = TimeSpan.FromMinutes(startH * 60 + startM),
                    EndTime = TimeSpan.FromMinutes(endH * 60 + endM),
                };
        }
    }
}