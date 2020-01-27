using System;

using Market.CustomersAndStaff.OnlineApi.Dto.Common;

namespace Market.CustomersAndStaff.OnlineApi.Dto.RecordingInfo
{
    public class WorkerCalendarRecordDto
    {
        public DateTime Date { get; set; }
        public TimePeriodDto[] WorkingTime { get; set; }
        public TimePeriodDto[] AvailableTime { get; set; }
    }
}