using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Models.Calendar;

namespace Market.CustomersAndStaff.Repositories.Interface
{
    public interface ICalendarRepository<T> where T : BaseCalendarRecord
    {
        Task WriteAsync(Guid shopId, WorkerCalendarDay<T> workerCalendarDay);
        Task WriteAsync(Guid shopId, IEnumerable<WorkerCalendarDay<T>> workerCalendarDays);
        Task<WorkerCalendarDay<T>> ReadWorkerCalendarDayAsync(Guid shopId, DateTime date, Guid workerId);
        Task<ShopCalendarDay<T>> ReadShopCalendarDayAsync(Guid shopId, DateTime date);
        Task<ShopCalendarMonth<T>> ReadShopCalendarMonthAsync(Guid shopId, DateTime month);
        Task<ShopCalendarRange<T>> ReadShopCalendarRangeAsync(Guid shopId, DateTime from, DateTime to);
        Task<int> GetVersionAsync(Guid shopId, DateTime month);
    }
}