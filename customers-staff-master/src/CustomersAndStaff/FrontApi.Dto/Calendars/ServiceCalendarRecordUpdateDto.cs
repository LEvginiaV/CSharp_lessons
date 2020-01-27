using System;

namespace Market.CustomersAndStaff.FrontApi.Dto.Calendars
{
    public class ServiceCalendarRecordUpdateDto : ServiceCalendarRecordInfoDto
    {
        public Guid? UpdatedWorkerId { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}