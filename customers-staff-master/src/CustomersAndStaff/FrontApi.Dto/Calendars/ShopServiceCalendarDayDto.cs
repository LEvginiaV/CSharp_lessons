using System;

namespace Market.CustomersAndStaff.FrontApi.Dto.Calendars
{
    public class ShopServiceCalendarDayDto
    {
        public DateTime Date { get; set; }
        public WorkerServiceCalendarDayDto[] WorkerCalendarDays { get; set; }
    }
}