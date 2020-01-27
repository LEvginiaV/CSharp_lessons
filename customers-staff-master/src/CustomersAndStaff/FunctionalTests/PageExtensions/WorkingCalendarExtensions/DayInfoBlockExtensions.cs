using System.Linq;

using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers.WorkingCalendarDayEditor;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;

namespace Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkingCalendarExtensions
{
    public static class DayInfoBlockExtensions
    {
        public static DayInfoBlock CheckStartEndTime(this DayInfoBlock block, string start, string end, string overflow = null, int atRow = 0)
        {
            block.TimeInfoLine.Count.Wait().MoreOrEqual(atRow + 1);
            var line = block.TimeInfoLine.ElementAt(atRow);
            line.StartTimeText.WaitText(start);
            line.EndTimeText.WaitText(end);

            if(!string.IsNullOrEmpty(overflow))
                line.OverflowText.WaitText(overflow);
                
            if(overflow == "") 
                line.OverflowText.WaitAbsence();

            return block;
        }
    }
}
