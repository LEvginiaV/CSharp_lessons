using System;

using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.ServiceCalendar;
using Market.CustomersAndStaff.Models.Workers;

namespace Market.CustomersAndStaff.Repositories
{
    public static class RepositoryConstants
    {
        public const string KeyspaceName = "customers_staff";
        public const string WorkerSchedule = "workerschedule";
        public const string ServiceCalendar = "servicecalendar";
        public const string ServiceCalendarRemoved = "servicecalendar_removed";

        public static string GetCalendarTableByType<T>() where T : BaseCalendarRecord
        {
            if(typeof(T) == typeof(WorkerScheduleRecord))
                return WorkerSchedule;

            if(typeof(T) == typeof(ServiceCalendarRecord))
                return ServiceCalendar;

            if (typeof(T) == typeof(ServiceCalendarRecord))
                return ServiceCalendar;

            if (typeof(T) == typeof(ServiceCalendarRemovedRecord))
                return ServiceCalendarRemoved;

            throw new ArgumentOutOfRangeException();
        }
    }
}