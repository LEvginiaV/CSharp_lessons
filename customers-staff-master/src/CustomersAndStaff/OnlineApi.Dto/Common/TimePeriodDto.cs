using System;

namespace Market.CustomersAndStaff.OnlineApi.Dto.Common
{
    public class TimePeriodDto
    {
        public TimePeriodDto()
        {
            
        }

        public TimePeriodDto(TimeSpan startTime, TimeSpan endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}