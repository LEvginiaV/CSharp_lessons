using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoFixture;

using AutoMapper;

using FluentAssertions;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Repositories.StoredModels;
using Market.CustomersAndStaff.Tests.Core.Configuration;

using MoreLinq;

using NUnit.Framework;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.SessionTableQueryExtending.PrimitiveStoring;

namespace Market.CustomersAndStaff.RepositoriesTests.CustomerTests
{
    public class CustomerRepositoryTests : IMainSuite
    {
        [GroboSetUp]
        public void SetUp()
        {
            organizationId = Guid.NewGuid();
            mapper = new Mapper(new MapperConfiguration(x => x.CreateMap<Customer, Customer>()));
        }

        [Test]
        public async Task WriteThenRead()
        {
            var expected = await customerRepository.CreateAsync(organizationId, fixture.Create<Customer>());

            var dbCustomer = await customerRepository.ReadAsync(organizationId, expected.Id);

            dbCustomer.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task UpdateVersionOnFirstWrite()
        {
            var startVersion = await customerRepository.GetVersionAsync(organizationId);

            await customerRepository.CreateAsync(organizationId, fixture.Create<Customer>());
            var testVersion = await customerRepository.GetVersionAsync(organizationId);

            testVersion.Should().NotBe(startVersion);
        }

        [Test]
        public async Task UpdateVersionAfterSeveralWrites()
        {
            for(int i = 0; i < 10; i++)
            {
                customerRepository.CreateAsync(organizationId, fixture.Create<Customer>()).Wait();
            }

            var startVersion = await customerRepository.GetVersionAsync(organizationId);

            await customerRepository.CreateAsync(organizationId, fixture.Create<Customer>());
            var testVersion = await customerRepository.GetVersionAsync(organizationId);

            testVersion.Should().NotBe(startVersion);
        }

        [Test]
        public async Task WriteManyTest()
        {
            var customers = fixture.CreateMany<Customer>(100).ToArray();
            var expected = await customerRepository.CreateManyAsync(organizationId, customers);

            customers = await customerRepository.ReadByOrganizationAsync(organizationId, true);

            customers.Should().BeEquivalentTo(expected);
            customers.Should().HaveCount(100);
        }

        [Test]
        public async Task ReadWithoutDeletedTest()
        {
            var customers = fixture.CreateMany<Customer>(100).ToArray();
            customers.ForEach((x, i) => x.IsDeleted = i % 3 == 0);
            var expected = await customerRepository.CreateManyAsync(organizationId, customers);

            customers = await customerRepository.ReadByOrganizationAsync(organizationId);

            customers.Should().BeEquivalentTo(expected.Where(x => !x.IsDeleted));
            customers.Should().HaveCount(66);
        }

        [Test]
        public async Task UpdateFieldsTest()
        {
            var original = await customerRepository.CreateAsync(organizationId, fixture.Create<Customer>());
            var expected = fixture.Create<Customer>();

            await customerRepository.UpdateAsync(organizationId, original.Id, customer => { mapper.Map(expected, customer); });

            expected.OrganizationId = organizationId;
            expected.Id = original.Id;

            var updatedCustomer = await customerRepository.ReadAsync(organizationId, original.Id);
            updatedCustomer.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void UpdateNotFoundTest()
        {
            Assert.CatchAsync(typeof(KeyNotFoundException),
                              () => customerRepository.UpdateAsync(Guid.NewGuid(), Guid.NewGuid(), _ => { }));
        }

        [Test]
        public async Task FindByPhoneEmpty()
        {
            await customerRepository.CreateAsync(organizationId, fixture.Create<Customer>());

            var result = await customerRepository.FindByPhoneAsync(organizationId, fixture.Create<string>());
            result.Should().BeEmpty();
        }

        [Test]
        public async Task FindSingleByPhone()
        {
            await customerRepository.CreateAsync(organizationId, fixture.Create<Customer>());
            var expected = await customerRepository.CreateAsync(organizationId, fixture.Create<Customer>());

            var result = await customerRepository.FindByPhoneAsync(organizationId, expected.Phone);

            result.Should().BeEquivalentTo(new[] {expected});
        }

        [Test]
        public async Task FindManyByPhone()
        {
            var phone = fixture.Create<string>();
            var customers = fixture.CreateMany<Customer>(20).ToArray();
            customers.Take(10).ForEach(x => x.Phone = phone);

            var expected = (await customerRepository.CreateManyAsync(organizationId, customers)).Take(10);
            var result = await customerRepository.FindByPhoneAsync(organizationId, phone);

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task FindByPhoneAfterUpdate()
        {
            var expected = await customerRepository.CreateAsync(organizationId, fixture.Create<Customer>());
            var previousPhone = expected.Phone;
            var phone = fixture.Create<string>();

            await customerRepository.UpdateAsync(organizationId, expected.Id, customer => customer.Phone = phone);

            expected.Phone = phone;

            (await customerRepository.FindByPhoneAsync(organizationId, previousPhone)).Should().BeEmpty();
            var result = await customerRepository.FindByPhoneAsync(organizationId, phone);
            result.Should().BeEquivalentTo(new[] {expected});
        }

        [Test]
        public async Task UpdateIndex()
        {
            var expected = await customerRepository.CreateAsync(organizationId, fixture.Create<Customer>());
            var phone = fixture.Create<string>();

            var element = await storage.FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.Id == expected.Id);
            element.Phone = phone;
            await storage.WriteAsync(element);
            expected.Phone = phone;

            (await customerRepository.FindByPhoneAsync(organizationId, phone)).Should().BeEmpty();

            await customerRepository.UpdateIndexAsync(organizationId);

            (await customerRepository.FindByPhoneAsync(organizationId, phone)).Should().BeEquivalentTo(new[] {expected});
        }

        private Guid organizationId;

        [Injected]
        private readonly ICustomerRepository customerRepository;

        [Injected]
        private readonly CassandraStorage<CustomerStorageElement> storage;

        [Injected]
        private IFixture fixture;

        private IMapper mapper;
    }
}