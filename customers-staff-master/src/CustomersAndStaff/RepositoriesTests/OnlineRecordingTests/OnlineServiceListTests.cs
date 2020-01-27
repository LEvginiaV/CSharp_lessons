using System;
using System.Linq;
using System.Threading.Tasks;

using AutoFixture;

using FluentAssertions;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.Models.OnlineRecording;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Tests.Core.Configuration;

using NUnit.Framework;

namespace Market.CustomersAndStaff.RepositoriesTests.OnlineRecordingTests
{
    public class OnlineServiceListTests : IMainSuite
    {
        [Test]
        public async Task ReadEmpty()
        {
            var dbServices = await onlineServiceListRepository.ReadAsync(Guid.NewGuid());
            dbServices.Should().BeEmpty();
        }

        [Test]
        public async Task WriteThenRead()
        {
            var services = fixture.CreateMany<OnlineService>(10).ToArray();
            var shopId = Guid.NewGuid();

            await onlineServiceListRepository.WriteAsync(shopId, services);
            var dbServices = await onlineServiceListRepository.ReadAsync(shopId);

            dbServices.Should().BeEquivalentTo(services);
        }

        [Injected]
        private readonly IOnlineServiceListRepository onlineServiceListRepository;

        [Injected]
        private readonly IFixture fixture;
    }
}