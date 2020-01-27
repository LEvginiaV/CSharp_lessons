using System;

namespace Market.CustomersAndStaff.Models.TimeZones
{
    public class TimeZone
    {
        public Guid Id { get; set; }
        public TimeSpan Offset { get; set; }
        public string Name { get; set; }
    }
}