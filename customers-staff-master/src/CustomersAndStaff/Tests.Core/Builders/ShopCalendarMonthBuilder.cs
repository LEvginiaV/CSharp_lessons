using System;
using System.Linq;

using Market.CustomersAndStaff.Models.Calendar;

namespace Market.CustomersAndStaff.Tests.Core.Builders
{
    public class ShopCalendarMonthBuilder<T> where T : BaseCalendarRecord
    {
        public static ShopCalendarMonthBuilder<T> Create(Guid shopId, DateTime month)
        {
            return new ShopCalendarMonthBuilder<T>(shopId, month);
        }

        private ShopCalendarMonthBuilder(Guid shopId, DateTime month)
        {
            shopCalendarMonth = new ShopCalendarMonth<T>
                {
                    ShopId = shopId,
                    Month = month,
                    ShopCalendarDays = new ShopCalendarDay<T>[0],
                };
        }

        public ShopCalendarMonthBuilder<T> AddShopDay(DateTime date, Action<ShopCalendarDayBuilder<T>> action)
        {
            var builder = ShopCalendarDayBuilder<T>.Create(shopCalendarMonth.ShopId, date);
            action?.Invoke(builder);
            shopCalendarMonth.ShopCalendarDays = shopCalendarMonth.ShopCalendarDays.Append(builder.Build()).ToArray();
            return this;
        }

        public ShopCalendarMonth<T> Build()
        {
            return shopCalendarMonth;
        }

        private readonly ShopCalendarMonth<T> shopCalendarMonth;
    }
}