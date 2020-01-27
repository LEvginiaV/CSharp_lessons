using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoFixture;

using AutoMapper;

using FluentAssertions;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Tests.Core.Configuration;

using MoreLinq;

using NUnit.Framework;

namespace Market.CustomersAndStaff.RepositoriesTests.WorkerTests
{
    public class WorkerRepositoryTests : IMainSuite
    {
        [GroboSetUp]
        public void SetUp()
        {
            fixture = new Fixture();
            shopId = Guid.NewGuid();

            mapper = new Mapper(new MapperConfiguration(x => x.CreateMap<Worker, Worker>()));
        }

        [Test]
        public async Task WriteThenRead()
        {
            var expected = await workerRepository.CreateAsync(shopId, fixture.Create<Worker>());

            var dbWorker = await workerRepository.ReadAsync(shopId, expected.Id);

            dbWorker.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task UpdateVersionOnFirstWrite()
        {
            var startVersion = await workerRepository.GetVersionAsync(shopId);

            await workerRepository.CreateAsync(shopId, fixture.Create<Worker>());
            var testVersion = await workerRepository.GetVersionAsync(shopId);

            testVersion.Should().NotBe(startVersion);
        }

        [Test]
        public async Task UpdateVersionAfterSeveralWrites()
        {
            for(int i = 0; i < 10; i++)
            {
                workerRepository.CreateAsync(shopId, fixture.Create<Worker>()).Wait();
            }

            var startVersion = await workerRepository.GetVersionAsync(shopId);

            await workerRepository.CreateAsync(shopId, fixture.Create<Worker>());
            var testVersion = await workerRepository.GetVersionAsync(shopId);

            testVersion.Should().NotBe(startVersion);
        }

        [Test]
        public async Task NumberGenerationTest()
        {
            for(int i = 0; i < 100; i++)
            {
                await workerRepository.CreateAsync(shopId, fixture.Create<Worker>());
            }

            var workers = await workerRepository.ReadByShopAsync(shopId, true);

            workers.Should().HaveCount(100);
            workers.Select(x => x.Code).Should().OnlyHaveUniqueItems();
            workers.ForEach(x => x.Code.Should().BePositive());
        }

        [Test]
        public async Task WriteManyTest()
        {
            var workers = fixture.CreateMany<Worker>(100).ToArray();
            var expected = await workerRepository.CreateManyAsync(shopId, workers);

            workers = await workerRepository.ReadByShopAsync(shopId, true);

            workers.Should().BeEquivalentTo(expected);
            workers.Should().HaveCount(100);
            workers.Select(x => x.Code).Should().OnlyHaveUniqueItems();
            workers.ForEach(x => x.Code.Should().BePositive());
        }

        [Test]
        public async Task ReadWithoutDeletedTest()
        {
            var workers = fixture.CreateMany<Worker>(100).ToArray();
            workers.ForEach((x, i) => x.IsDeleted = i % 3 == 0);
            var expected = await workerRepository.CreateManyAsync(shopId, workers);

            workers = await workerRepository.ReadByShopAsync(shopId);

            workers.Should().BeEquivalentTo(expected.Where(x => !x.IsDeleted));
            workers.Should().HaveCount(66);
            workers.Select(x => x.Code).Should().OnlyHaveUniqueItems();
            workers.ForEach(x => x.Code.Should().BePositive());
        }

        [Test]
        public async Task UpdateFieldsTest()
        {
            var original = await workerRepository.CreateAsync(shopId, fixture.Create<Worker>());
            var expected = fixture.Create<Worker>();

            await workerRepository.UpdateAsync(shopId, original.Id, customer => { mapper.Map(expected, customer); });

            expected.ShopId = shopId;
            expected.Id = original.Id;
            expected.Code = original.Code;

            var updatedWorker = await workerRepository.ReadAsync(shopId, original.Id);
            updatedWorker.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void UpdateNotFoundTest()
        {
            Assert.CatchAsync(typeof(KeyNotFoundException),
                              () => workerRepository.UpdateAsync(Guid.NewGuid(), Guid.NewGuid(), _ => { }));
        }

        private IFixture fixture;
        private Guid shopId;

        [Injected]
        private readonly IWorkerRepository workerRepository;

        private IMapper mapper;
    }
}