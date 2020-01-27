using System;

namespace Market.CustomersAndStaff.FrontApi.Dto.TimeZones
{
    public class TimeZoneDto
    {
        public Guid Id { get; set; }
        public TimeSpan Offset { get; set; }
        public string Name { get; set; }
    }
}