using System;

namespace Market.CustomersAndStaff.Utils.TimeSpanHelpers
{
    public static class TimeSpanHelper
    {
        public static TimeSpan RoundUpperByHour(TimeSpan ts)
        {
            var rem = ts.Ticks % TimeSpan.TicksPerHour;
            if(rem < 0)
            {
                rem += TimeSpan.TicksPerHour;
            }
            return rem > 0 ? new TimeSpan(ts.Ticks - rem + TimeSpan.TicksPerHour) : ts;
        }

        public static TimeSpan RoundLowerByHour(TimeSpan ts)
        {
            var rem = ts.Ticks % TimeSpan.TicksPerHour;
            if (rem < 0)
            {
                rem += TimeSpan.TicksPerHour;
            }
            return rem > 0 ? new TimeSpan(ts.Ticks - rem) : ts;
        }
    }
}