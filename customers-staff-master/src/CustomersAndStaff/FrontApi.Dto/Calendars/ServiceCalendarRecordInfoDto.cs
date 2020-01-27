using System;

using Market.CustomersAndStaff.Models.Common;

namespace Market.CustomersAndStaff.FrontApi.Dto.Calendars
{
    public class ServiceCalendarRecordInfoDto
    {
        public Guid? CustomerId { get; set; }
        public Guid[] ProductIds { get; set; }
        public TimePeriod Period { get; set; }
        public string Comment { get; set; }
        public bool IsOnlineRecord { get; set; }
    }

    public enum RecordStatusDto
    {
        Active,
        Canceled,
        Removed,
    }

    public enum CustomerStatusDto
    {
        Active,
        ActiveAccepted,
        Completed,
        CanceledBeforeEvent,
        NotCome,
        NoService,
        Mistake,
    }
}