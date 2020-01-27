using System;
using System.Threading.Tasks;
using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.Workers;

namespace Market.CustomersAndStaff.ServiceApi.Client
{
    public interface IWorkersSchedulesApiClient
    {
        Task<ShopCalendarRange<WorkerScheduleRecord>> Get(Guid shopId, DateTime from, DateTime to);
    }
}