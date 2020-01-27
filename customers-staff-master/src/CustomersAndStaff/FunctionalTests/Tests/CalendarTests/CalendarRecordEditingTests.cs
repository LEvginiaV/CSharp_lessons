using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Calendar;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.CalendarPageExtensions;
using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Models.ServiceCalendar;
using Market.CustomersAndStaff.Utils.Extensions;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.CalendarTests
{
    public class CalendarRecordEditingTests : CalendarTestBase
    {
        [Test]
        [Description("Заполнение полей записи")]
        public async Task FillingRecordFields()
        {
            const string customerName = "Иван";
            const string commentMessage = "Comment message";

            var service = await GetServiceCard();

            var workerId = await CreateWorker();
            var customerId = await CreateCustomer(customerName);
            await CreateOneWorkingDay(workerId, DateHelper.Tomorrow(), TimePeriod.CreateByHours(10, 16));
            await CreateSingleRecord(workerId, DateHelper.Tomorrow(), TimePeriod.CreateByHours(13, 14));

            var modal = GoToTomorrowCalendarPage()
                .ChangeRecord(0, 0);

            
            modal.SelectCustomerByName(customerName)
                 .SelectServicesByNames(service.Name)
                 .SetTimeRange("14:00", "15:00")
                 .SetComment(commentMessage)
                 .SaveAndClose();

            var record = await ReadSingleRecordTomorrow(workerId);

            record.CustomerId.Should().Be(customerId);
            record.Comment.Should().BeEquivalentTo(commentMessage);
            record.ProductIds.Single().Should().Be(service.Id.Value);
            record.Period.Should().BeEquivalentTo(TimePeriod.CreateByHours(14, 15));
        }

        [Test]
        [Description("Удаление полей записи")]
        public async Task FieldRemoving()
        {
            var recordTimePeriod = TimePeriod.CreateByHours(13, 14);
            var commentMessage = "Comment Message";
            var customerName = "Иван";

            var service = await GetServiceCard();

            var workerId = await CreateWorker();
            var customerId = await CreateCustomer(customerName);
            await CreateOneWorkingDay(workerId, DateHelper.Tomorrow(), TimePeriod.CreateByHours(10, 16));
            await CreateSingleRecord(workerId, DateHelper.Tomorrow(), new ServiceCalendarRecord
                {
                    Period = recordTimePeriod,
                    CustomerId = customerId,
                    Comment = commentMessage,
                    ProductIds = new [] {service.Id.Value}
                });
            
            var modal = GoToTomorrowCalendarPage()
                .ChangeRecord(0, 0);
            
            modal.EraseCustomer()
                 .RemoveLastService()
                 .SetComment("")
                 .SaveAndClose();
            
            var record = await ReadSingleRecordTomorrow(workerId);
            
            record.CustomerId.Should().BeNull();
            string.IsNullOrEmpty(record.Comment).Should().BeTrue();
            record.ProductIds.Length.Should().Be(0);
            record.Period.Should().BeEquivalentTo(recordTimePeriod);
        }

        [Test]
        [Description("Удаление услуги с розничной ценой и услуги без цены, проверка значения суммы")]
        public async Task RemoveServices()
        {
            var withPrice = await GetServiceCardWithPrice();
            var withoutPrice = await GetServiceCardWithoutPrice();

            var workerId = await CreateWorker();
            await CreateOneWorkingDay(workerId, DateHelper.Tomorrow(), TimePeriod.CreateByHours(10, 16));
            await CreateSingleRecord(workerId, DateHelper.Tomorrow(), new ServiceCalendarRecord
                {
                    Period = TimePeriod.CreateByHours(13, 14),
                    ProductIds = new [] {withPrice.Id.Value, withoutPrice.Id.Value}
                });
            
            var modal = GoToTomorrowCalendarPage()
                .ChangeRecord(0, 0);

            modal.CheckFooterTotal(300)
                 .RemoveLastService()
                 .CheckFooterTotal(300)
                 .RemoveLastService()
                 .CheckFooterTotal(null);
        }

        [Test]
        [Description("Добавление услуги с розничной ценой и услуги без цены, проверка значения суммы")]
        public async Task AddServicesToExistingRecord()
        {
            var withPrice = await GetServicesCards(count: 2); 
            var withoutPrice = await GetServiceCardWithoutPrice();
            
            var workerId = await CreateWorker();
            await CreateOneWorkingDay(workerId, DateHelper.Tomorrow(), TimePeriod.CreateByHours(10, 16));
            await CreateSingleRecord(workerId, DateHelper.Tomorrow(), new ServiceCalendarRecord
                {
                    Period = TimePeriod.CreateByHours(13, 14),
                    ProductIds = new [] {withPrice[0].Id.Value}
                });
            
            var page = GoToTomorrowCalendarPage();
            page.ChangeRecord(0, 0)
                .CheckFooterTotal(withPrice[0].PricesInfo.SellPrice)
                .Close();

            var day = await ReadWorkerCalendarDay(workerId, DateHelper.Tomorrow());
            var record = day.Records.Single();
            record.ProductIds = record.ProductIds.Concat(new[] {withoutPrice.Id.Value, withPrice[1].Id.Value}).ToArray();
            await serviceCalendarRepository.WriteAsync(shop.Id, day);

            page = page.Refresh<CalendarPage>();
            page.ChangeRecord(0, 0).CheckFooterTotal(withPrice.Sum(x => x.PricesInfo.SellPrice));
        }
    }
}
