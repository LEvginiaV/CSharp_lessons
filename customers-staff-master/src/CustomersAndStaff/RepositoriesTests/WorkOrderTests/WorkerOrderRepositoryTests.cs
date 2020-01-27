using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoFixture;

using FluentAssertions;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.Models.WorkOrders;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Tests.Core.Configuration;

using NUnit.Framework;

namespace Market.CustomersAndStaff.RepositoriesTests.WorkOrderTests
{
    [WithCustomizedFixture]
    public class WorkerOrderRepositoryTests : IMainSuite
    {
        [GroboSetUp]
        public void SetUp()
        {
            shopId = Guid.NewGuid();
        }

        [Test]
        public async Task WriteReadTest()
        {
            var order = fixture.Create<WorkOrder>();
            await workOrderRepository.WriteAsync(shopId, order);
            (await workOrderRepository.ReadAsync(shopId, order.Id)).Should().BeEquivalentTo(order);
        }

        [Test]
        public async Task ReadInfoTest()
        {
            var order = fixture.Create<WorkOrder>();
            await workOrderRepository.WriteAsync(shopId, order);

            (await workOrderRepository.ReadInfoByShopAsync(shopId)).Should().BeEquivalentTo((IEnumerable<WorkOrder>)new []
                {
                    new WorkOrder
                        {
                            Id = order.Id,
                            Number = order.Number,
                            DocumentStatus = order.DocumentStatus,
                            Status = order.Status,
                            ReceptionDate = order.ReceptionDate.Date,
                            ClientId = order.ClientId,
                            TotalSum = order.TotalSum,
                            FirstProductId = order.FirstProductId
                        }, 
                });
        }

        [Test]
        public async Task WriteReadManyTest()
        {
            var orders = fixture.CreateMany<WorkOrder>(5).ToArray();
            await Task.WhenAll(orders.Select(x => workOrderRepository.WriteAsync(shopId, x)));
            (await workOrderRepository.ReadByShopAsync(shopId, true)).Should().BeEquivalentTo(orders.Cast<object>());
        }

        [Test]
        public async Task WriteReadManyWithoutDeletedTest()
        {
            var orders = fixture.CreateMany<WorkOrder>(5).ToArray();
            await Task.WhenAll(orders.Select(x => workOrderRepository.WriteAsync(shopId, x)));
            (await workOrderRepository.ReadByShopAsync(shopId)).Should().BeEquivalentTo(orders.Where(x => x.DocumentStatus == WorkOrderDocumentStatus.Saved).Cast<object>());
        }

        [Test]
        public async Task OrderTest()
        {
            var order = fixture.Create<WorkOrder>();
            
            var expectedIds = Enumerable.Range(0, 100).Select(x => workOrderRepository.CreateAsync(shopId, order).Result).Reverse().ToArray();
            var actualIds = (await workOrderRepository.ReadInfoByShopAsync(shopId)).Select(x => x.Id).ToArray();
            actualIds.Should().BeEquivalentTo(expectedIds, cfg => cfg.WithStrictOrdering());
        }

        [Test]
        public async Task ReadEmptyWorkOrderNumberTest()
        {
            (await workOrderRepository.GetLastWorkOrderNumberAsync(shopId)).Should().BeEquivalentTo((WorkOrderNumber?)null);
        }

        [Test]
        public async Task ReadLastWorkOrderNumberTest()
        {
            var order = fixture.Create<WorkOrder>();
            await workOrderRepository.CreateAsync(shopId, fixture.Create<WorkOrder>());
            await workOrderRepository.CreateAsync(shopId, order);

            (await workOrderRepository.GetLastWorkOrderNumberAsync(shopId)).Should().BeEquivalentTo(order.Number);
        }

        [Injected]
        private IWorkOrderRepository workOrderRepository;
        [Injected]
        private IFixture fixture;

        private Guid shopId;
    }
}