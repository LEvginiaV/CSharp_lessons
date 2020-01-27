using System.Collections.Generic;
using System.Linq;

using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Utils.TimeSpanHelpers;

namespace Market.CustomersAndStaff.Utils.TimePeriodHelpers
{
    public static class TimePeriodExtensions
    {
        public static IEnumerable<TimePeriod> SubtractPeriods(this IEnumerable<TimePeriod> first, IEnumerable<TimePeriod> second)
        {
            return second.Aggregate(first, SubtractInternal);
        }

        private static IEnumerable<TimePeriod> SubtractInternal(IEnumerable<TimePeriod> source, TimePeriod period)
        {
            foreach (var sourcePeriod in source)
            {
                if (sourcePeriod.EndTime <= period.StartTime || period.EndTime <= sourcePeriod.StartTime)
                {
                    yield return sourcePeriod;
                }
                else
                {
                    if (sourcePeriod.StartTime < period.StartTime)
                    {
                        yield return new TimePeriod(sourcePeriod.StartTime, period.StartTime);
                    }

                    if (period.EndTime < sourcePeriod.EndTime)
                    {
                        yield return new TimePeriod(period.EndTime, sourcePeriod.EndTime);
                    }
                }
            }
        }

        public static IEnumerable<TimePeriod> RoundPeriodsByHours(this IEnumerable<TimePeriod> periods)
        {
            foreach (var period in periods)
            {
                var startTime = TimeSpanHelper.RoundUpperByHour(period.StartTime);
                var endTime = TimeSpanHelper.RoundLowerByHour(period.EndTime);

                if (startTime < endTime)
                {
                    yield return new TimePeriod(startTime, endTime);
                }
            }
        }
    }
}