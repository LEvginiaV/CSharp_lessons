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

namespace Market.CustomersAndStaff.UnitTests.ServiceCalendarTests
{
    public class CalendarTests
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
        public void AddRecordToNewDayTest()
        {
            var workerId = Guid.NewGuid();
            WorkerCalendarDay<ServiceCalendarRecord> savedDay = null;

            mainRepositoryMock.Setup(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>>()))
                              .Returns(Task.FromResult(0))
                              .Callback((Action<Guid, WorkerCalendarDay<ServiceCalendarRecord>>)((s, d) => savedDay = d));

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .Build()
                                       ));

            var (recordId, validation) = calendarService.CreateAsync(shopId, now, workerId,
                                                                     ServiceCalendarRecordBuilder
                                                                         .Create(Guid.Empty, new TimePeriod(7.Hours(), 9.Hours()))
                                                                         .Build()).Result;

            validation.IsSuccess.Should().BeTrue();

            savedDay.Should().BeEquivalentTo(WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                             .Create(workerId, now)
                                             .AddRecord(recordId, new TimePeriod(7.Hours(), 9.Hours()))
                                             .Build());

            mainRepositoryMock.Verify(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>>()), Times.Once);
            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void AddRecordToExistingDayTest()
        {
            var workerId = Guid.NewGuid();
            WorkerCalendarDay<ServiceCalendarRecord> savedDay = null;
            var currentRecordId = Guid.NewGuid();

            mainRepositoryMock.Setup(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>>()))
                              .Returns(Task.FromResult(0))
                              .Callback((Action<Guid, WorkerCalendarDay<ServiceCalendarRecord>>)((s, d) => savedDay = d));

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                               .Build()
                                       ));

            var (recordId, validation) = calendarService.CreateAsync(shopId, now, workerId,
                                                                     ServiceCalendarRecordBuilder
                                                                         .Create(Guid.Empty, new TimePeriod(7.Hours(), 9.Hours()))
                                                                         .Build()).Result;

            validation.IsSuccess.Should().BeTrue();

            savedDay.Should().BeEquivalentTo(WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                             .Create(workerId, now)
                                             .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                             .AddRecord(recordId, new TimePeriod(7.Hours(), 9.Hours()))
                                             .Build());

            mainRepositoryMock.Verify(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>>()), Times.Once);
            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void AddRecordToExistingPeriodTest()
        {
            var workerId = Guid.NewGuid();
            var currentRecordId = Guid.NewGuid();

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                               .Build()
                                       ));

            var (_, validation) = calendarService.CreateAsync(shopId, now, workerId,
                                                              ServiceCalendarRecordBuilder
                                                                  .Create(Guid.Empty, new TimePeriod(5.Hours(), 7.Hours()))
                                                                  .Build()).Result;

            validation.IsSuccess.Should().BeFalse();

            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void AddRecordToExistingPeriodWithCanceledRecordTest()
        {
            var workerId = Guid.NewGuid();
            WorkerCalendarDay<ServiceCalendarRecord> savedDay = null;
            var currentRecordId = Guid.NewGuid();

            mainRepositoryMock.Setup(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>>()))
                              .Returns(Task.FromResult(0))
                              .Callback((Action<Guid, WorkerCalendarDay<ServiceCalendarRecord>>)((s, d) => savedDay = d));

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()), builder => builder.WithRecordStatus(RecordStatus.Canceled))
                                               .Build()
                                       ));

            var (recordId, validation) = calendarService.CreateAsync(shopId, now, workerId,
                                                                     ServiceCalendarRecordBuilder
                                                                         .Create(Guid.Empty, new TimePeriod(5.Hours(), 7.Hours()))
                                                                         .Build()).Result;

            validation.IsSuccess.Should().BeTrue();

            savedDay.Should().BeEquivalentTo(WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                             .Create(workerId, now)
                                             .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()), builder => builder.WithRecordStatus(RecordStatus.Canceled))
                                             .AddRecord(recordId, new TimePeriod(5.Hours(), 7.Hours()))
                                             .Build());

            mainRepositoryMock.Verify(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>>()), Times.Once);
            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void UpdateToFreePeriodRecordTest()
        {
            var workerId = Guid.NewGuid();
            WorkerCalendarDay<ServiceCalendarRecord> savedDay = null;
            var currentRecordId = Guid.NewGuid();

            mainRepositoryMock.Setup(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>>()))
                              .Returns(Task.FromResult(0))
                              .Callback((Action<Guid, WorkerCalendarDay<ServiceCalendarRecord>>)((s, d) => savedDay = d));

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                               .Build()
                                       ));

            var validation = calendarService.UpdateAsync(shopId, now, workerId,
                                                         ServiceCalendarRecordBuilder
                                                             .Create(currentRecordId, new TimePeriod(7.Hours(), 9.Hours()))
                                                             .Build()).Result;

            validation.IsSuccess.Should().BeTrue();

            savedDay.Should().BeEquivalentTo(WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                             .Create(workerId, now)
                                             .AddRecord(currentRecordId, new TimePeriod(7.Hours(), 9.Hours()))
                                             .Build());

            mainRepositoryMock.Verify(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>>()), Times.Once);
            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void UpdateToPeriodWithCanceledRecordTest()
        {
            var workerId = Guid.NewGuid();
            WorkerCalendarDay<ServiceCalendarRecord> savedDay = null;
            var currentRecordId = Guid.NewGuid();
            var currentRecordId2 = Guid.NewGuid();

            mainRepositoryMock.Setup(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>>()))
                              .Returns(Task.FromResult(0))
                              .Callback((Action<Guid, WorkerCalendarDay<ServiceCalendarRecord>>)((s, d) => savedDay = d));
            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(currentRecordId, new TimePeriod(7.Hours(), 9.Hours()))
                                               .AddRecord(currentRecordId2, new TimePeriod(4.Hours(), 6.Hours()), builder => builder.WithRecordStatus(RecordStatus.Canceled))
                                               .Build()
                                       ));

            var validation = calendarService.UpdateAsync(shopId, now, workerId,
                                                         ServiceCalendarRecordBuilder
                                                             .Create(currentRecordId, new TimePeriod(5.Hours(), 7.Hours()))
                                                             .Build()).Result;

            validation.IsSuccess.Should().BeTrue();

            savedDay.Should().BeEquivalentTo(WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                             .Create(workerId, now)
                                             .AddRecord(currentRecordId, new TimePeriod(5.Hours(), 7.Hours()))
                                             .AddRecord(currentRecordId2, new TimePeriod(4.Hours(), 6.Hours()), builder => builder.WithRecordStatus(RecordStatus.Canceled))
                                             .Build());

            mainRepositoryMock.Verify(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>>()), Times.Once);
            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void UpdateRecordToExistingPeriodTest()
        {
            var workerId = Guid.NewGuid();
            var currentRecordId = Guid.NewGuid();
            var currentRecordId2 = Guid.NewGuid();

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                               .AddRecord(currentRecordId2, new TimePeriod(7.Hours(), 9.Hours()))
                                               .Build()
                                       ));

            var validation = calendarService.UpdateAsync(shopId, now, workerId,
                                                         ServiceCalendarRecordBuilder
                                                             .Create(currentRecordId, new TimePeriod(6.Hours(), 8.Hours()))
                                                             .Build()).Result;

            validation.IsSuccess.Should().BeFalse();

            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void RemoveRecordTest()
        {
            var workerId = Guid.NewGuid();
            WorkerCalendarDay<ServiceCalendarRecord> savedDay = null;
            WorkerCalendarDay<ServiceCalendarRemovedRecord> savedRemovedDay = null;
            var currentRecordId = Guid.NewGuid();
            var currentRecordId2 = Guid.NewGuid();

            mainRepositoryMock.Setup(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>>()))
                              .Returns(Task.FromResult(0))
                              .Callback((Action<Guid, WorkerCalendarDay<ServiceCalendarRecord>>)((s, d) => savedDay = d));
            removedRepositoryMock.Setup(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRemovedRecord>>()))
                                 .Returns(Task.FromResult(0))
                                 .Callback((Action<Guid, WorkerCalendarDay<ServiceCalendarRemovedRecord>>)((s, d) => savedRemovedDay = d));

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                               .AddRecord(currentRecordId2, new TimePeriod(7.Hours(), 9.Hours()))
                                               .Build()
                                       ));
            removedRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                                 .Returns(Task.FromResult(
                                              WorkerCalendarDayBuilder<ServiceCalendarRemovedRecord>
                                                  .Create(workerId, now)
                                                  .Build()
                                          ));

            var validation = calendarService.RemoveAsync(shopId, now, workerId, currentRecordId2).Result;

            validation.IsSuccess.Should().BeTrue();

            savedDay.Should().BeEquivalentTo(WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                             .Create(workerId, now)
                                             .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                             .Build());

            savedRemovedDay.Should().BeEquivalentTo(WorkerCalendarDayBuilder<ServiceCalendarRemovedRecord>
                                                    .Create(workerId, now)
                                                    .AddRecord(new ServiceCalendarRemovedRecord
                                                        {
                                                            Id = currentRecordId2,
                                                            Period = new TimePeriod(7.Hours(), 9.Hours()),
                                                            RecordStatus = RecordStatus.Removed,
                                                            CustomerStatus = CustomerStatus.Active,
                                                            Comment = "",
                                                            ProductIds = new Guid[0],
                                                        })
                                                    .Build());

            mainRepositoryMock.Verify(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>>()), Times.Once);
            removedRepositoryMock.Verify(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRemovedRecord>>()), Times.Once);
            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            removedRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void RemoveNotExistingRecordTest()
        {
            var workerId = Guid.NewGuid();
            var currentRecordId = Guid.NewGuid();
            var currentRecordId2 = Guid.NewGuid();

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                               .Build()
                                       ));

            var validation = calendarService.RemoveAsync(shopId, now, workerId, currentRecordId2).Result;

            validation.IsSuccess.Should().BeFalse();

            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void RemovePreviousDayTest()
        {
            var workerId = Guid.NewGuid();
            var currentRecordId = Guid.NewGuid();

            var validation = calendarService.RemoveAsync(shopId, now.AddDays(-1), workerId, currentRecordId).Result;

            validation.IsSuccess.Should().BeFalse();

            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [TestCase(CustomerStatus.Active, RecordStatus.Active)]
        [TestCase(CustomerStatus.ActiveAccepted, RecordStatus.Active)]
        [TestCase(CustomerStatus.CanceledBeforeEvent, RecordStatus.Canceled)]
        [TestCase(CustomerStatus.Completed, RecordStatus.Active)]
        [TestCase(CustomerStatus.NotCome, RecordStatus.Canceled)]
        [TestCase(CustomerStatus.NoService, RecordStatus.Canceled)]
        public void UpdateCustomerStatusTest(CustomerStatus customerStatus, RecordStatus recordStatus)
        {
            var workerId = Guid.NewGuid();
            WorkerCalendarDay<ServiceCalendarRecord> savedDay = null;
            var currentRecordId = Guid.NewGuid();

            mainRepositoryMock.Setup(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>>()))
                              .Returns(Task.FromResult(0))
                              .Callback((Action<Guid, WorkerCalendarDay<ServiceCalendarRecord>>)((s, d) => savedDay = d));

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                               .Build()
                                       ));

            var validation = calendarService.UpdateCustomerStatusAsync(shopId, now, workerId, currentRecordId, customerStatus).Result;

            validation.IsSuccess.Should().BeTrue();

            savedDay.Should().BeEquivalentTo(WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                             .Create(workerId, now)
                                             .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()),
                                                        builder => builder.WithCustomerStatus(customerStatus)
                                                                          .WithRecordStatus(recordStatus))
                                             .Build());

            mainRepositoryMock.Verify(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>>()), Times.Once);
            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [TestCase(CustomerStatus.Mistake)]
        public void UpdateCustomerStatusWithRemoveTest(CustomerStatus customerStatus)
        {
            var workerId = Guid.NewGuid();
            WorkerCalendarDay<ServiceCalendarRecord> savedDay = null;
            WorkerCalendarDay<ServiceCalendarRemovedRecord> savedRemovedDay = null;
            var currentRecordId = Guid.NewGuid();

            mainRepositoryMock.Setup(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>>()))
                              .Returns(Task.FromResult(0))
                              .Callback((Action<Guid, WorkerCalendarDay<ServiceCalendarRecord>>)((s, d) => savedDay = d));

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                               .Build()
                                       ));

            removedRepositoryMock.Setup(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRemovedRecord>>()))
                                 .Returns(Task.FromResult(0))
                                 .Callback((Action<Guid, WorkerCalendarDay<ServiceCalendarRemovedRecord>>)((s, d) => savedRemovedDay = d));
            removedRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                                 .Returns(Task.FromResult(
                                              WorkerCalendarDayBuilder<ServiceCalendarRemovedRecord>
                                                  .Create(workerId, now)
                                                  .Build()
                                          ));
            
            var validation = calendarService.UpdateCustomerStatusAsync(shopId, now, workerId, currentRecordId, customerStatus).Result;

            validation.IsSuccess.Should().BeTrue();

            savedDay.Should().BeEquivalentTo(WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                             .Create(workerId, now)
                                             .Build());
            savedRemovedDay.Should().BeEquivalentTo(WorkerCalendarDayBuilder<ServiceCalendarRemovedRecord>
                                                    .Create(workerId, now)
                                                    .AddRecord(new ServiceCalendarRemovedRecord
                                                        {
                                                            Id = currentRecordId,
                                                            Period = new TimePeriod(4.Hours(), 6.Hours()),
                                                            RecordStatus = RecordStatus.Removed,
                                                            CustomerStatus = customerStatus,
                                                            Comment = "",
                                                            ProductIds = new Guid[0],
                                                        })
                                                    .Build());

            mainRepositoryMock.Verify(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>>()), Times.Once);
            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            removedRepositoryMock.Verify(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRemovedRecord>>()), Times.Once);
            removedRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [TestCase(CustomerStatus.Active)]
        [TestCase(CustomerStatus.ActiveAccepted)]
        [TestCase(CustomerStatus.Completed)]
        public void UpdateCustomerStatusWithTimeIntersectionTest(CustomerStatus customerStatus)
        {
            var workerId = Guid.NewGuid();
            var currentRecordId = Guid.NewGuid();
            var currentRecordId2 = Guid.NewGuid();

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()),
                                                          builder => builder.WithRecordStatus(RecordStatus.Canceled)
                                                                            .WithCustomerStatus(CustomerStatus.CanceledBeforeEvent))
                                               .AddRecord(currentRecordId2, new TimePeriod(5.Hours(), 7.Hours()))
                                               .Build()
                                       ));

            var validation = calendarService.UpdateCustomerStatusAsync(shopId, now, workerId, currentRecordId, customerStatus).Result;

            validation.IsSuccess.Should().BeFalse();

            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void MoveRecordToAnotherWorker()
        {
            var workerId = Guid.NewGuid();
            var anotherWorkerId = Guid.NewGuid();
            var currentRecordId = Guid.NewGuid();
            var anotherRecordId = Guid.NewGuid();
            WorkerCalendarDay<ServiceCalendarRecord>[] savedDays = null;

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                               .Build()
                                       ));

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, anotherWorkerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(anotherWorkerId, now)
                                               .AddRecord(anotherRecordId, new TimePeriod(7.Hours(), 9.Hours()))
                                               .Build()
                                       ));

            mainRepositoryMock.Setup(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>[]>()))
                              .Returns(Task.FromResult(0))
                              .Callback((Action<Guid, WorkerCalendarDay<ServiceCalendarRecord>[]>)((s, d) => savedDays = d));

            var validation = calendarService.UpdateAsync(shopId, now, workerId,
                                                         ServiceCalendarRecordBuilder
                                                             .Create(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                                             .Build(), updateWorkerId : anotherWorkerId).Result;

            validation.IsSuccess.Should().BeTrue();

            savedDays.Should().BeEquivalentTo(new object[]
                {
                    WorkerCalendarDayBuilder<ServiceCalendarRecord>
                        .Create(workerId, now)
                        .Build(),
                    WorkerCalendarDayBuilder<ServiceCalendarRecord>
                        .Create(anotherWorkerId, now)
                        .AddRecord(anotherRecordId, new TimePeriod(7.Hours(), 9.Hours()))
                        .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                        .Build()
                });

            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, anotherWorkerId), Times.Once);
            mainRepositoryMock.Verify(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>[]>()), Times.Once);
            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void MoveRecordToAnotherWorkerWithExistingSlot()
        {
            var workerId = Guid.NewGuid();
            var anotherWorkerId = Guid.NewGuid();
            var currentRecordId = Guid.NewGuid();
            var anotherRecordId = Guid.NewGuid();

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                               .Build()
                                       ));

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, anotherWorkerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(anotherWorkerId, now)
                                               .AddRecord(anotherRecordId, new TimePeriod(5.Hours(), 7.Hours()))
                                               .Build()
                                       ));

            var validation = calendarService.UpdateAsync(shopId, now, workerId,
                                                         ServiceCalendarRecordBuilder
                                                             .Create(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                                             .Build(), updateWorkerId : anotherWorkerId).Result;

            validation.IsSuccess.Should().BeFalse();

            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, anotherWorkerId), Times.Once);
            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void MoveRecordToAnotherWorkerWithExistingButCanceledSlot()
        {
            var workerId = Guid.NewGuid();
            var anotherWorkerId = Guid.NewGuid();
            var currentRecordId = Guid.NewGuid();
            var anotherRecordId = Guid.NewGuid();
            WorkerCalendarDay<ServiceCalendarRecord>[] savedDays = null;

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                               .Build()
                                       ));

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, anotherWorkerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(anotherWorkerId, now)
                                               .AddRecord(anotherRecordId, new TimePeriod(5.Hours(), 7.Hours()), rec => rec.WithRecordStatus(RecordStatus.Canceled))
                                               .Build()
                                       ));

            mainRepositoryMock.Setup(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>[]>()))
                              .Returns(Task.FromResult(0))
                              .Callback((Action<Guid, WorkerCalendarDay<ServiceCalendarRecord>[]>)((s, d) => savedDays = d));

            var validation = calendarService.UpdateAsync(shopId, now, workerId,
                                                         ServiceCalendarRecordBuilder
                                                             .Create(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                                             .Build(), updateWorkerId : anotherWorkerId).Result;

            validation.IsSuccess.Should().BeTrue();

            savedDays.Should().BeEquivalentTo(new object[]
                {
                    WorkerCalendarDayBuilder<ServiceCalendarRecord>
                        .Create(workerId, now)
                        .Build(),
                    WorkerCalendarDayBuilder<ServiceCalendarRecord>
                        .Create(anotherWorkerId, now)
                        .AddRecord(anotherRecordId, new TimePeriod(5.Hours(), 7.Hours()), rec => rec.WithRecordStatus(RecordStatus.Canceled))
                        .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                        .Build()
                });

            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, anotherWorkerId), Times.Once);
            mainRepositoryMock.Verify(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>[]>()), Times.Once);
            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void MoveRecordToAnotherWorkerUnexistingRecord()
        {
            var workerId = Guid.NewGuid();
            var anotherWorkerId = Guid.NewGuid();
            var currentRecordId = Guid.NewGuid();
            var anotherRecordId = Guid.NewGuid();

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .Build()
                                       ));

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, anotherWorkerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(anotherWorkerId, now)
                                               .AddRecord(anotherRecordId, new TimePeriod(5.Hours(), 7.Hours()))
                                               .Build()
                                       ));

            var validation = calendarService.UpdateAsync(shopId, now, workerId,
                                                         ServiceCalendarRecordBuilder
                                                             .Create(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                                             .Build(), updateWorkerId : anotherWorkerId).Result;

            validation.IsSuccess.Should().BeFalse();

            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, anotherWorkerId), Times.Once);
            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [Test, Timeout(10000)]
        public void MoveRecordToAnotherWorkerDeadLockTest()
        {
            var workerId = Guid.NewGuid();
            var anotherWorkerId = Guid.NewGuid();
            var currentRecordId = Guid.NewGuid();
            var anotherRecordId = Guid.NewGuid();
            locker.Delay = 500;

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                               .Build()
                                       ));

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, anotherWorkerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(anotherWorkerId, now)
                                               .AddRecord(anotherRecordId, new TimePeriod(7.Hours(), 9.Hours()))
                                               .Build()
                                       ));

            mainRepositoryMock.Setup(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>[]>()))
                              .Returns(Task.FromResult(0));

            var task1 = calendarService.UpdateAsync(shopId, now, workerId,
                                                    ServiceCalendarRecordBuilder
                                                        .Create(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                                        .Build(), updateWorkerId : anotherWorkerId);
            var task2 = calendarService.UpdateAsync(shopId, now, anotherWorkerId,
                                                    ServiceCalendarRecordBuilder
                                                        .Create(anotherRecordId, new TimePeriod(7.Hours(), 9.Hours()))
                                                        .Build(), updateWorkerId : workerId);
            ;

            Task.WaitAll(task1, task2);

            task1.Result.IsSuccess.Should().BeTrue();
            task2.Result.IsSuccess.Should().BeTrue();

            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Exactly(2));
            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, anotherWorkerId), Times.Exactly(2));
            mainRepositoryMock.Verify(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>[]>()), Times.Exactly(2));
            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void MoveRecordToAnotherDateSameMonth()
        {
            var workerId = Guid.NewGuid();
            var anotherDate = now.Day > 15 ? now.AddDays(-5) : now.AddDays(5);
            var currentRecordId = Guid.NewGuid();
            var anotherRecordId = Guid.NewGuid();
            WorkerCalendarDay<ServiceCalendarRecord>[] savedDays = null;

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                               .Build()
                                       ));

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, anotherDate, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, anotherDate)
                                               .AddRecord(anotherRecordId, new TimePeriod(7.Hours(), 9.Hours()))
                                               .Build()
                                       ));

            mainRepositoryMock.Setup(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>[]>()))
                              .Returns(Task.FromResult(0))
                              .Callback((Action<Guid, WorkerCalendarDay<ServiceCalendarRecord>[]>)((s, d) => savedDays = d));

            var validation = calendarService.UpdateAsync(shopId, now, workerId,
                                                         ServiceCalendarRecordBuilder
                                                             .Create(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                                             .Build(), anotherDate).Result;

            validation.IsSuccess.Should().BeTrue();

            savedDays.Should().BeEquivalentTo(new object[]
                {
                    WorkerCalendarDayBuilder<ServiceCalendarRecord>
                        .Create(workerId, now)
                        .Build(),
                    WorkerCalendarDayBuilder<ServiceCalendarRecord>
                        .Create(workerId, anotherDate)
                        .AddRecord(anotherRecordId, new TimePeriod(7.Hours(), 9.Hours()))
                        .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                        .Build()
                });

            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, anotherDate, workerId), Times.Once);
            mainRepositoryMock.Verify(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>[]>()), Times.Once);
            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void MoveRecordToAnotherMonth()
        {
            var workerId = Guid.NewGuid();
            var anotherDate = now.AddMonths(1);
            var currentRecordId = Guid.NewGuid();
            var anotherRecordId = Guid.NewGuid();
            List<WorkerCalendarDay<ServiceCalendarRecord>> savedDays = new List<WorkerCalendarDay<ServiceCalendarRecord>>();

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                               .Build()
                                       ));

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, anotherDate, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, anotherDate)
                                               .AddRecord(anotherRecordId, new TimePeriod(7.Hours(), 9.Hours()))
                                               .Build()
                                       ));

            mainRepositoryMock.Setup(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>>()))
                              .Returns(Task.FromResult(0))
                              .Callback((Action<Guid, WorkerCalendarDay<ServiceCalendarRecord>>)((s, d) => savedDays.Add(d)));

            var validation = calendarService.UpdateAsync(shopId, now, workerId,
                                                         ServiceCalendarRecordBuilder
                                                             .Create(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                                             .Build(), anotherDate).Result;

            validation.IsSuccess.Should().BeTrue();

            savedDays.Should().BeEquivalentTo(new object[]
                {
                    WorkerCalendarDayBuilder<ServiceCalendarRecord>
                        .Create(workerId, now)
                        .Build(),
                    WorkerCalendarDayBuilder<ServiceCalendarRecord>
                        .Create(workerId, anotherDate)
                        .AddRecord(anotherRecordId, new TimePeriod(7.Hours(), 9.Hours()))
                        .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                        .Build()
                });

            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, anotherDate, workerId), Times.Once);
            mainRepositoryMock.Verify(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>>()), Times.Exactly(2));
            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void MoveRecordToAnotherDateWithExistingSlot()
        {
            var workerId = Guid.NewGuid();
            var anotherDate = now.Day > 15 ? now.AddDays(-5) : now.AddDays(5);
            var currentRecordId = Guid.NewGuid();
            var anotherRecordId = Guid.NewGuid();

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                               .Build()
                                       ));

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, anotherDate, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, anotherDate)
                                               .AddRecord(anotherRecordId, new TimePeriod(5.Hours(), 7.Hours()))
                                               .Build()
                                       ));

            var validation = calendarService.UpdateAsync(shopId, now, workerId,
                                                         ServiceCalendarRecordBuilder
                                                             .Create(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                                             .Build(), anotherDate).Result;

            validation.IsSuccess.Should().BeFalse();

            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, anotherDate, workerId), Times.Once);
            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void MoveRecordToAnotherDateWithExistingButCanceledSlot()
        {
            var workerId = Guid.NewGuid();
            var anotherDate = now.Day > 15 ? now.AddDays(-5) : now.AddDays(5);
            var currentRecordId = Guid.NewGuid();
            var anotherRecordId = Guid.NewGuid();
            WorkerCalendarDay<ServiceCalendarRecord>[] savedDays = null;

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                               .Build()
                                       ));

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, anotherDate, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, anotherDate)
                                               .AddRecord(anotherRecordId, new TimePeriod(5.Hours(), 7.Hours()), rec => rec.WithRecordStatus(RecordStatus.Canceled))
                                               .Build()
                                       ));

            mainRepositoryMock.Setup(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>[]>()))
                              .Returns(Task.FromResult(0))
                              .Callback((Action<Guid, WorkerCalendarDay<ServiceCalendarRecord>[]>)((s, d) => savedDays = d));

            var validation = calendarService.UpdateAsync(shopId, now, workerId,
                                                         ServiceCalendarRecordBuilder
                                                             .Create(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                                             .Build(), anotherDate).Result;

            validation.IsSuccess.Should().BeTrue();

            savedDays.Should().BeEquivalentTo(new object[]
                {
                    WorkerCalendarDayBuilder<ServiceCalendarRecord>
                        .Create(workerId, now)
                        .Build(),
                    WorkerCalendarDayBuilder<ServiceCalendarRecord>
                        .Create(workerId, anotherDate)
                        .AddRecord(anotherRecordId, new TimePeriod(5.Hours(), 7.Hours()), rec => rec.WithRecordStatus(RecordStatus.Canceled))
                        .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                        .Build()
                });

            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, anotherDate, workerId), Times.Once);
            mainRepositoryMock.Verify(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>[]>()), Times.Once);
            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void MoveRecordToAnotherDateUnexistingRecord()
        {
            var workerId = Guid.NewGuid();
            var anotherDate = now.Day > 15 ? now.AddDays(-5) : now.AddDays(5);
            var currentRecordId = Guid.NewGuid();
            var anotherRecordId = Guid.NewGuid();

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .Build()
                                       ));

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, anotherDate, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, anotherDate)
                                               .AddRecord(anotherRecordId, new TimePeriod(5.Hours(), 7.Hours()))
                                               .Build()
                                       ));

            var validation = calendarService.UpdateAsync(shopId, now, workerId,
                                                         ServiceCalendarRecordBuilder
                                                             .Create(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                                             .Build(), anotherDate).Result;

            validation.IsSuccess.Should().BeFalse();

            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, anotherDate, workerId), Times.Once);
            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void UpdateBothWorkerIdAndDate()
        {
            var workerId = Guid.NewGuid();
            var anotherWorkerId = Guid.NewGuid();
            var anotherDate = now.Day > 15 ? now.AddDays(-5) : now.AddDays(5);
            var currentRecordId = Guid.NewGuid();
            var anotherRecordId = Guid.NewGuid();
            WorkerCalendarDay<ServiceCalendarRecord>[] savedDays = null;

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(workerId, now)
                                               .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                               .Build()
                                       ));

            mainRepositoryMock.Setup(x => x.ReadWorkerCalendarDayAsync(shopId, anotherDate, anotherWorkerId))
                              .Returns(Task.FromResult(
                                           WorkerCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(anotherWorkerId, anotherDate)
                                               .AddRecord(anotherRecordId, new TimePeriod(7.Hours(), 9.Hours()))
                                               .Build()
                                       ));

            mainRepositoryMock.Setup(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>[]>()))
                              .Returns(Task.FromResult(0))
                              .Callback((Action<Guid, WorkerCalendarDay<ServiceCalendarRecord>[]>)((s, d) => savedDays = d));

            var validation = calendarService.UpdateAsync(shopId, now, workerId,
                                                         ServiceCalendarRecordBuilder
                                                             .Create(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                                                             .Build(), anotherDate, anotherWorkerId).Result;

            validation.IsSuccess.Should().BeTrue();

            savedDays.Should().BeEquivalentTo(new object[]
                {
                    WorkerCalendarDayBuilder<ServiceCalendarRecord>
                        .Create(workerId, now)
                        .Build(),
                    WorkerCalendarDayBuilder<ServiceCalendarRecord>
                        .Create(anotherWorkerId, anotherDate)
                        .AddRecord(anotherRecordId, new TimePeriod(7.Hours(), 9.Hours()))
                        .AddRecord(currentRecordId, new TimePeriod(4.Hours(), 6.Hours()))
                        .Build()
                });

            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, now, workerId), Times.Once);
            mainRepositoryMock.Verify(x => x.ReadWorkerCalendarDayAsync(shopId, anotherDate, anotherWorkerId), Times.Once);
            mainRepositoryMock.Verify(x => x.WriteAsync(shopId, It.IsAny<WorkerCalendarDay<ServiceCalendarRecord>[]>()), Times.Once);
            mainRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void ReadCalendarDayTest()
        {
            var workerId = Guid.NewGuid();
            var recordId1 = Guid.NewGuid();
            var recordId2 = Guid.NewGuid();

            mainRepositoryMock.Setup(x => x.ReadShopCalendarDayAsync(shopId, now))
                              .Returns(() => Task.FromResult(
                                           ShopCalendarDayBuilder<ServiceCalendarRecord>
                                               .Create(shopId, now)
                                               .AddWorkerDay(workerId,
                                                             workerDay => workerDay
                                                                          .AddRecord(recordId1, new TimePeriod(6.Hours(), 8.Hours()))
                                                                          .AddRecord(recordId2, new TimePeriod(7.Hours(), 9.Hours()),
                                                                                     record => record.WithRecordStatus(RecordStatus.Canceled)))
                                               .Build()));

            var shopDay = calendarService.ReadShopCalendarDayAsync(shopId, now).Result;
            var shopDayWithActive = calendarService.ReadShopCalendarDayAsync(shopId, now, RecordStatus.Active).Result;
            var shopDayWithCanceled = calendarService.ReadShopCalendarDayAsync(shopId, now, RecordStatus.Canceled).Result;

            shopDay.Should().BeEquivalentTo(ShopCalendarDayBuilder<ServiceCalendarRecord>
                                            .Create(shopId, now)
                                            .AddWorkerDay(workerId,
                                                          workerDay => workerDay
                                                                       .AddRecord(recordId1, new TimePeriod(6.Hours(), 8.Hours()))
                                                                       .AddRecord(recordId2, new TimePeriod(7.Hours(), 9.Hours()),
                                                                                  record => record.WithRecordStatus(RecordStatus.Canceled)))
                                            .Build());

            shopDayWithActive.Should().BeEquivalentTo(ShopCalendarDayBuilder<ServiceCalendarRecord>
                                                      .Create(shopId, now)
                                                      .AddWorkerDay(workerId,
                                                                    workerDay => workerDay
                                                                        .AddRecord(recordId1, new TimePeriod(6.Hours(), 8.Hours())))
                                                      .Build());

            shopDayWithCanceled.Should().BeEquivalentTo(ShopCalendarDayBuilder<ServiceCalendarRecord>
                                                        .Create(shopId, now)
                                                        .AddWorkerDay(workerId,
                                                                      workerDay => workerDay
                                                                          .AddRecord(recordId2, new TimePeriod(7.Hours(), 9.Hours()),
                                                                                     record => record.WithRecordStatus(RecordStatus.Canceled)))
                                                        .Build());
        }

        private ICalendarService calendarService;
        private Mock<ICalendarRepository<ServiceCalendarRecord>> mainRepositoryMock;
        private Mock<ICalendarRepository<ServiceCalendarRemovedRecord>> removedRepositoryMock;
        private Guid shopId;
        private DateTime now;
        private FakeLocker locker;
    }
}