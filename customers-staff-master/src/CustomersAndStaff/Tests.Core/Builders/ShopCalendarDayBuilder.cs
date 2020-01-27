using System;
using System.Linq;

using Market.CustomersAndStaff.Models.Calendar;

namespace Market.CustomersAndStaff.Tests.Core.Builders
{
    public class ShopCalendarDayBuilder<T> where T : BaseCalendarRecord
    {
        public static ShopCalendarDayBuilder<T> Create(Guid shopId, DateTime date)
        {
            return new ShopCalendarDayBuilder<T>(shopId, date);
        }

        private ShopCalendarDayBuilder(Guid shopId, DateTime date)
        {
            shopCalendarDay = new ShopCalendarDay<T>
                {
                    ShopId = shopId,
                    Date = date,
                    WorkerCalendarDays = new WorkerCalendarDay<T>[0],
                };
        }

        public ShopCalendarDayBuilder<T> AddWorkerDay(Guid workerId, Action<WorkerCalendarDayBuilder<T>> action)
        {
            var builder = WorkerCalendarDayBuilder<T>.Create(workerId, shopCalendarDay.Date);
            action?.Invoke(builder);
            shopCalendarDay.WorkerCalendarDays = shopCalendarDay.WorkerCalendarDays.Append(builder.Build()).ToArray();
            return this;
        }

        public ShopCalendarDay<T> Build()
        {
            return shopCalendarDay;
        }

        private readonly ShopCalendarDay<T> shopCalendarDay;
    }
}