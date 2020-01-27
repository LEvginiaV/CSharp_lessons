using System;
using System.Collections.Generic;
using System.Linq;

using Kontur.Utilities.Convertions.Time;

using Market.Api.Models.Products;
using Market.Api.Models.Shops;
using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.OnlineRecording;
using Market.CustomersAndStaff.Models.ServiceCalendar;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.OnlineApi.Converters.Mappers;
using Market.CustomersAndStaff.OnlineApi.Dto.Common;
using Market.CustomersAndStaff.OnlineApi.Dto.RecordingInfo;
using Market.CustomersAndStaff.Utils.CalendarHelpers;
using Market.CustomersAndStaff.Utils.TimePeriodHelpers;

using MoreLinq;

namespace Market.CustomersAndStaff.OnlineApi.Converters.RecordingInfo
{
    public class RecordingInfoConverter : IRecordingInfoConverter
    {
        public RecordingInfoConverter(IMapperWrapper mapperWrapper)
        {
            this.mapperWrapper = mapperWrapper;
        }

        public RecordingInfoDto CreateInfo(
            Shop shop,
            OnlineService[] onlineServices,
            Product[] products,
            Worker[] workers,
            ShopCalendarRange<WorkerScheduleRecord> workersSchedule,
            ShopCalendarRange<ServiceCalendarRecord> serviceCalendar,
            DateTime today)
        {
            var onlineServiceIds = onlineServices.Select(x => x.ProductId).ToHashSet();
            var workerDtos = CreateWorkers(workers.Where(x => x.IsAvailableOnline && !x.IsDeleted).ToArray(), workersSchedule.FillAllDays(), serviceCalendar.FillAllDays());
            var workerIds = workerDtos.Select(x => x.WorkerId).ToArray();

            return new RecordingInfoDto
                {
                    Workers = workerDtos,
                    Services = products.Where(x => onlineServiceIds.Contains(x.Id.Value)).Select(x => new ServiceDto
                        {
                            ServiceId = x.Id.Value,
                            GroupId = Guid.TryParse(x.GroupId, out var groupId) ? groupId : Guid.Empty,
                            Name = x.Name,
                            Price = x.PricesInfo?.SellPrice,
                            Workers = workerIds,
                        }).ToArray(),
                    Name = shop.Name,
                    Address = shop.Address,
                    Today = today,
                };
        }

        private WorkerDto[] CreateWorkers(
            Worker[] workers,
            ShopCalendarRange<WorkerScheduleRecord> workersSchedule,
            ShopCalendarRange<ServiceCalendarRecord> serviceCalendar)
        {
            var workerDictionary = workers.ToDictionary(x => x.Id, x => new List<WorkerCalendarRecordDto>());

            for(var i = 0; i < 30; i++)
            {
                var date = workersSchedule.StartDate + i.Days();

                var workersScheduleDay = workersSchedule.ShopCalendarDays[i];
                var serviceCalendarDay = serviceCalendar.ShopCalendarDays[i];
                var serviceCalendarDict = serviceCalendarDay.WorkerCalendarDays.ToDictionary(x => x.WorkerId);

                foreach(var workerDay in workersScheduleDay.WorkerCalendarDays.Where(x => workerDictionary.ContainsKey(x.WorkerId)))
                {
                    var recordDto = CreateWorkerCalendarRecordDto(date, workerDay, serviceCalendarDict);

                    if(recordDto.WorkingTime.Length > 0)
                    {
                        workerDictionary[workerDay.WorkerId].Add(recordDto);
                    }
                }
            }

            return workers.Where(x => workerDictionary[x.Id].Count > 0)
                          .Select(x => new WorkerDto
                              {
                                  WorkerId = x.Id,
                                  Name = x.FullName,
                                  Position = x.Position,
                                  Schedule = workerDictionary[x.Id].ToArray(),
                              })
                          .ToArray();
        }

        private WorkerCalendarRecordDto CreateWorkerCalendarRecordDto(
            DateTime date,
            WorkerCalendarDay<WorkerScheduleRecord> workerDay,
            Dictionary<Guid, WorkerCalendarDay<ServiceCalendarRecord>> serviceCalendarDict)
        {
            var workingTime = workerDay.Records
                                       .Select(x => x.Period)
                                       .RoundPeriodsByHours()
                                       .OrderBy(x => x.StartTime)
                                       .ToArray();

            var recordDto = new WorkerCalendarRecordDto
                {
                    Date = date,
                    WorkingTime = mapperWrapper.Map<TimePeriodDto[]>(workingTime),
                    AvailableTime = Array.Empty<TimePeriodDto>(),
                };

            if(recordDto.WorkingTime.Length > 0)
            {
                if(!serviceCalendarDict.TryGetValue(workerDay.WorkerId, out var serviceRecordings) || serviceRecordings.Records.Length == 0)
                {
                    recordDto.AvailableTime = recordDto.WorkingTime;
                }
                else
                {
                    var servicesTime = serviceRecordings.Records
                                                        .Where(x => x.RecordStatus == RecordStatus.Active)
                                                        .Select(x => x.Period);

                    var availableTime = workingTime.SubtractPeriods(servicesTime);
                    recordDto.AvailableTime = availableTime.RoundPeriodsByHours()
                                                           .Select(x => mapperWrapper.Map<TimePeriodDto>(x))
                                                           .ToArray();
                }
            }

            return recordDto;
        }

        private readonly IMapperWrapper mapperWrapper;
    }
}