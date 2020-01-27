using System;
using System.Threading.Tasks;

using FluentAssertions;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Tests.Core.Configuration;

using NUnit.Framework;

namespace Market.CustomersAndStaff.RepositoriesTests.TimeZoneTests
{
    public class TimeZoneTest : IMainSuite
    {
        [Test]
        public async Task WriteReadTest()
        {
            var shopId = Guid.NewGuid();
            var timeZoneId = Guid.NewGuid();

            await timeZoneSettingsRepository.SetAsync(shopId, timeZoneId);
            (await timeZoneSettingsRepository.GetAsync(shopId)).Should().Be(timeZoneId);
        }

        [Test]
        public async Task ReadEmptyTest()
        {
            (await timeZoneSettingsRepository.GetAsync(Guid.NewGuid())).Should().Be(null);
        }

        [Injected]
        private ITimeZoneSettingsRepository timeZoneSettingsRepository;
    }
}