using System;
using System.Threading.Tasks;

using AutoFixture;

using FluentAssertions;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.ServiceApi.Client;
using Market.CustomersAndStaff.Tests.Core.Builders;
using Market.CustomersAndStaff.Tests.Core.Configuration;

using NUnit.Framework;

namespace Market.CustomersAndStaff.ApiTests
{
    public class WorkersSchedulesApiTests : IMainSuite
    {
        [GroboSetUp]
        public void SetUp()
        {
            shopId = Guid.NewGuid();
            defaultRecord = CreateRecord(new TimePeriod(TimeSpan.FromHours(3), TimeSpan.FromHours(5)));
            var serviceApiClient = new ServiceApiClient(new ServiceApiClientSettings(new[] { new Uri("http://localhost:16001"), }, TimeSpan.FromSeconds(15), null));
            workersSchedulesApiClient = serviceApiClient.WorkersSchedules;
        }

        [Test]
        public void BadDatesTest()
        {
            var firstDate = new DateTime(2019, 10, 1);
            var secondDate = new DateTime(2019, 11, 1);

            Assert.Throws<AggregateException>(() => workersSchedulesApiClient.Get(shopId, secondDate, firstDate).Wait());
        }

        [Test]
        public void TooBigMonthsPeriodTest()
        {
            var firstDate = new DateTime(2019, 9, 1);
            var secondDate = new DateTime(2019, 12, 1);

            Assert.Throws<AggregateException>(() => workersSchedulesApiClient.Get(shopId, firstDate, secondDate).Wait());
        }

        [Test]
        public async Task EmptySchedulesTest()
        {
            var firstDate = new DateTime(2019, 10, 1);
            var secondDate = new DateTime(2019, 12, 1);

            var shopCalendarRange = await workersSchedulesApiClient.Get(shopId, firstDate, secondDate);
            shopCalendarRange.ShopCalendarDays.Length.Should().Be(0);
        }

        [Test]
        public async Task OneMonthWithEmptySchedulesRangeTest()
        {
            var firstDate = new DateTime(2019, 10, 1);
            var secondDate = new DateTime(2019, 10, 10);
            var workerId = Guid.NewGuid();

            await workerScheduleRepository.WriteAsync(shopId, new[]
                {
                    WorkerCalendarDayBuilder<WorkerScheduleRecord>.Create(workerId, secondDate.AddDays(2))
                                               .AddRecord(defaultRecord)
                                               .Build()
                });

            var shopCalendarRange = await workersSchedulesApiClient.Get(shopId, firstDate, secondDate);
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
                    WorkerCalendarDayBuilder<WorkerScheduleRecord>.Create(workerId, firstDate)
                                               .AddRecord(defaultRecord)
                                               .Build(),
                    WorkerCalendarDayBuilder<WorkerScheduleRecord>.Create(workerId, secondDate)
                                               .AddRecord(defaultRecord)
                                               .Build(),
                    WorkerCalendarDayBuilder<WorkerScheduleRecord>.Create(workerId, secondDate.AddDays(2))
                                               .AddRecord(defaultRecord)
                                               .Build()
                });

            var shopCalendarRange = await workersSchedulesApiClient.Get(shopId, firstDate, secondDate);
            shopCalendarRange.ShopCalendarDays.Length.Should().Be(2);

            shopCalendarRange.Should().BeEquivalentTo(ShopCalendarRangeBuilder<WorkerScheduleRecord>
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
        public async Task ThreeMonthSchedulesTest()
        {
            var firstDate = new DateTime(2019, 10, 1);
            var secondDate = new DateTime(2019, 11, 1);
            var thirdDate = new DateTime(2019, 12, 1);
            var workerId = Guid.NewGuid();

            await workerScheduleRepository.WriteAsync(shopId, new[]
                {
                    WorkerCalendarDayBuilder<WorkerScheduleRecord>.Create(workerId, firstDate)
                                               .AddRecord(defaultRecord)
                                               .Build(),
                    WorkerCalendarDayBuilder<WorkerScheduleRecord>.Create(workerId, secondDate)
                                               .AddRecord(defaultRecord)
                                               .Build(),
                    WorkerCalendarDayBuilder<WorkerScheduleRecord>.Create(workerId, thirdDate)
                                               .AddRecord(defaultRecord)
                                               .Build()
                });

            var shopCalendarRange = await workersSchedulesApiClient.Get(shopId, firstDate, thirdDate);
            shopCalendarRange.ShopCalendarDays.Length.Should().Be(3);

            shopCalendarRange.Should().BeEquivalentTo(ShopCalendarRangeBuilder<WorkerScheduleRecord>
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
        public async Task EmptyAndNonEmptySchedulesMonthTest()
        {
            var firstDate = new DateTime(2019, 10, 1);
            var secondDate = new DateTime(2019, 12, 1);
            var thirdDate = new DateTime(2019, 12, 1);
            var workerId = Guid.NewGuid();

            await workerScheduleRepository.WriteAsync(shopId, new[]
                {
                    WorkerCalendarDayBuilder<WorkerScheduleRecord>.Create(workerId, thirdDate)
                                               .AddRecord(defaultRecord)
                                               .Build()
                });

            var shopCalendarRange = await workersSchedulesApiClient.Get(shopId, firstDate, thirdDate);
            shopCalendarRange.ShopCalendarDays.Length.Should().Be(1);

            shopCalendarRange.Should().BeEquivalentTo(ShopCalendarRangeBuilder<WorkerScheduleRecord>
                                                      .Create(shopId, firstDate, thirdDate)
                                                      .AddShopCalendarDay(secondDate,
                                                                          shopBuilder => shopBuilder.AddWorkerDay(workerId,
                                                                                                                  workerBuilder => workerBuilder.AddRecord(defaultRecord)))
                                                      .Build());
        }

        private WorkerScheduleRecord CreateRecord(TimePeriod period)
        {
            var record = fixture.Create<WorkerScheduleRecord>();
            record.Period = period;
            return record;
        }

        private IWorkersSchedulesApiClient workersSchedulesApiClient;

        [Injected]
        private IFixture fixture;

        [Injected]
        private ICalendarRepository<WorkerScheduleRecord> workerScheduleRepository;

        private Guid shopId;
        private WorkerScheduleRecord defaultRecord;
    }
}