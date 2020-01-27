using System;
using System.Collections.Generic;
using System.Linq;

using Market.CustomersAndStaff.FrontApi.Converters.Mappers;
using Market.CustomersAndStaff.FrontApi.Dto.WorkingCalendar;
using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.Utils.Extensions;

namespace Market.CustomersAndStaff.FrontApi.Converters.WorkerSchedule
{
    public class WorkerScheduleConverter : IWorkerScheduleConverter
    {
        public WorkerScheduleConverter(IMapperWrapper mapperWrapper)
        {
            this.mapperWrapper = mapperWrapper;
        }
        
        public ShopWorkingCalendarDto ConvertObsolete(ShopCalendarMonth<WorkerScheduleRecord> model, HashSet<Guid> workerIdSet, int version)
        {
            var workingCalendarMap = new Dictionary<Guid, WorkingCalendarDayInfoDto[]>();
            var month = model.Month;
            foreach(var grouping in model.ShopCalendarDays
                                         .SelectMany(x => x.WorkerCalendarDays)
                                         .GroupBy(x => x.WorkerId)
                                         .Where(x => workerIdSet.Contains(x.Key)))
            {
                var dict = grouping.ToDictionary(x => x.Date.Day, x => mapperWrapper.Map<WorkingCalendarDayInfoDto>(x));

                var daysForWorker = Enumerable.Range(1, DateTime.DaysInMonth(month.Year, month.Month))
                                              .Select(x => dict.TryGetValue(x, out var dto) ? dto : GetDefaultDay(new DateTime(month.Year, month.Month, x)))
                                              .ToArray();
                
                workingCalendarMap.Add(grouping.Key, daysForWorker);
            }

            return new ShopWorkingCalendarDto
                {
                    Version = version,
                    Month = month,
                    WorkingCalendarMap = workingCalendarMap,
                };
        }

        public Dictionary<Guid, WorkingCalendarDayInfoDto[]> Convert(ShopCalendarRange<WorkerScheduleRecord> range, HashSet<Guid> workerIdSet)
        {
            return range.ShopCalendarDays
                        .SelectMany(x => x.WorkerCalendarDays)
                        .GroupBy(x => x.WorkerId)
                        .Where(x => workerIdSet.Contains(x.Key))
                        .ToDictionary(x => x.Key, x =>
                            {
                                var workerDaysMap = x.ToDictionary(day => day.Date);

                                return DateHelper.CreateDatesRange(range.StartDate, range.EndDate)
                                                 .Select(date => workerDaysMap.GetOrDefault(date) ?? GetDefaultDayNew(date))
                                                 .Select(day => mapperWrapper.Map<WorkingCalendarDayInfoDto>(day))
                                                 .ToArray();
                            });
        }

        private WorkerCalendarDay<WorkerScheduleRecord> GetDefaultDayNew(DateTime date)
        {
            return new WorkerCalendarDay<WorkerScheduleRecord>
                {
                    Date = date,
                    Records = new WorkerScheduleRecord[0],
                };
        }

        public ShopWorkingDayDto Convert(ShopCalendarDay<WorkerScheduleRecord> model, HashSet<Guid> workerIdSet, int version)
        {
            var workingDayMap = model.WorkerCalendarDays
                 .Where(x => workerIdSet.Contains(x.WorkerId))
                 .ToDictionary(x => x.WorkerId, x => x.Records.Select(r => mapperWrapper.Map<WorkingCalendarRecordDto>(r)).ToArray());
            
            return new ShopWorkingDayDto
                {
                    Date = model.Date,
                    Version = version,
                    WorkingDayMap = workingDayMap,
                };
        }

        private WorkingCalendarDayInfoDto GetDefaultDay(DateTime date)
        {
            return new WorkingCalendarDayInfoDto
                {
                    Date = date,
                    Records = new WorkingCalendarRecordDto[0],
                };
        }

        private readonly IMapperWrapper mapperWrapper;
    }
}