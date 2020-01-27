using System;
using System.Collections.Generic;

namespace Market.CustomersAndStaff.FrontApi.Dto.WorkingCalendar
{
    public class ShopWorkingCalendarDto
    {
        public int Version { get; set; }
        public DateTime Month { get; set; }
        public Dictionary<Guid, WorkingCalendarDayInfoDto[]> WorkingCalendarMap { get; set; }
    }
}