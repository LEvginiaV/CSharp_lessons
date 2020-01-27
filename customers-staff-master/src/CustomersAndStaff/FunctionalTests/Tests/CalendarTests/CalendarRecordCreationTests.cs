using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Market.CustomersAndStaff.FunctionalTests.Components.Pages;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.CalendarPageExtensions;
using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Utils.Extensions;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.CalendarTests
{
    public class CalendarRecordCreationTests : CalendarTestBase
    {
        [Test]
        [Description("Создание записи на свободное рабочее время сотрудника и подсчет суммы")]
        public async Task CreateRecordAndCheckSum()
        {
            const string customerName = "Иван";
            
            var withPrice = await GetServiceCardWithPrice();
            var withoutPrice = await GetServiceCardWithoutPrice();
            
            var workerId = await CreateWorker();
            var customerId = await CreateCustomer(customerName);
            
            var modal = GoToTomorrowCalendarPage()
                .OpenCalendarModalAtHour(0, 12);
            
            modal.SelectCustomerByName(customerName)
                 .SelectServicesByNames(withPrice.Name, withoutPrice.Name)
                 .CheckFooterTotal(withPrice.PricesInfo.SellPrice)
                 .SaveAndClose();

            var expectedProductIds = new[] {withPrice.Id.Value, withoutPrice.Id.Value};
            
            var record = await ReadSingleRecordTomorrow(workerId);
            record.CustomerId.Should().Be(customerId);
            record.ProductIds.Should().BeEquivalentTo(expectedProductIds);
        }

        [Test]
        [Description("Создание записи на переиод времени 23:00 - 24:00")]
        public async Task CreateRecordAtLastHour()
        {
            var workerId = await CreateWorker();

            var modal = GoToTomorrowCalendarPage()
                .OpenCalendarModalAtHour(0, 23);
            
            modal.CheckTimeRange("23:00", "24:00")
                 .SaveAndClose();

            await CheckSingleRecord(workerId, DateHelper.Tomorrow(), TimePeriod.CreateByHours(23, 24));
        }

        [Test]
        [Description("Создание записи на свободное рабочее время сотрудника с отменой")]
        public async Task StartCreateRecordAndCancel()
        {
            var workerId = await CreateWorker();

            GoToTomorrowCalendarPage()
                .OpenCalendarModalAtHour(0, 12)
                .Close();

            var workerCalendarDay = await ReadWorkerCalendarDay(workerId, DateHelper.Tomorrow());
            workerCalendarDay.Records.Length.Should().Be(0);
        }

        [Test]
        [Description("Создание записи, сотрудник занят")]
        public async Task CreateRecordButWorkerIsBusy()
        {
            var initialPeriod = TimePeriod.CreateByHours(13, 14);
            var workerId = await CreateWorker();
            
            await CreateOneWorkingDay(workerId, DateHelper.Tomorrow(), TimePeriod.CreateByHours(10, 16));
            await CreateSingleRecord(workerId, DateHelper.Tomorrow(), initialPeriod);

            var page = GoToTomorrowCalendarPage();
            var modal = page.OpenCalendarModalAtHour(0, 12);
            modal.SetTimeRange("12:30", "13:30");
            modal.SetComment("");
            modal.CheckTimeValidation("Попадает на другую запись, сотрудник будет занят", Level.Error, ErrorSide.LeftAndRight);
            modal.ClickSave();
            modal.WaitPresence(); // ничего не происходит, окно остаётся открытым
            modal.Close();

            var record = await ReadSingleRecordTomorrow(workerId);
            record.Period.Should().BeEquivalentTo(initialPeriod);
        }

        [Test]
        [Description("Создание записи на нерабочее время сотрудника")]
        public async Task CreateRecordAtWorkerDayOff()
        {
            var workerId = await CreateWorker();
            
            var page = LoadMainPage().GoToCalendarPage()
                                     .GoToNextDay()
                                     .GoToNextDay()
                                     .GoToNextDay();
            
            var modal = page.OpenCalendarModalAtHour(0, 12);
            modal.CheckTimeValidation("Попадет на нерабочее время сотрудника", Level.Warning)
                 .SaveAndClose();

            var workingDay = await ReadWorkerCalendarDay(workerId, DateTime.Now.AddDays(3));
            workingDay.Records.Single().Period.Should().BeEquivalentTo(TimePeriod.CreateByHours(12, 13));
        }

        [Test]
        [Description("Смена клиента с созданием новых клиентов: совсем новые и с аналогичным ФИО")]
        public async Task ChangeCustomerToNewWithSameName()
        {
            const string customerName = "Иван";
            const string newCustomerName = "Петр из ZZ Top 1969";
            const string newCustomerPhone = "+7 012 345-67-89";
            const string newCustomerPhone2 = "7999 999-99-99";
            
            var workerId = await CreateWorker();
            await CreateOneWorkingDay(workerId, DateHelper.Tomorrow(), TimePeriod.CreateByHours(10, 16));
            
            var customerId = await CreateCustomer(customerName);
            await CreateSingleRecord(workerId, DateHelper.Tomorrow(), TimePeriod.CreateByHours(12, 13), customerId);

            var page = GoToTomorrowCalendarPage();

            var modal = page.ChangeRecord(0, 0);

            modal.EraseCustomer()
                 .AddNewCustomer(newCustomerName)
                 .SaveAndClose();

            (await customerRepository.ReadByOrganizationAsync(shop.OrganizationId))
                .Select(x => x.Name)
                .Should()
                .BeEquivalentTo(customerName, newCustomerName);

            page.CheckNameOnRecord(newCustomerName, 0, 0);
            
            modal = page.ChangeRecord(0, 0);
            modal.CheckCustomerName(newCustomerName)
                 .EraseCustomer()
                 .AddNewCustomer(newCustomerPhone)
                 .SaveAndClose();

            var customerWithPhone = (await ReadCustomersByOrganisation()).Single(x => !string.IsNullOrEmpty(x.Phone));
            customerWithPhone.Phone.Should().BeEquivalentTo(PhoneDbFormat(newCustomerPhone));

            page.CheckNameOnRecord(newCustomerPhone, 0, 0);
            modal = page.ChangeRecord(0, 0);
            modal.CheckCustomerPhone(newCustomerPhone)
                 .EraseCustomer()
                 .SearchCustomer(newCustomerName)
                 .CheckSuggestedCustomers(newCustomerName)
                 .AddNewCustomer(newCustomerName)
                 .CheckCustomerNameInEditor(newCustomerName)
                 .SetCustomerPhone(newCustomerPhone2.Substring(1))
                 .SaveAndClose();


            var customersWithPhone = (await ReadCustomersByOrganisation()).Where(x => !string.IsNullOrEmpty(x.Phone));
            var lastCustomer = customersWithPhone.Single(x => x.Phone.Equals(PhoneDbFormat(newCustomerPhone2)));

            var record = await ReadSingleRecordTomorrow(workerId);
            record.CustomerId.Should().Be(lastCustomer.Id);
        }

        [Test]
        [Description("Добавление клиента при создании записи с отменой добавления клиента")]
        public async Task CreateRecordAndCancelCustomerCreation()
        {
            var workerId = await CreateWorker();
            await CreateOneWorkingDay(workerId, DateHelper.Tomorrow(), TimePeriod.CreateByHours(10, 16));

            var modal = GoToTomorrowCalendarPage()
                .OpenCalendarModalAtHour(0, 12);
            
            modal.AddNewCustomer("Петр")
                 .CancelAddingNewCustomer()
                 .SaveAndClose();

            (await ReadSingleRecordTomorrow(workerId)).CustomerId.Should().BeNull();
        }

        [Test]
        [Description("Выбор другого сотрудника при создании записи")]
        public async Task ChangeWorkerInRecord()
        {
            const string workerName1 = "Анатолий";
            const string workerName2 = "Василий";
            var workerId1 = await CreateWorker(workerName1);
            var workerId2 = await CreateWorker(workerName2);
            await CreateOneWorkingDay(workerId1, DateHelper.Tomorrow(), TimePeriod.CreateByHours(10, 16));
            await CreateSingleRecord(workerId1, DateHelper.Tomorrow(), TimePeriod.CreateByHours(12, 13));
            
            var page = GoToTomorrowCalendarPage();

            page.ChangeRecord(0, 0)
                .EraseWorker()
                .SelectWorkerByName(workerName2)
                .CheckSelectedWorkerName(workerName2)
                .SaveAndClose();

            (await ReadWorkerCalendarDay(workerId2, DateHelper.Tomorrow())).Records.Length.Should().Be(1);
            (await ReadWorkerCalendarDay(workerId1, DateHelper.Tomorrow())).Records.Length.Should().Be(0);
        }

        private static string PhoneDbFormat(string phone) => phone.Replace(" ", "").Replace("+", "").Replace("-", "");
    }
}
