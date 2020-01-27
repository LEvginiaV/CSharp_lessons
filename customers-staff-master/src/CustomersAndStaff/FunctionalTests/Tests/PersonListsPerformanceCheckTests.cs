using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using AutoFixture;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Customers;
using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.Repositories.Interface;

using MoreLinq;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests
{
    public class PersonListsPerformanceCheckTests : TestBase
    {
        [Test]
        [Description("Замеряем, сколько времени нужно для отрисовки списка и страницы с работниками")]
        [Explicit]
        public async Task WorkerListPerfCheck()
        {
            var workers = fixture.CreateMany<Worker>(60).ToArray();
            workers.ForEach(x => x.IsDeleted = false);
            await workerRepository.CreateManyAsync(shop.Id, workers);
            
            var workerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.WorkersLink.Click())
                    .GoToPage<WorkerListPage>();
            
            
            var enterTimes = new long[30];
            var leaveTimes = new long[30];

            for(var i = 0; i < 30; i++)
            {
                workerListPage.WorkerItem.WaitCount(30);

                var sw = Stopwatch.StartNew();
                workerListPage.WorkerItem.ElementAt(i).Click();
                var workerPage = workerListPage.GoToPage<WorkerPage>();
                sw.Stop();
                
                enterTimes[i] = sw.ElapsedMilliseconds;
                
                sw.Restart();
                workerPage.BackButton.Click();
                workerListPage = workerPage.GoToPage<WorkerListPage>();
                sw.Stop();

                leaveTimes[i] = sw.ElapsedMilliseconds;
            }

            Console.WriteLine($"Avg enter time: {enterTimes.Average()} ms based on:\n[" + string.Join(",", enterTimes) + "]");
            Console.WriteLine($"Avg leave time: {leaveTimes.Average()} ms based on:\n[" + string.Join(",", leaveTimes) + "]");
        }


        [Test]
        [Description("Замеряем, сколько времени нужно для отрисовки списка и страницы с клиентами")]
        [Explicit]
        public async Task CustomerListPerfCheck()
        {
            var customers = fixture.CreateMany<Customer>(60).ToArray();
            customers.ForEach(x => x.IsDeleted = false);
            await customerRepository.CreateManyAsync(shop.OrganizationId, customers);
            
            var customerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.CustomersLink.Click())
                    .GoToPage<CustomerListPage>();
            
            
            var enterTimes = new long[30];
            var leaveTimes = new long[30];

            for(var i = 0; i < 30; i++)
            {
                customerListPage.CustomerItem.WaitCount(50);

                var sw = Stopwatch.StartNew();
                customerListPage.CustomerItem.ElementAt(i).Click();
                var customerPage = customerListPage.GoToPage<CustomerPage>();
                sw.Stop();
                
                enterTimes[i] = sw.ElapsedMilliseconds;
                
                sw.Restart();
                customerPage.BackButton.Click();
                customerListPage = customerPage.GoToPage<CustomerListPage>();
                sw.Stop();

                leaveTimes[i] = sw.ElapsedMilliseconds;
            }

            Console.WriteLine($"Avg enter time: {enterTimes.Average()} ms based on:\n[" + string.Join(",", enterTimes) + "]");
            Console.WriteLine($"Avg leave time: {leaveTimes.Average()} ms based on:\n[" + string.Join(",", leaveTimes) + "]");
            
        }

        [Injected]
        private readonly IFixture fixture;

        [Injected]
        private readonly IWorkerRepository workerRepository;
        
        [Injected]
        private readonly ICustomerRepository customerRepository;
    }
}
