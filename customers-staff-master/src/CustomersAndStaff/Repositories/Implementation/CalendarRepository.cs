using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Cassandra;

using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Repositories.Serializer;
using Market.CustomersAndStaff.Repositories.StoredModels;
using Market.CustomersAndStaff.Utils.Extensions;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.SessionTableQueryExtending.PrimitiveStoring;
using SKBKontur.Catalogue.CassandraUtils.DistributedLock.Locker;

namespace Market.CustomersAndStaff.Repositories.Implementation
{
    public class CalendarRepository<T> : ICalendarRepository<T> where T : BaseCalendarRecord
    {
        public CalendarRepository(
            ICassandraStorageFactory factory,
            ISerializer serializer,
            IMapper mapper,
            ILocker locker,
            ICounterRepository counterRepository)
        {
            tableName = RepositoryConstants.GetCalendarTableByType<T>();
            storage = factory.Get<CalendarDayStorageElement>(RepositoryConstants.KeyspaceName, tableName);
            this.serializer = serializer;
            this.mapper = mapper;
            this.locker = locker;
            this.counterRepository = counterRepository;
        }

        public async Task WriteAsync(Guid shopId, WorkerCalendarDay<T> workerCalendarDay)
        {
            var month = DateHelper.GetFirstDayOfMonth(workerCalendarDay.Date);
            using(await locker.LockAsync(GetLockId(shopId, month)))
            {
                await storage.WriteAsync(ToStorageElement(shopId, workerCalendarDay));
                await UpdateVersionAsync(shopId, month);
            }
        }

        public async Task WriteAsync(Guid shopId, IEnumerable<WorkerCalendarDay<T>> workerCalendarDays)
        {
            var tasks = workerCalendarDays.GroupBy(x => DateHelper.GetFirstDayOfMonth(x.Date))
                                          .Select(x => WriteByMonthAsync(shopId, x.Key, x.ToArray()))
                                          .ToArray();
            await Task.WhenAll(tasks);
        }

        private async Task WriteByMonthAsync(Guid shopId, DateTime month, IEnumerable<WorkerCalendarDay<T>> workerCalendarDays)
        {
            using(await locker.LockAsync(GetLockId(shopId, month)))
            {
                await storage.WriteAsync(workerCalendarDays.Select(x => ToStorageElement(shopId, x)));
                await UpdateVersionAsync(shopId, month);
            }
        }

        public async Task<WorkerCalendarDay<T>> ReadWorkerCalendarDayAsync(Guid shopId, DateTime date, Guid workerId)
        {
            date = date.Date;
            var month = DateHelper.GetFirstDayOfMonth(date);
            var localMonth = mapper.Map<LocalDate>(month);
            var localDate = mapper.Map<LocalDate>(date);
            var workerCalendarDay = await storage.FirstOrDefaultAsync(x => x.ShopId == shopId &&
                                                                           x.Month == localMonth &&
                                                                           x.Date == localDate &&
                                                                           x.WorkerId == workerId);
            return workerCalendarDay == null
                       ? new WorkerCalendarDay<T> {Date = date, WorkerId = workerId, Records = new T[0]}
                       : ToWorkerCalendarDay(workerCalendarDay);
        }

        public async Task<ShopCalendarDay<T>> ReadShopCalendarDayAsync(Guid shopId, DateTime date)
        {
            date = date.Date;
            var month = DateHelper.GetFirstDayOfMonth(date);
            var localMonth = mapper.Map<LocalDate>(month);
            var localDate = mapper.Map<LocalDate>(date);
            var workerSchedules = await storage.WhereAsync(x => x.ShopId == shopId && x.Month == localMonth && x.Date == localDate);
            return ToShopCalendarDay(shopId, date, workerSchedules);
        }

        public async Task<ShopCalendarMonth<T>> ReadShopCalendarMonthAsync(Guid shopId, DateTime month)
        {
            var localMonth = mapper.Map<LocalDate>(DateHelper.GetFirstDayOfMonth(month));
            var shopSchedules = await storage.WhereAsync(x => x.ShopId == shopId && x.Month == localMonth);
            return new ShopCalendarMonth<T>
                {
                    ShopId = shopId,
                    Month = month,
                    ShopCalendarDays = shopSchedules.GroupBy(x => x.Date)
                                                    .Select(x => ToShopCalendarDay(shopId, mapper.Map<DateTime>(x.Key), x.ToArray()))
                                                    .ToArray(),
                };
        }

        public async Task<ShopCalendarRange<T>> ReadShopCalendarRangeAsync(Guid shopId, DateTime @from, DateTime to)
        {
            from = from.Date;
            to = to.Date;

            if(from > to)
            {
                throw new ArgumentException("To date must be greater than from date");
            }

            var calendarTasks = DateHelper.GetMonthRange(from, to)
                                          .Select(date => ReadShopCalendarRangeInternal(shopId, date, from, to))
                                          .ToArray();

            await Task.WhenAll(calendarTasks);

            var shopCalendarDays = calendarTasks.SelectMany(calendarTask => calendarTask.Result).ToArray();

            return new ShopCalendarRange<T>
                {
                    ShopId = shopId,
                    StartDate = from,
                    EndDate = to,
                    ShopCalendarDays = shopCalendarDays
                };
        }

        private async Task<ShopCalendarDay<T>[]> ReadShopCalendarRangeInternal(Guid shopId, DateTime month, DateTime @from, DateTime to)
        {
            var localMonth = mapper.Map<LocalDate>(month);
            var localFrom = mapper.Map<LocalDate>(from);
            var localTo = mapper.Map<LocalDate>(to);
            var shopSchedules = await storage.WhereAsync(x => x.ShopId == shopId && x.Month == localMonth && x.Date >= localFrom && x.Date <= localTo);
            return shopSchedules.GroupBy(x => x.Date)
                                .Select(x => ToShopCalendarDay(shopId, mapper.Map<DateTime>(x.Key), x.ToArray()))
                                .ToArray();
        }

        public async Task<int> GetVersionAsync(Guid shopId, DateTime month)
        {
            return await counterRepository.GetCurrentAsync(GetVersionKey(shopId, month));
        }

        private async Task UpdateVersionAsync(Guid shopId, DateTime month)
        {
            await counterRepository.IncrementAsync(GetVersionKey(shopId, month));
        }

        private string GetVersionKey(Guid shopId, DateTime month)
        {
            return $"version/calendar/{tableName}/{shopId}/{month:yyyy-MM}";
        }

        private string GetLockId(Guid shopId, DateTime month)
        {
            return $"calendar/{tableName}/{shopId}/{month:yyyy-MM}";
        }

        private WorkerCalendarDay<T> ToWorkerCalendarDay(CalendarDayStorageElement storageElement)
        {
            return new WorkerCalendarDay<T>
                {
                    WorkerId = storageElement.WorkerId,
                    Date = mapper.Map<DateTime>(storageElement.Date),
                    Records = serializer.Deserialize<T[]>(storageElement.Records),
                };
        }

        private ShopCalendarDay<T> ToShopCalendarDay(Guid shopId, DateTime date, IEnumerable<CalendarDayStorageElement> storageElements)
        {
            var workerSchedule = new ShopCalendarDay<T>
                {
                    ShopId = shopId,
                    Date = date,
                    WorkerCalendarDays = storageElements.Select(ToWorkerCalendarDay).ToArray(),
                };

            return workerSchedule;
        }

        private CalendarDayStorageElement ToStorageElement(Guid shopId, WorkerCalendarDay<T> workerScheduleDay)
        {
            return new CalendarDayStorageElement
                {
                    ShopId = shopId,
                    Month = mapper.Map<LocalDate>(DateHelper.GetFirstDayOfMonth(workerScheduleDay.Date)),
                    WorkerId = workerScheduleDay.WorkerId,
                    Date = mapper.Map<LocalDate>(workerScheduleDay.Date),
                    Records = serializer.Serialize(workerScheduleDay.Records),
                };
        }

        private static ShopCalendarMonth<T> GetCalendarByRange(ShopCalendarMonth<T> shopCalendarMonth, DateTime from, DateTime to)
        {
            return new ShopCalendarMonth<T>
                {
                    ShopId = shopCalendarMonth.ShopId,
                    Month = shopCalendarMonth.Month,
                    ShopCalendarDays = shopCalendarMonth.ShopCalendarDays
                                                        .Where(t => t.Date >= from && t.Date <= to)
                                                        .ToArray()
                };
        }

        private readonly CassandraStorage<CalendarDayStorageElement> storage;
        private readonly ICounterRepository counterRepository;
        private readonly ISerializer serializer;
        private readonly IMapper mapper;
        private readonly ILocker locker;
        private readonly string tableName;
    }
}