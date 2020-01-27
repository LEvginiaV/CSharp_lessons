using System;

namespace Market.CustomersAndStaff.FrontApi.Dto.Calendars
{
    public class ServiceCalendarRecordDto : ServiceCalendarRecordInfoDto
    {
        public Guid Id { get; set; }
        public RecordStatusDto RecordStatus { get; set; }
        public CustomerStatusDto CustomerStatus { get; set; }
    }
}