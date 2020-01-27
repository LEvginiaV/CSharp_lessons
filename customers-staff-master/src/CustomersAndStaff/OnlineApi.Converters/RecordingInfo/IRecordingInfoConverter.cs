using System;

using Market.Api.Models.Products;
using Market.Api.Models.Shops;
using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.OnlineRecording;
using Market.CustomersAndStaff.Models.ServiceCalendar;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.OnlineApi.Dto.RecordingInfo;

namespace Market.CustomersAndStaff.OnlineApi.Converters.RecordingInfo
{
    public interface IRecordingInfoConverter
    {
        RecordingInfoDto CreateInfo(
            Shop shop,
            OnlineService[] onlineServices,
            Product[] products,
            Worker[] workers,
            ShopCalendarRange<WorkerScheduleRecord> workersSchedule,
            ShopCalendarRange<ServiceCalendarRecord> serviceCalendar,
            DateTime today);
    }
}