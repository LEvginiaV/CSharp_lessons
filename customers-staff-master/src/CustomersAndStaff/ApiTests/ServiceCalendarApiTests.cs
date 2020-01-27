using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Models.ServiceCalendar;
using Market.CustomersAndStaff.ServiceApi.Client;
using Market.CustomersAndStaff.Tests.Core.Configuration;

using NUnit.Framework;

namespace Market.CustomersAndStaff.ApiTests
{
    [TestFixture]
    public class ServiceCalendarApiTests : IMainSuite
    {
        [GroboSetUp]
        public void SetUp()
        {
            serviceApiClient = new ServiceApiClient(new ServiceApiClientSettings(new[] {new Uri("http://localhost:16001"),}, TimeSpan.FromSeconds(15), null));
            serviceCalendarApiClient = serviceApiClient.ServiceCalendar;
        }

        [Test]
        public async Task WriteThenRead()
        {
            var shopId = Guid.NewGuid();
            var workerId = Guid.NewGuid();
            
            var record = new ServiceCalendarRecord { Period = TimePeriod.CreateByHours(12, 13), Comment = "test" };
            
            record.Id = await serviceCalendarApiClient.CreateRecord(shopId, DateTime.Now, workerId, record);
            
            var day = await serviceCalendarApiClient.GetShopDay(shopId, DateTime.Now);
            
            day.WorkerCalendarDays.Single().Records.Single().Should().BeEquivalentTo(record);
        }

        [Test]
        public async Task ReadNonExisted()
        {
            var day = await serviceCalendarApiClient.GetShopDay(Guid.NewGuid(), DateTime.Now);
            day.WorkerCalendarDays.Length.Should().Be(0);
        }

        private IServiceCalendarApiClient serviceCalendarApiClient;

        private IServiceApiClient serviceApiClient;
    }
}