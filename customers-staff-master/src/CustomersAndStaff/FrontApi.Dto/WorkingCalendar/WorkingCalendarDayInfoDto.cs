using System;

namespace Market.CustomersAndStaff.FrontApi.Dto.WorkingCalendar
{
    public class WorkingCalendarDayInfoDto
    {
        public DateTime Date { get; set; }
        public WorkingCalendarRecordDto[] Records { get; set; }
    }
}