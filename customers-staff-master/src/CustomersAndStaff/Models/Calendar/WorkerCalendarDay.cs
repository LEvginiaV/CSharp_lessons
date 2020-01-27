using System;

namespace Market.CustomersAndStaff.Models.Calendar
{
    public class WorkerCalendarDay<T> where T : BaseCalendarRecord
    {
        public Guid WorkerId { get; set; }
        public DateTime Date { get; set; }
        public T[] Records { get; set; }
    }
}