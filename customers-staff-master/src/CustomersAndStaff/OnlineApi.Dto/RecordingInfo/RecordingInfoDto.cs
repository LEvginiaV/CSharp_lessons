using System;

namespace Market.CustomersAndStaff.OnlineApi.Dto.RecordingInfo
{
    public class RecordingInfoDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public ServiceDto[] Services { get; set; }
        public WorkerDto[] Workers { get; set; }
        public DateTime Today { get; set; }
    }
}