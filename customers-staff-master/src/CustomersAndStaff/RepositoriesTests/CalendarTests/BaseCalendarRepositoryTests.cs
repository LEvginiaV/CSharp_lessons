using System;
using System.Linq;
using System.Threading.Tasks;

using AutoFixture;

using FluentAssertions;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Tests.Core.Builders;
using Market.CustomersAndStaff.Tests.Core.Configuration;
using Market.CustomersAndStaff.Utils.Extensions;

using NUnit.Framework;

namespace Market.CustomersAndStaff.RepositoriesTests.CalendarTests
{
    public class BaseCalendarRepositoryTests<T> : IMainSuite where T : BaseCalendarRecord, new()
    {
        [GroboSetUp]
        public void SetUp()
        {
            shopId = Guid.NewGuid();
            workerId = Guid.NewGuid();
            currentDate = DateTime.UtcNow.Date;
            currentDate = currentDate.AddDays(-currentDate.Day + 1);
            defaultRecord = CreateRecord(new TimePeriod(TimeSpan.FromHours(3), TimeSpan.FromHours(5)));

            workerScheduleRepository.WriteAsync(shopId, WorkerCalendarDayBuilder<T>.Create(workerId, currentDate)
                                                                                   .AddRecord(defaultRecord)
                                                                                   .Build()).Wait();
        }

        [Test]
        public async Task UpdateEmptyScheduleTest()
        {
            var shopCalendarMonth = await workerScheduleRepository.ReadShopCalendarMonthAsync(shopId, currentDate);
            shopCalendarMonth.Should().BeEquivalentTo(ShopCalendarMonthBuilder<T>
                                                      .Create(shopId, currentDate)
                                                      .AddShopDay(currentDate,
                                                                  shopBuilder => shopBuilder.AddWorkerDay(workerId,
                                                                                                          workerBuilder => workerBuilder.AddRecord(defaultRecord)))
                                                      .Build());

            var shopCalendarDay = await workerScheduleRepository.ReadShopCalendarDayAsync(shopId, currentDate);
            shopCalendarDay.Should().BeEquivalentTo(ShopCalendarDayBuilder<T>.Create(shopId, currentDate)
                                                                             .AddWorkerDay(workerId, builder => builder.AddRecord(defaultRecord))
                                                                             .Build());

            var workerSchedule = await workerScheduleRepository.ReadWorkerCalendarDayAsync(shopId, currentDate, workerId);
            workerSchedule.Should().BeEquivalentTo(WorkerCalendarDayBuilder<T>.Create(workerId, currentDate)
                                                                              .AddRecord(defaultRecord)
                                                                              .Build());
        }

        [Test]
        public async Task AddNewWorkerToShopSchedule()
        {
            var newWorkerId = Guid.NewGuid();
            var newRecord = CreateRecord(new TimePeriod(TimeSpan.FromHours(4), TimeSpan.FromHours(6)));

            await workerScheduleRepository.WriteAsync(shopId, WorkerCalendarDayBuilder<T>.Create(newWorkerId, currentDate)
                                                                                         .AddRecord(newRecord)
                                                                                         .Build());

            var shopSchedule = await workerScheduleRepository.ReadShopCalendarDayAsync(shopId, currentDate);
            shopSchedule.Should().BeEquivalentTo(ShopCalendarDayBuilder<T>.Create(shopId, currentDate)
                                                                          .AddWorkerDay(workerId, builder => builder.AddRecord(defaultRecord))
                                                                          .AddWorkerDay(newWorkerId, builder => builder.AddRecord(newRecord))
                                                                          .Build());

            var workerSchedule = await workerScheduleRepository.ReadWorkerCalendarDayAsync(shopId, currentDate, newWorkerId);
            workerSchedule.Should().BeEquivalentTo(WorkerCalendarDayBuilder<T>.Create(newWorkerId, currentDate)
                                                                              .AddRecord(newRecord)
                                                                              .Build());
        }

        [Test]
        public async Task AddNewDateToWorkerSchedule()
        {
            var anotherDate = DateHelper.GetFirstDayOfMonth(currentDate).AddDays(5);

            if(currentDate == anotherDate)
            {
                anotherDate = currentDate.AddDays(5);
            }

            var newRecord = CreateRecord(new TimePeriod(TimeSpan.FromHours(4), TimeSpan.FromHours(6)));
            await workerScheduleRepository.WriteAsync(shopId, WorkerCalendarDayBuilder<T>.Create(workerId, anotherDate)
                                                                                         .AddRecord(newRecord)
                                                                                         .Build());

            var shopCalendarMonth = await workerScheduleRepository.ReadShopCalendarMonthAsync(shopId, currentDate);
            shopCalendarMonth.Should().BeEquivalentTo(
                ShopCalendarMonthBuilder<T>.Create(shopId, currentDate)
                                           .AddShopDay(currentDate,
                                                       shopBuilder => shopBuilder.AddWorkerDay(workerId, workerBuilder => workerBuilder.AddRecord(defaultRecord)))
                                           .AddShopDay(currentDate.AddDays(5),
                                                       shopBuilder => shopBuilder.AddWorkerDay(workerId, workerBuilder => workerBuilder.AddRecord(newRecord)))
                                           .Build());

            var workerSchedule = await workerScheduleRepository.ReadWorkerCalendarDayAsync(shopId, anotherDate, workerId);
            workerSchedule.Should().BeEquivalentTo(WorkerCalendarDayBuilder<T>.Create(workerId, anotherDate)
                                                                              .AddRecord(newRecord)
                                                                              .Build());
        }

        [Test]
        public async Task UpdateWorkerSchedule()
        {
            var newRecord = CreateRecord(new TimePeriod(TimeSpan.FromHours(4), TimeSpan.FromHours(6)));
            await workerScheduleRepository.WriteAsync(shopId, WorkerCalendarDayBuilder<T>.Create(workerId, currentDate)
                                                                                         .AddRecord(newRecord)
                                                                                         .Build());

            var workerSchedule = await workerScheduleRepository.ReadWorkerCalendarDayAsync(shopId, currentDate, workerId);
            workerSchedule.Should().BeEquivalentTo(WorkerCalendarDayBuilder<T>.Create(workerId, currentDate)
                                                                              .AddRecord(newRecord)
                                                                              .Build());
        }

        [Test]
        public async Task VersionTest()
        {
            var version = await workerScheduleRepository.GetVersionAsync(shopId, currentDate);
            version.Should().Be(2);

            var month = DateHelper.GetFirstDayOfMonth(currentDate);
            var tasks = Enumerable.Range(0, 20)
                                  .Select(x => month.AddDays(x))
                                  .Select(x => workerScheduleRepository.WriteAsync(shopId, WorkerCalendarDayBuilder<T>.Create(workerId, x)
                                                                                                                      .AddRecord(defaultRecord)
                                                                                                                      .Build()
                                          ));

            await Task.WhenAll(tasks);

            version = await workerScheduleRepository.GetVersionAsync(shopId, currentDate);
            version.Should().Be(22);
        }

        [Test]
        public void BadRangeDatesTest()
        {
            var firstDate = new DateTime(2019, 10, 1);
            var secondDate = new DateTime(2019, 11, 1);

            Assert.Throws<AggregateException>(() => workerScheduleRepository.ReadShopCalendarRangeAsync(shopId, secondDate, firstDate).Wait());
        }

        [Test]
        public async Task EmptyRangeTest()
        {
            var firstDate = new DateTime(2019, 10, 1);
            var secondDate = new DateTime(2019, 12, 1);

            var shopCalendarRange = await workerScheduleRepository.ReadShopCalendarRangeAsync(shopId, firstDate, secondDate);
            shopCalendarRange.ShopCalendarDays.Length.Should().Be(0);
        }

        [Test]
        public async Task OneMonthWithEmptyRangeTest()
        {
            var firstDate = new DateTime(2019, 10, 1);
            var secondDate = new DateTime(2019, 10, 10);
            var workerId = Guid.NewGuid();

            await workerScheduleRepository.WriteAsync(shopId, new[]
                {
                    WorkerCalendarDayBuilder<T>.Create(workerId, secondDate.AddDays(2))
                                               .AddRecord(defaultRecord)
                                               .Build()
                });

            var shopCalendarRange = await workerScheduleRepository.ReadShopCalendarRangeAsync(shopId, firstDate, secondDate);
            shopCalendarRange.ShopCalendarDays.Length.Should().Be(0);
        }

        [Test]
        public async Task OneMonthWithRangeTest()
        {
            var firstDate = new DateTime(2019, 10, 1);
            var secondDate = new DateTime(2019, 10, 10);
            var workerId = Guid.NewGuid();

            await workerScheduleRepository.WriteAsync(shopId, new[]
                {
                    WorkerCalendarDayBuilder<T>.Create(workerId, firstDate)
                                               .AddRecord(defaultRecord)
                                               .Build(),
                    WorkerCalendarDayBuilder<T>.Create(workerId, secondDate)
                                               .AddRecord(defaultRecord)
                                               .Build(),
                    WorkerCalendarDayBuilder<T>.Create(workerId, secondDate.AddDays(2))
                                               .AddRecord(defaultRecord)
                                               .Build()
                });

            var shopCalendarRange = await workerScheduleRepository.ReadShopCalendarRangeAsync(shopId, firstDate, secondDate);
            shopCalendarRange.ShopCalendarDays.Length.Should().Be(2);

            shopCalendarRange.Should().BeEquivalentTo(ShopCalendarRangeBuilder<T>
                                                      .Create(shopId, firstDate, secondDate)
                                                      .AddShopCalendarDay(firstDate,
                                                                          shopBuilder => shopBuilder.AddWorkerDay(workerId,
                                                                                                                  workerBuilder => workerBuilder.AddRecord(defaultRecord)))
                                                      .AddShopCalendarDay(secondDate,
                                                                          shopBuilder => shopBuilder.AddWorkerDay(workerId,
                                                                                                                  workerBuilder => workerBuilder.AddRecord(defaultRecord)))
                                                      .Build());
        }

        [Test]
        public async Task ThreeMonthRangeTest()
        {
            var firstDate = new DateTime(2019, 10, 1);
            var secondDate = new DateTime(2019, 11, 1);
            var thirdDate = new DateTime(2019, 12, 1);

            await workerScheduleRepository.WriteAsync(shopId, new[]
                {
                    WorkerCalendarDayBuilder<T>.Create(workerId, firstDate)
                                               .AddRecord(defaultRecord)
                                               .Build(),
                    WorkerCalendarDayBuilder<T>.Create(workerId, secondDate)
                                               .AddRecord(defaultRecord)
                                               .Build(),
                    WorkerCalendarDayBuilder<T>.Create(workerId, thirdDate)
                                               .AddRecord(defaultRecord)
                                               .Build()
                });

            var shopCalendarRange = await workerScheduleRepository.ReadShopCalendarRangeAsync(shopId, firstDate, thirdDate);
            shopCalendarRange.ShopCalendarDays.Length.Should().Be(3);

            shopCalendarRange.Should().BeEquivalentTo(ShopCalendarRangeBuilder<T>
                                                          .Create(shopId, firstDate, thirdDate)
                                                          .AddShopCalendarDay(firstDate,
                                                                      shopBuilder => shopBuilder.AddWorkerDay(workerId,
                                                                                                              workerBuilder => workerBuilder.AddRecord(defaultRecord)))
                                                          .AddShopCalendarDay(secondDate,
                                                                      shopBuilder => shopBuilder.AddWorkerDay(workerId,
                                                                                                              workerBuilder => workerBuilder.AddRecord(defaultRecord)))
                                                          .AddShopCalendarDay(thirdDate,
                                                                      shopBuilder => shopBuilder.AddWorkerDay(workerId,
                                                                                                              workerBuilder => workerBuilder.AddRecord(defaultRecord)))
                                                          .Build());
        }

        [Test]
        public async Task EmptyAndNonEmptyMonthRangeTest()
        {
            var firstDate = new DateTime(2019, 10, 1);
            var secondDate = new DateTime(2019, 12, 1);
            var thirdDate = new DateTime(2019, 12, 1);

            await workerScheduleRepository.WriteAsync(shopId, new[]
                {
                    WorkerCalendarDayBuilder<T>.Create(workerId, thirdDate)
                                               .AddRecord(defaultRecord)
                                               .Build()
                });

            var shopCalendarRange = await workerScheduleRepository.ReadShopCalendarRangeAsync(shopId, firstDate, thirdDate);
            shopCalendarRange.ShopCalendarDays.Length.Should().Be(1);

            shopCalendarRange.Should().BeEquivalentTo(ShopCalendarRangeBuilder<T>
                                                      .Create(shopId, firstDate, thirdDate)
                                                      .AddShopCalendarDay(secondDate,
                                                                          shopBuilder => shopBuilder.AddWorkerDay(workerId,
                                                                                                                  workerBuilder => workerBuilder.AddRecord(defaultRecord)))
                                                      .Build());
        }

        private T CreateRecord(TimePeriod period)
        {
            var record = fixture.Create<T>();
            record.Period = period;
            return record;
        }

        [Injected]
        private ICalendarRepository<T> workerScheduleRepository;

        [Injected]
        private IFixture fixture;

        private Guid shopId;
        private Guid workerId;
        private DateTime currentDate;
        private T defaultRecord;
    }
}