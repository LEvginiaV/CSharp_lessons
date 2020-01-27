using System;

namespace Market.CustomersAndStaff.FrontApi.Dto.Calendars
{
    public class WorkerServiceCalendarDayDto
    {
        public Guid WorkerId { get; set; }
        public DateTime Date { get; set; }
        public ServiceCalendarRecordDto[] Records { get; set; }
    }
}