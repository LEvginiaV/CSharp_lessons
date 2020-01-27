using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Models.WorkOrders;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Services.WorkOrders;

namespace Market.CustomersAndStaff.FunctionalTests.Helpers.WorkOrders
{
    public class WorkOrderHelper : IWorkOrderHelper
    {
        public WorkOrderHelper(IWorkOrderService workOrderService, IWorkOrderRepository workOrderRepository)
        {
            this.workOrderService = workOrderService;
            this.workOrderRepository = workOrderRepository;
        }

        public async Task CreateOrderAsync(WorkOrder workOrder)
        {
            await workOrderService.CreateNewOrderAsync(ContextHelper.GetCurrentShopId(), workOrder);
        }

        public async Task CreateOrderAsync(Customer customer, Action<WorkOrderBuilder> builderAction = null)
        {
            var builder = WorkOrderBuilder.CreateWithCustomer(customer);
            builderAction?.Invoke(builder);
            await workOrderService.CreateNewOrderAsync(ContextHelper.GetCurrentShopId(), builder.Build());
        }

        public async Task CheckRepositoryIsEmpty()
        {
            (await workOrderRepository.ReadByShopAsync(ContextHelper.GetCurrentShopId(), true)).Should().BeEmpty();
        }

        public async Task<WorkOrder> ReadSingleAsync()
        {
            return (await workOrderRepository.ReadByShopAsync(ContextHelper.GetCurrentShopId(), true)).Single();
        }

        private readonly IWorkOrderService workOrderService;
        private readonly IWorkOrderRepository workOrderRepository;
    }
}