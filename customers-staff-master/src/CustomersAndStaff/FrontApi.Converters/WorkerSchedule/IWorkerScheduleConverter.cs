using System;
using System.Collections.Generic;

using Market.CustomersAndStaff.FrontApi.Dto.WorkingCalendar;
using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.Workers;

namespace Market.CustomersAndStaff.FrontApi.Converters.WorkerSchedule
{
    public interface IWorkerScheduleConverter
    {
        ShopWorkingCalendarDto ConvertObsolete(ShopCalendarMonth<WorkerScheduleRecord> model, HashSet<Guid> workerIdSet, int version);
        Dictionary<Guid, WorkingCalendarDayInfoDto[]> Convert(ShopCalendarRange<WorkerScheduleRecord> range, HashSet<Guid> workerIdSet);
        ShopWorkingDayDto Convert(ShopCalendarDay<WorkerScheduleRecord> model, HashSet<Guid> workerIdSet, int version);
    }
}