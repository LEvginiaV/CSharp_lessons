using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using GroboContainer.NUnitExtensions;

using Market.Api.Models.Products;
using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Calendar;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.CalendarPageExtensions;
using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Models.ServiceCalendar;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Utils.Extensions;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.CalendarTests
{
    public class CalendarTestBase : TestBase
    {
        protected async Task CreateOneWorkingDay(Guid workerId, DateTime date, TimePeriod period)
        {
            var workingDay = new WorkerCalendarDay<WorkerScheduleRecord>
                {
                    WorkerId = workerId,
                    Date = date,
                    Records = new[] {new WorkerScheduleRecord {Period = period}}
                };
            await workerScheduleRepository.WriteAsync(shop.Id, new [] {workingDay});
        }

        protected async Task CreateSingleRecord(Guid workerId, DateTime date, ServiceCalendarRecord record)
        {
            await serviceCalendarRepository.WriteAsync(shop.Id, new WorkerCalendarDay<ServiceCalendarRecord>
                {
                    Date = date,
                    WorkerId = workerId,
                    Records = new[] {record}
                });
        }
        
        protected async Task CreateSingleRecord(Guid workerId, DateTime date, TimePeriod timePeriod, Guid? customerId = null, string comment = null)
        {
            await serviceCalendarRepository.WriteAsync(shop.Id, new WorkerCalendarDay<ServiceCalendarRecord>
                {
                    Date = date,
                    WorkerId = workerId,
                    Records = new[]
                        {
                            new ServiceCalendarRecord {Period = timePeriod, CustomerId = customerId, Comment = comment},
                        }
                });
        }
        

        protected async Task CheckSingleRecord(Guid workerId, DateTime date, TimePeriod timePeriod)
        {
            var calendarDay = await serviceCalendarRepository.ReadWorkerCalendarDayAsync(shop.Id, date, workerId);
            calendarDay.Records[0].Period.Should().BeEquivalentTo(timePeriod);
        }

        protected async Task<ServiceCalendarRecord> ReadSingleRecordTomorrow(Guid workerId)
        {
            return (await ReadWorkerCalendarDay(workerId, DateHelper.Tomorrow())).Records.Single();
        }

        protected async Task<WorkerCalendarDay<ServiceCalendarRecord>> ReadWorkerCalendarDay(Guid workerId, DateTime date)
        {
            return await serviceCalendarRepository.ReadWorkerCalendarDayAsync(shop.Id, date, workerId);
        }

        protected async Task<Guid> CreateWorker(string name = "Василий")
        {
            return (await workerRepository.CreateAsync(shop.Id, new Worker {FullName = name})).Id;
        }

        protected async Task<Guid> CreateCustomer(string name = "Иван")
        {
            return (await customerRepository.CreateAsync(shop.OrganizationId, new Customer {Name = name})).Id;
        }

        protected async Task<Customer[]> ReadCustomersByOrganisation()
        {
            return await customerRepository.ReadByOrganizationAsync(shop.OrganizationId);
        }

        protected async Task<Product> GetServiceCardWithPrice(Product[] products = null) =>
            (await GetServicesCards(products, 1))[0];
        
        protected async Task<Product> GetServiceCardWithoutPrice(Product[] products = null) =>
            (await GetServicesCards(products, 1, false))[0];

        protected async Task<Product[]> GetServicesCards(Product[] products = null, int count = 1, bool withPrice = true)
        {
            if(products == null)
            {
                products = await marketApiClient.Products.GetAll(shop.Id);
            }

            return products.Where(x => x.PricesInfo?.SellPrice != null ? withPrice : !withPrice)
                           .Where(x => x.ProductCategory == ProductCategory.Service)
                           .Take(count)
                           .ToArray();
        }

        protected async Task<Product> GetServiceCard()
        {
            return (await marketApiClient.Products.GetAll(shop.Id)).First(x => x.ProductCategory == ProductCategory.Service);
        }
            

        protected CalendarPage GoToTomorrowCalendarPage() => LoadMainPage().GoToCalendarPage().GoToNextDay();

        [Injected]
        protected readonly IWorkerRepository workerRepository;

        [Injected]
        protected readonly ICustomerRepository customerRepository;

        [Injected]
        protected readonly ICalendarRepository<WorkerScheduleRecord> workerScheduleRepository;
        
        [Injected]
        protected readonly ICalendarRepository<ServiceCalendarRecord> serviceCalendarRepository;
    }
}