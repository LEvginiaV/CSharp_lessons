using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Models.TimeZones;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.ServiceApi.Client;
using Market.CustomersAndStaff.Tests.Core.Configuration;

using NUnit.Framework;

namespace Market.CustomersAndStaff.ApiTests
{
    [TestFixture]
    public class ShopTimeZoneApiTests : IMainSuite
    {
        [GroboSetUp]
        public void SetUp()
        {
            shopId = Guid.NewGuid();
            var serviceApiClient = new ServiceApiClient(new ServiceApiClientSettings(new[] {new Uri("http://localhost:16001"),}, TimeSpan.FromSeconds(15), null));
            shopLocalTimeApiClient = serviceApiClient.ShopTimeZone;
        }

        [Test]
        public async Task ReadNonEmpty()
        {
            var timeZone = TimeZoneList.TimeZones.First();
            await timeZoneSettingsRepository.SetAsync(shopId, timeZone.Id);

            var actual = await shopLocalTimeApiClient.Get(shopId);

            actual.Should().Be(timeZone.Offset);
        }

        [Test]
        public async Task ReadEmpty()
        {
            var localTime = await shopLocalTimeApiClient.Get(shopId);

            localTime.Should().Be(null);
        }

        [Injected]
        private ITimeZoneSettingsRepository timeZoneSettingsRepository;

        private Guid shopId;
        private IShopTimeZoneApiClient shopLocalTimeApiClient;
    }
}