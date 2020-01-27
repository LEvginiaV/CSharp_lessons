using System;
using System.Linq;

using Market.CustomersAndStaff.Models.Calendar;

namespace Market.CustomersAndStaff.Utils.CalendarHelpers
{
    public static class CalendarExtensions
    {
        public static ShopCalendarRange<T> FillAllDays<T>(this ShopCalendarRange<T> source) where T : BaseCalendarRecord
        {
            var totalDays = (int)(source.EndDate - source.StartDate).TotalDays + 1;

            var result = new ShopCalendarRange<T>
                {
                    ShopId = source.ShopId,
                    StartDate = source.StartDate,
                    EndDate = source.EndDate,
                    ShopCalendarDays = new ShopCalendarDay<T>[totalDays],
                };

            foreach(var shopCalendarDay in source.ShopCalendarDays.OrderBy(x => x.Date))
            {
                var ind = (int)(shopCalendarDay.Date - source.StartDate).TotalDays;
                result.ShopCalendarDays[ind] = shopCalendarDay;
            }

            for(var i = 0; i < result.ShopCalendarDays.Length; i++)
            {
                if(result.ShopCalendarDays[i] == null)
                {
                    result.ShopCalendarDays[i] = new ShopCalendarDay<T>
                        {
                            ShopId = source.ShopId,
                            Date = source.StartDate + TimeSpan.FromDays(i),
                            WorkerCalendarDays = new WorkerCalendarDay<T>[0],
                        };
                }
            }

            return result;
        }
    }
}