using System;
using System.Collections.Generic;
using System.Globalization;

namespace Market.CustomersAndStaff.Utils.Extensions
{
    public static class DateHelper
    {
        public static DateTime Tomorrow() => DateTime.Today.AddDays(1);

        public static DateTime GetFirstDayOfMonth(DateTime month)
        {
            return month.Date.AddDays(-month.Day + 1);
        }

        public static DateTime GetMiddleOfMonth(DateTime dateTime)
        {
            return GetFirstDayOfMonth(dateTime).AddDays(15);
        }
        
        public static DateTime GetFirstDayOfPreviousMonth(DateTime date)
        {
            return GetFirstDayOfMonth(GetFirstDayOfMonth(date).AddDays(-1));
        }
        
        public static DateTime GetFirstDayOfNextMonth(DateTime date)
        {
            var nextMonth = date.Month + 1;
            return new DateTime(date.Year + (nextMonth > 12 ? 1 : 0), nextMonth > 12 ? 1 : nextMonth, 1);
        }

        public static DateTime GetLastDayOfMonth(DateTime date)
        {
            return GetFirstDayOfNextMonth(date).AddDays(-1);
        }

        public static int GetMonthDifference(DateTime from, DateTime to)
        {
            var fromMonth = GetFirstDayOfMonth(from);
            var toMonth = GetFirstDayOfMonth(to);
            var monthsCounter = 0;

            for (var date = fromMonth; date <= toMonth; date = date.AddMonths(1))
            {
                ++monthsCounter;
            }

            return monthsCounter;
        }

        public static IEnumerable<DateTime> GetMonthRange(DateTime from, DateTime to)
        {
            var fromMonth = GetFirstDayOfMonth(from);
            var toMonth = GetFirstDayOfMonth(to);

            for (var date = fromMonth; date <= toMonth; date = date.AddMonths(1))
            {
                yield return date;
            }
        }
        
        private static readonly string[] dayOfWeekNames = {"Воскресенье", "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота"};

        public static string GetDayOfWeekName(this DateTime dateTime) => dayOfWeekNames[(int)dateTime.DayOfWeek];

        public static string GetDayAndFullMonth(this DateTime dateTime) => dateTime.ToString("m", new CultureInfo("ru-RU"));

        public static string GetFullMonthName(this DateTime dateTime) => dateTime.ToString("MMMM");

        public static IEnumerable<DateTime> CreateDatesRange(DateTime from, DateTime to)
        {
            for(var i = from.Date; i <= to; i = i.AddDays(1))
            {
                yield return i;
            }
        }
    }
}