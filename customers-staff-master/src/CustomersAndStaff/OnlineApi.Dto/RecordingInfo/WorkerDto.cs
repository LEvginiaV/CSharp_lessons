using System;

namespace Market.CustomersAndStaff.OnlineApi.Dto.RecordingInfo
{
    public class WorkerDto
    {
        public Guid WorkerId { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public WorkerCalendarRecordDto[] Schedule { get; set; }
    }
}