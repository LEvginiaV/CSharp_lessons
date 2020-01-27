using System;

namespace Market.CustomersAndStaff.OnlineApi.Dto.RecordingInfo
{
    public class ServiceDto
    {
        public Guid ServiceId { get; set; }
        public Guid GroupId { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public Guid[] Workers { get; set; }
    }
}