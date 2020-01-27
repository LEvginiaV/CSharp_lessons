using System;
using System.Diagnostics;

namespace Market.CustomersAndStaff.Models.Common
{
    public class TimePeriod
    {
        public TimePeriod()
        {
            
        }

        public TimePeriod(TimeSpan startTime, TimeSpan endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public static bool operator !=(TimePeriod a, TimePeriod b)
        {
            return !(a == b);
        }
        
        public static bool operator ==(TimePeriod a, TimePeriod b)
        {
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return ReferenceEquals(a, null) && ReferenceEquals(b, null);
            }
            return a.StartTime == b.StartTime && a.EndTime == b.EndTime;
        }

        public static TimePeriod CreateByHours(int h1, int h2) 
            => new TimePeriod(TimeSpan.FromHours(h1), TimeSpan.FromHours(h2));

        public static TimePeriod CreateByHoursAndHalf(int h1, int h2) 
            => new TimePeriod(TimeSpan.FromMinutes(h1 * 60 + 30), TimeSpan.FromMinutes(h2 * 60 + 30));

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}