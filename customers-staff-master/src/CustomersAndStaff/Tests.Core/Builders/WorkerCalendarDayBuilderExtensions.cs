using System;

using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Models.ServiceCalendar;
using Market.CustomersAndStaff.Models.Workers;

namespace Market.CustomersAndStaff.Tests.Core.Builders
{
    public static class WorkerCalendarDayBuilderExtensions
    {
        public static WorkerCalendarDayBuilder<ServiceCalendarRecord> AddRecord(
            this WorkerCalendarDayBuilder<ServiceCalendarRecord> builder,
            Guid recordId,
            TimePeriod period,
            Action<ServiceCalendarRecordBuilder> action = null)
        {
            var recordBuilder = ServiceCalendarRecordBuilder.Create(recordId, period);
            action?.Invoke(recordBuilder);
            return builder.AddRecord(recordBuilder.Build());
        }

        public static WorkerCalendarDayBuilder<WorkerScheduleRecord> AddRecord(
            this WorkerCalendarDayBuilder<WorkerScheduleRecord> builder,
            TimePeriod period)
        {
            return builder.AddRecord(new WorkerScheduleRecord{Period = period});
        }
    }
}