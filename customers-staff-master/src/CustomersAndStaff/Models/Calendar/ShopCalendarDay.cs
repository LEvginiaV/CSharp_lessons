using System;

namespace Market.CustomersAndStaff.Models.Calendar
{
    public class ShopCalendarDay<T> where T : BaseCalendarRecord
    {
        public Guid ShopId { get; set; }
        public DateTime Date { get; set; }
        public WorkerCalendarDay<T>[] WorkerCalendarDays { get; set; }
    }
}