using System;

namespace Market.CustomersAndStaff.FrontApi.Dto.Common
{
    public class TimePeriodDto
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}