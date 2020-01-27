using System;

using Market.CustomersAndStaff.Models.Common;

namespace Market.CustomersAndStaff.Services.ServiceCalendar
{
    public static class TimePeriodExtensions
    {
        public static bool IntersectsWith(this TimePeriod period1, TimePeriod period2)
        {

            return Max(period1.StartTime, period2.StartTime) < Min(period1.EndTime, period2.EndTime);
        }

        private static TimeSpan Max(TimeSpan timeSpan1, TimeSpan timeSpan2)
        {
            return new TimeSpan(Math.Max(timeSpan1.Ticks, timeSpan2.Ticks));
        }

        private static TimeSpan Min(TimeSpan timeSpan1, TimeSpan timeSpan2)
        {
            return new TimeSpan(Math.Min(timeSpan1.Ticks, timeSpan2.Ticks));
        }
    }
}