using System;

namespace Market.CustomersAndStaff.Models.Calendar
{
    public class ShopCalendarRange<T> where T : BaseCalendarRecord
    {
        public Guid ShopId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ShopCalendarDay<T>[] ShopCalendarDays { get; set; }
    }
}