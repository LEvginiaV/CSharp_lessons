using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.Models.WorkOrders;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Repositories.StoredModels;
using Market.CustomersAndStaff.Tests.Core.Configuration;

using NUnit.Framework;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.SessionTableQueryExtending.PrimitiveStoring;

namespace Market.CustomersAndStaff.RepositoriesTests.WorkOrderTests
{
    public class WorkOrderNumberRepositoryTests : IMainSuite
    {
        [GroboSetUp]
        public void SetUp()
        {
            shopId = Guid.NewGuid();
        }

        [Test]
        public async Task ReserveSingleNumberTest()
        {
            var generatedNumber = await workerOrderNumberRepository.ReserveFirstAvailableNumberAsync(shopId, new WorkOrderNumber("ВГ", 123456));
            generatedNumber.Should().Be(new WorkOrderNumber("ВГ", 123456));
        }

        [Test]
        public async Task MakeReservedNumberUsedTest()
        {
            var generatedNumber = await workerOrderNumberRepository.ReserveFirstAvailableNumberAsync(shopId, new WorkOrderNumber("ВГ", 123456));
            (await workerOrderNumberRepository.TryMakeNumberUsedAsync(shopId, new WorkOrderNumber("ВГ", 123456))).Should().BeTrue();
        }

        [Test]
        public async Task TryMakeExistingNumberUsedTest()
        {
            (await workerOrderNumberRepository.TryMakeNumberUsedAsync(shopId, new WorkOrderNumber("ВГ", 123456))).Should().BeTrue();
            (await workerOrderNumberRepository.TryMakeNumberUsedAsync(shopId, new WorkOrderNumber("ВГ", 123456))).Should().BeFalse();
        }

        [Test]
        public async Task FreeNumberTest()
        {
            (await workerOrderNumberRepository.TryMakeNumberUsedAsync(shopId, new WorkOrderNumber("ВГ", 123456))).Should().BeTrue();
            await workerOrderNumberRepository.FreeNumberAsync(shopId, new WorkOrderNumber("ВГ", 123456));
            (await workerOrderNumberRepository.TryMakeNumberUsedAsync(shopId, new WorkOrderNumber("ВГ", 123456))).Should().BeTrue();
        }

        [Test]
        public async Task ReserveFirstAvailableNumberTest()
        {
            await workerOrderNumberRepository.TryMakeNumberUsedAsync(shopId, new WorkOrderNumber("ВГ", 123456));
            var generatedNumber = await workerOrderNumberRepository.ReserveFirstAvailableNumberAsync(shopId, new WorkOrderNumber("ВГ", 123456));
            generatedNumber.Should().Be(new WorkOrderNumber("ВГ", 123457));
        }

        [Test]
        public async Task ReserveFirstAvailableNumberFromMaxNumberTest()
        {
            await workerOrderNumberRepository.TryMakeNumberUsedAsync(shopId, new WorkOrderNumber("ЯЯ", 999999));
            var generatedNumber = await workerOrderNumberRepository.ReserveFirstAvailableNumberAsync(shopId, new WorkOrderNumber("ЯЯ", 999999));
            generatedNumber.Should().Be(new WorkOrderNumber("АА", 000001));
        }

        [Test]
        public async Task ReserveFirstAvailableNumberOverflowTest()
        {
            await workerOrderNumberRepository.TryMakeNumberUsedAsync(shopId, new WorkOrderNumber("ЯЯ", 999998));
            await workerOrderNumberRepository.TryMakeNumberUsedAsync(shopId, new WorkOrderNumber("ЯЯ", 999999));
            var generatedNumber = await workerOrderNumberRepository.ReserveFirstAvailableNumberAsync(shopId, new WorkOrderNumber("ЯЯ", 999998));
            generatedNumber.Should().Be(new WorkOrderNumber("АА", 000001));
        }

        [TestCase(999)]
        [TestCase(1000)]
        [TestCase(1001)]
        [TestCase(99999)]
        public async Task ReserveFirstAvailableNumberAfterSeveralNumbersTest(int numbersCount)
        {
            var numbers = Enumerable.Range(0, numbersCount)
                                    .Select(i => new WorkOrderNumber("ВГ", 123456 + i))
                                    .Select(x => new WorkerOrderNumberStorageElement {ShopId = shopId, Point = x.ToString()})
                                    .ToArray();

            await storage.WriteAsync(numbers);

            var generatedNumber = await workerOrderNumberRepository.ReserveFirstAvailableNumberAsync(shopId, new WorkOrderNumber("ВГ", 123456));
            generatedNumber.Should().Be(new WorkOrderNumber("ВГ", 123456 + numbersCount));
        }

        [Injected]
        private IWorkerOrderNumberRepository workerOrderNumberRepository;

        [Injected]
        private CassandraStorage<WorkerOrderNumberStorageElement> storage;

        private Guid shopId;
    }
}