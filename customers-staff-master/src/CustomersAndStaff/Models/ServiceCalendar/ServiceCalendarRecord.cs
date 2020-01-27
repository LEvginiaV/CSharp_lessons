using System;

using Market.CustomersAndStaff.Models.Calendar;

namespace Market.CustomersAndStaff.Models.ServiceCalendar
{
    public class ServiceCalendarRemovedRecord : ServiceCalendarRecord { }

    public class ServiceCalendarRecord : BaseCalendarRecord
    {
        public Guid Id { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid[] ProductIds { get; set; }
        public string Comment { get; set; }
        public RecordStatus RecordStatus { get; set; }
        public CustomerStatus CustomerStatus { get; set; }
        public bool IsOnlineRecord { get; set; }
    }

    public enum RecordStatus
    {
        Active,
        Canceled,
        Removed,
    }

    public enum CustomerStatus
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