using System;

namespace Market.CustomersAndStaff.Models.Calendar
{
    public class ShopCalendarMonth<T> where T : BaseCalendarRecord
    {
        public Guid ShopId { get; set; }
        public DateTime Month { get; set; }
        public ShopCalendarDay<T>[] ShopCalendarDays { get; set; }
    }
}