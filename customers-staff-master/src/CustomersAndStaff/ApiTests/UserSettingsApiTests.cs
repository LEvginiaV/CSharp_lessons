using System;
using System.Threading.Tasks;

using FluentAssertions;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.ServiceApi.Client;
using Market.CustomersAndStaff.Tests.Core.Configuration;

using NUnit.Framework;

namespace Market.CustomersAndStaff.ApiTests
{
    [TestFixture]
    public class UserSettingsApiTests : IMainSuite
    {
        [GroboSetUp]
        public void SetUp()
        {
            serviceApiClient = new ServiceApiClient(new ServiceApiClientSettings(new[] {new Uri("http://localhost:16001"),}, TimeSpan.FromSeconds(15), null));
            userSettingsApiClient = serviceApiClient.UserSettingsApiClient;
        }

        [Test]
        public async Task WriteThenRead()
        {
            var userId = Guid.NewGuid();
            var settingKey = Guid.NewGuid().ToString();
            var settingValue = Guid.NewGuid().ToString();
            await userSettingsApiClient.UpdateAsync(userId, settingKey, settingValue);

            var settings = await userSettingsApiClient.ReadAsync(userId);

            settings.ContainsKey(settingKey).Should().BeTrue();
            settings[settingKey].Should().Be(settingValue);
        }

        [Test]
        public async Task ReadNonExisted()
        {
            var settings = await userSettingsApiClient.ReadAsync(Guid.NewGuid());
            settings.Count.Should().Be(0);
        }

        private IUserSettingsApiClient userSettingsApiClient;

        private IServiceApiClient serviceApiClient;
    }
}