using System;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.ServiceCalendar;

namespace Market.CustomersAndStaff.ServiceApi.Client.Extensions
{
    public interface IServiceCalendarApiClient
    {
        Task<Guid> CreateRecord(Guid shopId, DateTime date, Guid workerId, ServiceCalendarRecord record);
        Task<ShopCalendarDay<ServiceCalendarRecord>> GetShopDay(Guid shopId, DateTime date);
    }
}