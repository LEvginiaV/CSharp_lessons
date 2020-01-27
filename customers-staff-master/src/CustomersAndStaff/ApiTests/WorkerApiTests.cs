using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using AutoFixture;

using FluentAssertions;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.ServiceApi.Client;
using Market.CustomersAndStaff.ServiceApi.Client.Core;
using Market.CustomersAndStaff.Tests.Core.Configuration;

using MoreLinq;

using NUnit.Framework;

namespace Market.CustomersAndStaff.ApiTests
{
    public class WorkerApiTests : IMainSuite
    {
        [GroboSetUp]
        public void SetUp()
        {
            shopId = Guid.NewGuid();
            serviceApiClient = new ServiceApiClient(new ServiceApiClientSettings(new[] {new Uri("http://localhost:16001"),}, TimeSpan.FromSeconds(15), null));
            workerApiClient = serviceApiClient.Workers;
        }

        [Test]
        public async Task WriteThenRead()
        {
            var expected = await workerApiClient.CreateAsync(shopId, fixture.Create<Worker>());

            var dbWorker = await workerApiClient.ReadAsync(shopId, expected.Id);

            dbWorker.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task UpdateVersionOnFirstWrite()
        {
            var startVersion = await workerApiClient.GetVersionAsync(shopId);

            await workerApiClient.CreateAsync(shopId, fixture.Create<Worker>());
            var testVersion = await workerApiClient.GetVersionAsync(shopId);

            testVersion.Should().NotBe(startVersion);
        }

        [Test]
        public async Task UpdateVersionAfterSeveralWrites()
        {
            for(int i = 0; i < 10; i++)
            {
                workerApiClient.CreateAsync(shopId, fixture.Create<Worker>()).Wait();
            }

            var startVersion = await workerApiClient.GetVersionAsync(shopId);

            await workerApiClient.CreateAsync(shopId, fixture.Create<Worker>());
            var testVersion = await workerApiClient.GetVersionAsync(shopId);

            testVersion.Should().NotBe(startVersion);
        }

        [Test]
        public async Task WriteManyTest()
        {
            var workers = fixture.CreateMany<Worker>(100).ToArray();
            var expected = await workerApiClient.CreateManyAsync(shopId, workers);

            workers = await workerApiClient.ReadByOrganizationAsync(shopId, true);

            workers.Should().BeEquivalentTo(expected);
            workers.Should().HaveCount(100);
        }

        [Test]
        public async Task ReadWithoutDeletedTest()
        {
            var workers = fixture.CreateMany<Worker>(100).ToArray();
            workers.ForEach((x, i) => x.IsDeleted = i % 3 == 0);
            var expected = await workerApiClient.CreateManyAsync(shopId, workers);

            workers = await workerApiClient.ReadByOrganizationAsync(shopId);

            workers.Should().BeEquivalentTo(expected.Where(x => !x.IsDeleted));
            workers.Should().HaveCount(66);
        }

        [Test]
        public async Task UpdateFieldsTest()
        {
            var original = await workerApiClient.CreateAsync(shopId, fixture.Create<Worker>());
            var expected = fixture.Create<Worker>();

            await workerApiClient.UpdateAsync(shopId, original.Id, expected);

            expected.ShopId = shopId;
            expected.Id = original.Id;
            expected.Code = original.Code;

            var updatedWorker = await workerApiClient.ReadAsync(shopId, original.Id);
            updatedWorker.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void UpdateNotFoundTest()
        {
            ((Action)(
                         () => workerApiClient.UpdateAsync(Guid.NewGuid(), Guid.NewGuid(), fixture.Create<Worker>()).Wait()))
                .Should()
                .Throw<HttpResponseException>()
                .Which
                .StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Injected]
        private readonly Fixture fixture;

        private IWorkerApiClient workerApiClient;

        private IServiceApiClient serviceApiClient;
        private Guid shopId;
    }
}