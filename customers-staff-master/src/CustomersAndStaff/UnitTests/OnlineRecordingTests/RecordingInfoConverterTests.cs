using System;
using System.Linq;

using FluentAssertions;
using FluentAssertions.Extensions;

using Market.Api.Models.Products;
using Market.Api.Models.Shops;
using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Models.OnlineRecording;
using Market.CustomersAndStaff.Models.ServiceCalendar;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.OnlineApi.Converters.Mappers;
using Market.CustomersAndStaff.OnlineApi.Converters.RecordingInfo;
using Market.CustomersAndStaff.OnlineApi.Dto.Common;
using Market.CustomersAndStaff.OnlineApi.Dto.RecordingInfo;
using Market.CustomersAndStaff.Tests.Core;
using Market.CustomersAndStaff.Tests.Core.Builders;

using NUnit.Framework;

using Serilog.Core;

namespace Market.CustomersAndStaff.UnitTests.OnlineRecordingTests
{
    public class RecordingInfoConverterTests
    {
        [SetUp]
        public void SetUp()
        {
            recordingInfoConverter = new RecordingInfoConverter(new MapperWrapper(Logger.None));
        }

        [Test]
        public void EmptyTest()
        {
            var info = recordingInfoConverter.CreateInfo(new Shop(),
                                                         new OnlineService[0],
                                                         new Product[0],
                                                         new Worker[0],
                                                         new ShopCalendarRange<WorkerScheduleRecord>
                                                             {
                                                                 StartDate = 10.January(2019),
                                                                 EndDate = 9.February(2019),
                                                                 ShopCalendarDays = new ShopCalendarDay<WorkerScheduleRecord>[0],
                                                             },
                                                         new ShopCalendarRange<ServiceCalendarRecord>
                                                             {
                                                                 StartDate = 10.January(2019),
                                                                 EndDate = 9.February(2019),
                                                                 ShopCalendarDays = new ShopCalendarDay<ServiceCalendarRecord>[0],
                                                             },
                                                         10.January(2019));

            info.Should().BeEquivalentTo(new RecordingInfoDto
                {
                    Services = new ServiceDto[0],
                    Workers = new WorkerDto[0],
                    Today = 10.January(2019),
                });
        }

        [Test]
        public void ShopInfoTest()
        {
            var shopName = RandomStringGenerator.GenerateRandomCyrillic(50);
            var shopAddress = RandomStringGenerator.GenerateRandomCyrillic(60);

            var info = recordingInfoConverter.CreateInfo(new Shop
                                                             {
                                                                 Name = shopName,
                                                                 Address = shopAddress,
                                                             },
                                                         new OnlineService[0],
                                                         new Product[0],
                                                         new Worker[0],
                                                         new ShopCalendarRange<WorkerScheduleRecord>
                                                             {
                                                                 StartDate = 10.January(2019),
                                                                 EndDate = 9.February(2019),
                                                                 ShopCalendarDays = new ShopCalendarDay<WorkerScheduleRecord>[0],
                                                             },
                                                         new ShopCalendarRange<ServiceCalendarRecord>
                                                             {
                                                                 StartDate = 10.January(2019),
                                                                 EndDate = 9.February(2019),
                                                                 ShopCalendarDays = new ShopCalendarDay<ServiceCalendarRecord>[0],
                                                             },
                                                         10.January(2019));

            info.Should().BeEquivalentTo(new RecordingInfoDto
                {
                    Name = shopName,
                    Address = shopAddress,
                    Services = new ServiceDto[0],
                    Workers = new WorkerDto[0],
                    Today = 10.January(2019),
                });
        }

        [Test]
        public void ServiceInfoTest()
        {
            var products = new[]
                {
                    new Product
                        {
                            Id = Guid.NewGuid(),
                            GroupId = Guid.NewGuid().ToString(),
                            Name = RandomStringGenerator.GenerateRandomCyrillic(20),
                            PricesInfo = new PriceInfo {SellPrice = random.Next(100000) / 100m},
                        },
                    new Product
                        {
                            Id = Guid.NewGuid(),
                            GroupId = null,
                            Name = RandomStringGenerator.GenerateRandomCyrillic(20),
                            PricesInfo = new PriceInfo {SellPrice = random.Next(100000) / 100m},
                        },
                    new Product
                        {
                            Id = Guid.NewGuid(),
                            GroupId = Guid.NewGuid().ToString(),
                            Name = RandomStringGenerator.GenerateRandomCyrillic(20),
                        },
                    new Product
                        {
                            Id = Guid.NewGuid(),
                            GroupId = Guid.NewGuid().ToString(),
                            Name = RandomStringGenerator.GenerateRandomCyrillic(20),
                            PricesInfo = new PriceInfo(),
                        },
                    new Product
                        {
                            Id = Guid.NewGuid(),
                            GroupId = Guid.NewGuid().ToString(),
                            Name = RandomStringGenerator.GenerateRandomCyrillic(20),
                            PricesInfo = new PriceInfo {SellPrice = random.Next(100000) / 100m},
                        },
                };

            var onlineServices = products.Skip(1).Select(x => x.Id.Value).Append(Guid.NewGuid()).Select(x => new OnlineService {ProductId = x}).ToArray();

            var info = recordingInfoConverter.CreateInfo(new Shop(),
                                                         onlineServices,
                                                         products,
                                                         new Worker[0],
                                                         new ShopCalendarRange<WorkerScheduleRecord>
                                                             {
                                                                 StartDate = 10.January(2019),
                                                                 EndDate = 9.February(2019),
                                                                 ShopCalendarDays = new ShopCalendarDay<WorkerScheduleRecord>[0],
                                                             },
                                                         new ShopCalendarRange<ServiceCalendarRecord>
                                                             {
                                                                 StartDate = 10.January(2019),
                                                                 EndDate = 9.February(2019),
                                                                 ShopCalendarDays = new ShopCalendarDay<ServiceCalendarRecord>[0],
                                                             },
                                                         10.January(2019));

            info.Should().BeEquivalentTo(new RecordingInfoDto
                {
                    Services = new[]
                        {
                            new ServiceDto
                                {
                                    ServiceId = products[1].Id.Value,
                                    GroupId = Guid.Empty,
                                    Name = products[1].Name,
                                    Price = products[1].PricesInfo.SellPrice,
                                    Workers = new Guid[0],
                                },
                            new ServiceDto
                                {
                                    ServiceId = products[2].Id.Value,
                                    GroupId = Guid.Parse(products[2].GroupId),
                                    Name = products[2].Name,
                                    Price = null,
                                    Workers = new Guid[0],
                                },
                            new ServiceDto
                                {
                                    ServiceId = products[3].Id.Value,
                                    GroupId = Guid.Parse(products[3].GroupId),
                                    Name = products[3].Name,
                                    Price = null,
                                    Workers = new Guid[0],
                                },
                            new ServiceDto
                                {
                                    ServiceId = products[4].Id.Value,
                                    GroupId = Guid.Parse(products[4].GroupId),
                                    Name = products[4].Name,
                                    Price = products[4].PricesInfo.SellPrice,
                                    Workers = new Guid[0],
                                },
                        },
                    Workers = new WorkerDto[0],
                    Today = 10.January(2019),
                });
        }

        [Test]
        public void WorkerInfoTest()
        {
            #region Arrange

            var workers = new[]
                {
                    new Worker
                        {
                            Id = Guid.NewGuid(),
                            FullName = RandomStringGenerator.GenerateRandomCyrillic(50),
                            Position = RandomStringGenerator.GenerateRandomCyrillic(50),
                            IsAvailableOnline = true,
                            IsDeleted = false,
                        },
                    new Worker
                        {
                            Id = Guid.NewGuid(),
                            FullName = RandomStringGenerator.GenerateRandomCyrillic(50),
                            Position = RandomStringGenerator.GenerateRandomCyrillic(50),
                            IsAvailableOnline = true,
                            IsDeleted = false,
                        },
                    new Worker
                        {
                            Id = Guid.NewGuid(),
                            FullName = RandomStringGenerator.GenerateRandomCyrillic(50),
                            Position = RandomStringGenerator.GenerateRandomCyrillic(50),
                            IsAvailableOnline = true,
                            IsDeleted = false,
                        },
                    new Worker
                        {
                            Id = Guid.NewGuid(),
                            FullName = RandomStringGenerator.GenerateRandomCyrillic(50),
                            Position = RandomStringGenerator.GenerateRandomCyrillic(50),
                            IsAvailableOnline = false,
                            IsDeleted = false,
                        },
                    new Worker
                        {
                            Id = Guid.NewGuid(),
                            FullName = RandomStringGenerator.GenerateRandomCyrillic(50),
                            Position = RandomStringGenerator.GenerateRandomCyrillic(50),
                            IsAvailableOnline = true,
                            IsDeleted = true,
                        },
                    new Worker
                        {
                            Id = Guid.NewGuid(),
                            FullName = RandomStringGenerator.GenerateRandomCyrillic(50),
                            Position = RandomStringGenerator.GenerateRandomCyrillic(50),
                            IsAvailableOnline = true,
                            IsDeleted = false,
                        },
                };

            var workerSchedule =
                ShopCalendarRangeBuilder<WorkerScheduleRecord>
                    .Create(Guid.Empty, 10.January(2019), 9.February(2019))
                    .AddShopCalendarDay(10.January(2019),
                                        dayBuilder => dayBuilder
                                                      .AddWorkerDay(workers[0].Id,
                                                                    workerBuilder => workerBuilder
                                                                        .AddRecord(new TimePeriod(8.Hours() + 30.Minutes(), 17.Hours())))
                                                      .AddWorkerDay(workers[1].Id,
                                                                    workerBuilder => workerBuilder
                                                                                     .AddRecord(new TimePeriod(4.Hours() + 30.Minutes(), 8.Hours()))
                                                                                     .AddRecord(new TimePeriod(17.Hours() + 30.Minutes(), 22.Hours())))
                                                      .AddWorkerDay(workers[5].Id,
                                                                    workerBuilder => workerBuilder
                                                                        .AddRecord(new TimePeriod(12.Hours(), 14.Hours()))))
                    .AddShopCalendarDay(22.January(2019),
                                        dayBuilder => dayBuilder
                                                      .AddWorkerDay(workers[0].Id,
                                                                    workerBuilder => workerBuilder
                                                                        .AddRecord(new TimePeriod(8.Hours() + 30.Minutes(), 17.Hours())))
                                                      .AddWorkerDay(workers[3].Id,
                                                                    workerBuilder => workerBuilder
                                                                        .AddRecord(new TimePeriod(8.Hours() + 30.Minutes(), 17.Hours())))
                                                      .AddWorkerDay(workers[4].Id,
                                                                    workerBuilder => workerBuilder
                                                                        .AddRecord(new TimePeriod(8.Hours() + 30.Minutes(), 17.Hours()))))
                    .Build();

            var serviceCalendar =
                ShopCalendarRangeBuilder<ServiceCalendarRecord>
                    .Create(Guid.Empty, 10.January(2019), 9.February(2019))
                    .AddShopCalendarDay(10.January(2019),
                                        dayBuilder => dayBuilder
                                                      .AddWorkerDay(workers[0].Id,
                                                                    workerBuilder => workerBuilder
                                                                                     .AddRecord(Guid.NewGuid(), TimePeriod.CreateByHours(10, 11))
                                                                                     .AddRecord(Guid.NewGuid(), TimePeriod.CreateByHours(11, 12), r => r.WithRecordStatus(RecordStatus.Canceled))
                                                                                     .AddRecord(Guid.NewGuid(), new TimePeriod(13.Hours() + 15.Minutes(), 13.Hours() + 45.Minutes())))
                                                      .AddWorkerDay(workers[2].Id,
                                                                    workerBuilder => workerBuilder
                                                                        .AddRecord(Guid.NewGuid(), TimePeriod.CreateByHours(10, 11)))
                                                      .AddWorkerDay(workers[5].Id,
                                                                    workerBuilder => workerBuilder
                                                                        .AddRecord(Guid.NewGuid(), new TimePeriod(11.Hours(), 15.Hours()))))
                    .AddShopCalendarDay(22.January(2019),
                                        dayBuilder => dayBuilder
                                                      .AddWorkerDay(workers[0].Id,
                                                                    workerBuilder => workerBuilder
                                                                                     .AddRecord(Guid.NewGuid(), TimePeriod.CreateByHours(8, 9))
                                                                                     .AddRecord(Guid.NewGuid(), new TimePeriod(16.Hours() + 30.Minutes(), 17.Hours() + 30.Minutes())))
                                                      .AddWorkerDay(workers[3].Id,
                                                                    workerBuilder => workerBuilder
                                                                        .AddRecord(Guid.NewGuid(), TimePeriod.CreateByHours(10, 11)))
                                                      .AddWorkerDay(workers[4].Id,
                                                                    workerBuilder => workerBuilder
                                                                        .AddRecord(Guid.NewGuid(), TimePeriod.CreateByHours(10, 11))))
                    .Build();

            #endregion

            var info = recordingInfoConverter.CreateInfo(new Shop(),
                                                         new OnlineService[0],
                                                         new Product[0],
                                                         workers,
                                                         workerSchedule,
                                                         serviceCalendar,
                                                         10.January(2019));

            #region Assert

            info.Should().BeEquivalentTo(new RecordingInfoDto
                {
                    Services = new ServiceDto[0],
                    Workers = new[]
                        {
                            new WorkerDto
                                {
                                    WorkerId = workers[0].Id,
                                    Name = workers[0].FullName,
                                    Position = workers[0].Position,
                                    Schedule = new[]
                                        {
                                            new WorkerCalendarRecordDto
                                                {
                                                    Date = 10.January(2019),
                                                    WorkingTime = new[] {new TimePeriodDto(9.Hours(), 17.Hours())},
                                                    AvailableTime = new[]
                                                        {
                                                            new TimePeriodDto(9.Hours(), 10.Hours()),
                                                            new TimePeriodDto(11.Hours(), 13.Hours()),
                                                            new TimePeriodDto(14.Hours(), 17.Hours()),
                                                        }
                                                },
                                            new WorkerCalendarRecordDto
                                                {
                                                    Date = 22.January(2019),
                                                    WorkingTime = new[] {new TimePeriodDto(9.Hours(), 17.Hours())},
                                                    AvailableTime = new[] {new TimePeriodDto(9.Hours(), 16.Hours())},
                                                },
                                        },
                                },
                            new WorkerDto
                                {
                                    WorkerId = workers[1].Id,
                                    Name = workers[1].FullName,
                                    Position = workers[1].Position,
                                    Schedule = new[]
                                        {
                                            new WorkerCalendarRecordDto
                                                {
                                                    Date = 10.January(2019),
                                                    WorkingTime = new[]
                                                        {
                                                            new TimePeriodDto(5.Hours(), 8.Hours()),
                                                            new TimePeriodDto(18.Hours(), 22.Hours()),
                                                        },
                                                    AvailableTime = new[]
                                                        {
                                                            new TimePeriodDto(5.Hours(), 8.Hours()),
                                                            new TimePeriodDto(18.Hours(), 22.Hours()),
                                                        },
                                                },
                                        },
                                },
                            new WorkerDto
                                {
                                    WorkerId = workers[5].Id,
                                    Name = workers[5].FullName,
                                    Position = workers[5].Position,
                                    Schedule = new[]
                                        {
                                            new WorkerCalendarRecordDto
                                                {
                                                    Date = 10.January(2019),
                                                    WorkingTime = new[] {new TimePeriodDto(12.Hours(), 14.Hours())},
                                                    AvailableTime = new TimePeriodDto[0],
                                                },
                                        }
                                },
                        },
                    Today = 10.January(2019),
                });

            #endregion
        }

        [Test]
        public void LinkServicesAndWorkersTest()
        {
            var products = new[]
                {
                    new Product
                        {
                            Id = Guid.NewGuid(),
                            GroupId = Guid.NewGuid().ToString(),
                            Name = RandomStringGenerator.GenerateRandomCyrillic(20),
                            PricesInfo = new PriceInfo {SellPrice = random.Next(100000) / 100m},
                        },
                    new Product
                        {
                            Id = Guid.NewGuid(),
                            GroupId = null,
                            Name = RandomStringGenerator.GenerateRandomCyrillic(20),
                            PricesInfo = new PriceInfo {SellPrice = random.Next(100000) / 100m},
                        },
                };

            var onlineServices = products.Select(x => new OnlineService {ProductId = x.Id.Value}).ToArray();

            var workers = new[]
                {
                    new Worker
                        {
                            Id = Guid.NewGuid(),
                            FullName = RandomStringGenerator.GenerateRandomCyrillic(50),
                            Position = RandomStringGenerator.GenerateRandomCyrillic(50),
                            IsAvailableOnline = true,
                            IsDeleted = false,
                        },
                    new Worker
                        {
                            Id = Guid.NewGuid(),
                            FullName = RandomStringGenerator.GenerateRandomCyrillic(50),
                            Position = RandomStringGenerator.GenerateRandomCyrillic(50),
                            IsAvailableOnline = true,
                            IsDeleted = false,
                        },
                };

            var workerSchedule =
                ShopCalendarRangeBuilder<WorkerScheduleRecord>
                    .Create(Guid.Empty, 10.January(2019), 9.February(2019))
                    .AddShopCalendarDay(10.January(2019),
                                        dayBuilder => dayBuilder
                                                      .AddWorkerDay(workers[0].Id,
                                                                    workerBuilder => workerBuilder
                                                                        .AddRecord(new TimePeriod(8.Hours() + 30.Minutes(), 17.Hours())))
                                                      .AddWorkerDay(workers[1].Id,
                                                                    workerBuilder => workerBuilder
                                                                        .AddRecord(new TimePeriod(8.Hours() + 30.Minutes(), 17.Hours()))))
                    .Build();

            var info = recordingInfoConverter.CreateInfo(new Shop(),
                                                         onlineServices,
                                                         products,
                                                         workers,
                                                         workerSchedule,
                                                         new ShopCalendarRange<ServiceCalendarRecord>
                                                             {
                                                                 StartDate = 10.January(2019),
                                                                 EndDate = 9.February(2019),
                                                                 ShopCalendarDays = new ShopCalendarDay<ServiceCalendarRecord>[0],
                                                             },
                                                         10.January(2019));

            info.Services.Select(x => x.Workers).Should().AllBeEquivalentTo(workers.Select(x => x.Id).ToArray());
        }

        private RecordingInfoConverter recordingInfoConverter;
        private readonly Random random = new Random();
    }
}