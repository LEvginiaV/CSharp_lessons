using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using FluentAssertions;
using FluentAssertions.Extensions;

using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Models.ServiceCalendar;
using Market.CustomersAndStaff.ModelValidators.Calendar;
using Market.CustomersAndStaff.ModelValidators.Periods;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Services.ServiceCalendar;
using Market.CustomersAndStaff.Tests.Core.Builders;
using Market.CustomersAndStaff.UnitTests.Helpers;

using Moq;

using NUnit.Framework;

namespace Market.CustomersAndStaff.UnitTests.ValidatorsTests
{
    public class ServiceCalendarRecordValidatorTests
    {
        [SetUp]
        public void SetUp()
        {
            locker = new FakeLocker();
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<ServiceCalendarRecord, ServiceCalendarRemovedRecord>()));
            mainRepositoryMock = new Mock<ICalendarRepository<ServiceCalendarRecord>>(MockBehavior.Strict);
            removedRepositoryMock = new Mock<ICalendarRepository<ServiceCalendarRemovedRecord>>(MockBehavior.Strict);
            calendarService = new CalendarService(locker, mainRepositoryMock.Object, removedRepositoryMock.Object, mapper,
                                                  new ServiceCalendarRecordValidator(new PeriodValidator<ServiceCalendarRecord>()));
            shopId = Guid.NewGuid();
            now = DateTime.UtcNow.Date;
        }

        [Test]
        [TestCaseSource(nameof(TestData))]
        public void CommentLengthWhileCreatingTest(string comment, bool isValid)
        {
            var workerId = Guid.NewGuid();

            #region Prepare mock
            mainRepositoryMock.Setup(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>>()))
                              .Returns(Task.FromResult(0))
                              .Callback((Action<Guid, WorkerCalendarDay<ServiceCalendarRecord>>)((s, d) => { }));

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(Guid.Empty, new TimePeriod(4.Hours(), 6.Hours()))
                                               .Build()
                                       ));
            #endregion

            var (_, validationResult) = calendarService.CreateAsync(shopId, now, workerId,
                                                                     ServiceCalendarRecordBuilder
                                                                         .Create(Guid.Empty, new TimePeriod(7.Hours(), 9.Hours()))
                                                                         .WithComment(comment)
                                                                         .Build()).Result;
            validationResult.IsSuccess.Should().Be(isValid);
        }

        [Test]
        [TestCaseSource(nameof(TestData))]
        public void CommentLengthWhileUpdatingSingleRecordTest(string comment, bool isValid)
        {
            var workerId = Guid.NewGuid();

            #region Prepare mock
            mainRepositoryMock.Setup(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>>()))
                              .Returns(Task.FromResult(0))
                              .Callback((Action<Guid, WorkerCalendarDay<ServiceCalendarRecord>>)((s, d) => { }));

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(Guid.Empty, new TimePeriod(4.Hours(), 6.Hours()))
                                               .Build()
                                       ));
            #endregion

            var (recordId, _) = calendarService.CreateAsync(shopId, now, workerId,
                                                                     ServiceCalendarRecordBuilder
                                                                         .Create(Guid.Empty, new TimePeriod(7.Hours(), 9.Hours()))
                                                                         .Build()).Result;

            var validationResult = calendarService.UpdateAsync(shopId, now, workerId,
                                                                    ServiceCalendarRecordBuilder
                                                                        .Create(recordId, new TimePeriod(7.Hours(), 9.Hours()))
                                                                        .WithComment(comment)
                                                                        .Build()).Result; 
            validationResult.IsSuccess.Should().Be(isValid);
        }

        [Test]
        [TestCaseSource(nameof(TestData))]
        public void CommentLengthWhileUpdatingTwoRecordsTest(string comment, bool isValid)
        {
            var workerId = Guid.NewGuid();
            var newWorkerId = Guid.NewGuid();

            #region Prepare mock
            mainRepositoryMock.Setup(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>>()))
                              .Returns(Task.FromResult(0))
                              .Callback((Action<Guid, WorkerCalendarDay<ServiceCalendarRecord>>)((s, d) => { }));

            mainRepositoryMock.Setup(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>[]>()))
                              .Returns(Task.FromResult(0))
                              .Callback((Action<Guid, WorkerCalendarDay<ServiceCalendarRecord>[]>)((s, d) => { }));

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(Guid.Empty, new TimePeriod(4.Hours(), 6.Hours()))
                                               .Build()
                                       ));

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, newWorkerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(Guid.Empty, new TimePeriod(4.Hours(), 6.Hours()))
                                               .Build()
                                       ));
            #endregion

            var (recordId, _) = calendarService.CreateAsync(shopId, now, workerId,
                                                            ServiceCalendarRecordBuilder
                                                                .Create(Guid.Empty, new TimePeriod(7.Hours(), 9.Hours()))
                                                                .Build()).Result;

            var validationResult = calendarService.UpdateAsync(shopId, now, workerId,
                                                               ServiceCalendarRecordBuilder
                                                                   .Create(recordId, new TimePeriod(7.Hours(), 9.Hours()))
                                                                   .WithComment(comment)
                                                                   .Build(), null, newWorkerId).Result;
            validationResult.IsSuccess.Should().Be(isValid);
        }

        private static IEnumerable<TestCaseData> TestData()
        {
            yield return new TestCaseData(new object[]
                    {
                        "500 symbols |Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt",
                        true
                    })
                    {TestName = "500 symbols is correct"};
            yield return new TestCaseData(new object[]
                    {
                        "501 symbols |Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt.",
                        false
                    })
                    { TestName = "501 symbols is not correct" };
        }

        private ICalendarService calendarService;
        private Mock<ICalendarRepository<ServiceCalendarRecord>> mainRepositoryMock;
        private Mock<ICalendarRepository<ServiceCalendarRemovedRecord>> removedRepositoryMock;
        private Guid shopId;
        private DateTime now;
        private FakeLocker locker;
    }
}