using System;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Models.ServiceCalendar;
using Market.CustomersAndStaff.Models.Validations;
using Market.CustomersAndStaff.ModelValidators;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Utils.Extensions;

using SKBKontur.Catalogue.CassandraUtils.DistributedLock.Locker;

namespace Market.CustomersAndStaff.Services.ServiceCalendar
{
    public class CalendarService : ICalendarService
    {
        public CalendarService(
            ILocker locker,
            ICalendarRepository<ServiceCalendarRecord> calendarRepository,
            ICalendarRepository<ServiceCalendarRemovedRecord> calendarRemovedRepository, IMapper mapper, 
            IValidator<ServiceCalendarRecord> serviceCalendarRecordValidator)
        {
            this.locker = locker;
            this.calendarRepository = calendarRepository;
            this.calendarRemovedRepository = calendarRemovedRepository;
            this.mapper = mapper;
            this.serviceCalendarRecordValidator = serviceCalendarRecordValidator;
        }

        public async Task<(Guid recordId, ValidationResult validationResult)> CreateAsync(Guid shopId, DateTime date, Guid workerId, ServiceCalendarRecord record)
        {
            var validationResult = serviceCalendarRecordValidator.Validate(record);
            if(!validationResult.IsSuccess)
            {
                return (Guid.Empty, validationResult);
            }

            record.Id = Guid.NewGuid();
            record.RecordStatus = RecordStatus.Active;
            record.CustomerStatus = CustomerStatus.Active;

            using(await locker.LockAsync(GetLockId(shopId, workerId, date)))
            {
                var workerCalendarDay = await calendarRepository.ReadWorkerCalendarDayAsync(shopId, date, workerId);
                if(!ValidateSlot(workerCalendarDay, record.Id, record.Period))
                {
                    return (record.Id, ValidationResult.Fail("periodSlot", "period is not empty"));
                }

                workerCalendarDay.Records = workerCalendarDay.Records.Append(record).ToArray();
                await calendarRepository.WriteAsync(shopId, workerCalendarDay);

                return (record.Id, ValidationResult.Success());
            }
        }

        public async Task<ValidationResult> UpdateAsync(Guid shopId, DateTime date, Guid workerId, ServiceCalendarRecord record, DateTime? updateDate, Guid? updateWorkerId)
        {
            var validationResult = serviceCalendarRecordValidator.Validate(record);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            if ((updateDate == null || updateDate.Value == date) && (updateWorkerId == null || updateWorkerId.Value == workerId))
            {
                return await UpdateSingleRecordAsync(shopId, date, workerId, record);
            }

            return await UpdateTwoRecordsAsync(shopId, date, workerId, record, updateDate ?? date, updateWorkerId ?? workerId);
        }

        private async Task<ValidationResult> UpdateSingleRecordAsync(Guid shopId, DateTime date, Guid workerId, ServiceCalendarRecord record)
        {
            using(await locker.LockAsync(GetLockId(shopId, workerId, date)))
            {
                var workerCalendarDay = await calendarRepository.ReadWorkerCalendarDayAsync(shopId, date, workerId);

                var currentRecord = GetRecord(workerCalendarDay, record.Id);
                if(currentRecord == null)
                {
                    return ValidationResult.Fail("recordNotFound", $"no record with id {record.Id}");
                }

                if(!ValidateSlot(workerCalendarDay, record.Id, record.Period))
                {
                    return ValidationResult.Fail("periodSlot", "period is not empty");
                }

                record.CustomerStatus = currentRecord.CustomerStatus;
                record.RecordStatus = currentRecord.RecordStatus;

                workerCalendarDay.Records = workerCalendarDay.Records.Where(x => x.Id != record.Id).Append(record).ToArray();
                await calendarRepository.WriteAsync(shopId, workerCalendarDay);

                return ValidationResult.Success();
            }
        }

        private async Task<ValidationResult> UpdateTwoRecordsAsync(Guid shopId, DateTime date, Guid workerId, ServiceCalendarRecord record, DateTime updateDate, Guid updateWorkerId)
        {
            var locks = new[] {GetLockId(shopId, workerId, date), GetLockId(shopId, updateWorkerId, updateDate)}.OrderBy(x => x).ToArray();

            using(await locker.LockAsync(locks[0]))
            using(await locker.LockAsync(locks[1]))
            {
                var workerCalendarDay = await calendarRepository.ReadWorkerCalendarDayAsync(shopId, date, workerId);
                var updateWorkerCalendarDay = await calendarRepository.ReadWorkerCalendarDayAsync(shopId, updateDate, updateWorkerId);

                var currentRecord = GetRecord(workerCalendarDay, record.Id);
                if(currentRecord == null)
                {
                    return ValidationResult.Fail("recordNotFound", $"no record with id {record.Id}");
                }

                if(!ValidateSlot(updateWorkerCalendarDay, record.Id, record.Period))
                {
                    return ValidationResult.Fail("periodSlot", "period is not empty");
                }

                record.CustomerStatus = currentRecord.CustomerStatus;
                record.RecordStatus = currentRecord.RecordStatus;

                workerCalendarDay.Records = workerCalendarDay.Records.Where(x => x.Id != record.Id).ToArray();
                updateWorkerCalendarDay.Records = updateWorkerCalendarDay.Records.Append(record).ToArray();

                if(DateHelper.GetFirstDayOfMonth(date) == DateHelper.GetFirstDayOfMonth(updateDate))
                {
                    await calendarRepository.WriteAsync(shopId, new[] {updateWorkerCalendarDay, workerCalendarDay});
                }
                else
                {
                    await calendarRepository.WriteAsync(shopId, updateWorkerCalendarDay);
                    await calendarRepository.WriteAsync(shopId, workerCalendarDay);
                }

                return ValidationResult.Success();
            }
        }

        public async Task<ValidationResult> RemoveAsync(Guid shopId, DateTime date, Guid workerId, Guid recordId)
        {
            if(date < DateTime.UtcNow.Date)
            {
                return ValidationResult.Fail("date", "expected date > now");
            }

            using(await locker.LockAsync(GetLockId(shopId, workerId, date)))
            {
                var workerCalendarDay = await calendarRepository.ReadWorkerCalendarDayAsync(shopId, date, workerId);

                var currentRecord = GetRecord(workerCalendarDay, recordId);
                if(currentRecord == null)
                {
                    return ValidationResult.Fail("recordNotFound", $"no record with id {recordId}");
                }

                var workerCalendarRemovedDay = await calendarRemovedRepository.ReadWorkerCalendarDayAsync(shopId, date, workerId);
                currentRecord.RecordStatus = RecordStatus.Removed;

                workerCalendarDay.Records = workerCalendarDay.Records.Where(x => x.Id != recordId).ToArray();
                workerCalendarRemovedDay.Records = workerCalendarRemovedDay.Records.Append(mapper.Map<ServiceCalendarRemovedRecord>(currentRecord)).ToArray();

                await calendarRemovedRepository.WriteAsync(shopId, workerCalendarRemovedDay);
                await calendarRepository.WriteAsync(shopId, workerCalendarDay);

                return ValidationResult.Success();
            }
        }

        public async Task<ValidationResult> UpdateCustomerStatusAsync(Guid shopId, DateTime date, Guid workerId, Guid recordId, CustomerStatus newStatus)
        {
            using(await locker.LockAsync(GetLockId(shopId, workerId, date)))
            {
                var workerCalendarDay = await calendarRepository.ReadWorkerCalendarDayAsync(shopId, date, workerId);

                var currentRecord = GetRecord(workerCalendarDay, recordId);
                if(currentRecord == null)
                {
                    return ValidationResult.Fail("recordNotFound", $"no record with id {recordId}");
                }

                currentRecord.CustomerStatus = newStatus;

                if (newStatus == CustomerStatus.CanceledBeforeEvent || newStatus == CustomerStatus.NotCome || newStatus == CustomerStatus.NoService)
                {
                    currentRecord.RecordStatus = RecordStatus.Canceled;
                }
                else if(newStatus == CustomerStatus.Mistake)
                {
                    var workerCalendarRemovedDay = await calendarRemovedRepository.ReadWorkerCalendarDayAsync(shopId, date, workerId);
                    currentRecord.RecordStatus = RecordStatus.Removed;

                    workerCalendarDay.Records = workerCalendarDay.Records.Where(x => x.Id != recordId).ToArray();
                    workerCalendarRemovedDay.Records = workerCalendarRemovedDay.Records.Append(mapper.Map<ServiceCalendarRemovedRecord>(currentRecord)).ToArray();

                    await calendarRemovedRepository.WriteAsync(shopId, workerCalendarRemovedDay);
                }
                else
                {
                    if(currentRecord.RecordStatus != RecordStatus.Active)
                    {
                        if(!ValidateSlot(workerCalendarDay, recordId, currentRecord.Period))
                        {
                            return ValidationResult.Fail("periodSlot", "period is not empty");
                        }
                    }

                    currentRecord.RecordStatus = RecordStatus.Active;
                }
                
                await calendarRepository.WriteAsync(shopId, workerCalendarDay);

                return ValidationResult.Success();
            }
        }

        public async Task<ShopCalendarDay<ServiceCalendarRecord>> ReadShopCalendarDayAsync(Guid shopId, DateTime date, RecordStatus? status = null)
        {
            var shopDay = await calendarRepository.ReadShopCalendarDayAsync(shopId, date);

            if(status != null)
            {
                foreach(var workerCalendarDay in shopDay.WorkerCalendarDays)
                {
                    workerCalendarDay.Records = workerCalendarDay.Records.Where(x => x.RecordStatus == status).ToArray();
                }
            }

            return shopDay;
        }

        private static ServiceCalendarRecord GetRecord(WorkerCalendarDay<ServiceCalendarRecord> calendarDay, Guid recordId)
        {
            return calendarDay.Records.FirstOrDefault(x => x.Id == recordId);
        }

        private static bool ValidateSlot(WorkerCalendarDay<ServiceCalendarRecord> calendarDay, Guid recordId, TimePeriod period)
        {
            return calendarDay.Records.All(x => x.Id == recordId || x.RecordStatus != RecordStatus.Active || !x.Period.IntersectsWith(period));
        }

        private string GetLockId(Guid shopId, Guid workerId, DateTime date)
        {
            return $"calendar/{shopId}/{workerId}/{date:yyyy-MM-dd}";
        }

        private readonly ILocker locker;
        private readonly ICalendarRepository<ServiceCalendarRecord> calendarRepository;
        private readonly ICalendarRepository<ServiceCalendarRemovedRecord> calendarRemovedRepository;
        private readonly IMapper mapper;
        private readonly IValidator<ServiceCalendarRecord> serviceCalendarRecordValidator;
    }
}