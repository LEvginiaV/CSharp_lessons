using System;
using System.Collections.Generic;

namespace Market.CustomersAndStaff.FrontApi.Dto.WorkingCalendar
{
    public class ShopWorkingDayDto
    {
        public int Version { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<Guid, WorkingCalendarRecordDto[]> WorkingDayMap { get; set; }
    }
}