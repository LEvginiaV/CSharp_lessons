using System;
using System.Linq;

using Market.CustomersAndStaff.Models.Calendar;

namespace Market.CustomersAndStaff.Tests.Core.Builders
{
    public class ShopCalendarRangeBuilder<T> where T : BaseCalendarRecord
    {
        public static ShopCalendarRangeBuilder<T> Create(Guid shopId, DateTime from, DateTime to)
        {
            return new ShopCalendarRangeBuilder<T>(shopId, from, to);
        }

        private ShopCalendarRangeBuilder(Guid shopId, DateTime from, DateTime to)
        {
            shopCalendarRange = new ShopCalendarRange<T>
                {
                    ShopId = shopId,
                    StartDate = from,
                    EndDate = to,
                    ShopCalendarDays = new ShopCalendarDay<T>[0],
                };
        }

        public ShopCalendarRangeBuilder<T> AddShopCalendarDay(DateTime date, Action<ShopCalendarDayBuilder<T>> action)
        {
            var builder = ShopCalendarDayBuilder<T>.Create(shopCalendarRange.ShopId, date);
            action?.Invoke(builder);
            shopCalendarRange.ShopCalendarDays = shopCalendarRange.ShopCalendarDays.Append(builder.Build()).ToArray();
            return this;
        }

        public ShopCalendarRange<T> Build()
        {
            return shopCalendarRange;
        }

        private readonly ShopCalendarRange<T> shopCalendarRange;
    }
}