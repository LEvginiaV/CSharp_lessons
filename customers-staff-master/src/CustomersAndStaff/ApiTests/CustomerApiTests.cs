using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using AutoFixture;

using FluentAssertions;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.ServiceApi.Client;
using Market.CustomersAndStaff.ServiceApi.Client.Core;
using Market.CustomersAndStaff.Tests.Core.Configuration;

using MoreLinq;

using NUnit.Framework;

namespace Market.CustomersAndStaff.ApiTests
{
    [TestFixture]
    public class CustomerApiTests : IMainSuite
    {
        [GroboSetUp]
        public void SetUp()
        {
            organizationId = Guid.NewGuid();
            serviceApiClient = new ServiceApiClient(new ServiceApiClientSettings(new[] {new Uri("http://localhost:16001"),}, TimeSpan.FromSeconds(15), null));
            customerApiClient = serviceApiClient.Customers;
        }

        [Test]
        public async Task WriteThenRead()
        {
            var expected = await customerApiClient.CreateAsync(organizationId, fixture.Create<Customer>());

            var dbCustomer = await customerApiClient.ReadAsync(organizationId, expected.Id);

            dbCustomer.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task UpdateVersionOnFirstWrite()
        {
            var startVersion = await customerApiClient.GetVersionAsync(organizationId);

            await customerApiClient.CreateAsync(organizationId, fixture.Create<Customer>());
            var testVersion = await customerApiClient.GetVersionAsync(organizationId);

            testVersion.Should().NotBe(startVersion);
        }

        [Test]
        public async Task UpdateVersionAfterSeveralWrites()
        {
            for(int i = 0; i < 10; i++)
            {
                customerApiClient.CreateAsync(organizationId, fixture.Create<Customer>()).Wait();
            }

            var startVersion = await customerApiClient.GetVersionAsync(organizationId);

            await customerApiClient.CreateAsync(organizationId, fixture.Create<Customer>());
            var testVersion = await customerApiClient.GetVersionAsync(organizationId);

            testVersion.Should().NotBe(startVersion);
        }

        [Test]
        public async Task WriteManyTest()
        {
            var customers = fixture.CreateMany<Customer>(100).ToArray();
            var expected = await customerApiClient.CreateManyAsync(organizationId, customers);

            customers = await customerApiClient.ReadByOrganizationAsync(organizationId, true);

            customers.Should().BeEquivalentTo(expected);
            customers.Should().HaveCount(100);
        }

        [Test]
        public async Task ReadWithoutDeletedTest()
        {
            var customers = fixture.CreateMany<Customer>(100).ToArray();
            customers.ForEach((x, i) => x.IsDeleted = i % 3 == 0);
            var expected = await customerApiClient.CreateManyAsync(organizationId, customers);

            customers = await customerApiClient.ReadByOrganizationAsync(organizationId);

            customers.Should().BeEquivalentTo(expected.Where(x => !x.IsDeleted));
            customers.Should().HaveCount(66);
        }

        [Test]
        public async Task UpdateFieldsTest()
        {
            var original = await customerApiClient.CreateAsync(organizationId, fixture.Create<Customer>());
            var expected = fixture.Create<Customer>();

            await customerApiClient.UpdateAsync(organizationId, original.Id, expected);

            expected.OrganizationId = organizationId;
            expected.Id = original.Id;

            var updatedCustomer = await customerApiClient.ReadAsync(organizationId, original.Id);
            updatedCustomer.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void UpdateNotFoundTest()
        {
            ((Action)(
                         () => customerApiClient.UpdateAsync(Guid.NewGuid(), Guid.NewGuid(), fixture.Create<Customer>()).Wait()))
                .Should()
                .Throw<HttpResponseException>()
                .Which
                .StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Injected]
        private readonly Fixture fixture;

        private ICustomerApiClient customerApiClient;

        private IServiceApiClient serviceApiClient;
        private Guid organizationId;
    }
}