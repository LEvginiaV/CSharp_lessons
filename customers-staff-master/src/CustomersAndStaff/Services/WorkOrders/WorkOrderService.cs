using System;
using System.Linq;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Models.Validations;
using Market.CustomersAndStaff.Models.WorkOrders;
using Market.CustomersAndStaff.ModelValidators;
using Market.CustomersAndStaff.Repositories.Interface;

namespace Market.CustomersAndStaff.Services.WorkOrders
{
    public class WorkOrderService : IWorkOrderService
    {
        public WorkOrderService(
            IWorkOrderRepository workOrderRepository,
            IWorkerOrderNumberRepository workerOrderNumberRepository, 
            IValidator<WorkOrder> workOrderValidator)
        {
            this.workOrderRepository = workOrderRepository;
            this.workerOrderNumberRepository = workerOrderNumberRepository;
            this.workOrderValidator = workOrderValidator;
        }

        public async Task<(WorkOrderNumber OrderNumber, string AdditionalText)> GetCreateInfoAsync(Guid shopId)
        {
            var lastOrder = await workOrderRepository.GetLastWorkOrder(shopId);
            var currentLastNumber = lastOrder?.Number ?? WorkOrderNumber.Min;
            return (await workerOrderNumberRepository.ReserveFirstAvailableNumberAsync(shopId, currentLastNumber), lastOrder?.AdditionalText);
        }

        public async Task<(Guid, ValidationResult)> CreateNewOrderAsync(Guid shopId, WorkOrder order)
        {
            var result = workOrderValidator.Validate(order);
            if(!result.IsSuccess)
            {
                return (Guid.Empty, result);
            }

            if(!await workerOrderNumberRepository.TryMakeNumberUsedAsync(shopId, order.Number))
            {
                return (Guid.Empty, ValidationResult.Fail("orderNumberUsed", "order number is used"));
            }

            order.DocumentStatus = WorkOrderDocumentStatus.Saved;
            CalculateStats(order);
            var id = await workOrderRepository.CreateAsync(shopId, order);

            return (id, ValidationResult.Success());
        }

        public async Task<ValidationResult> SaveOrderAsync(Guid shopId, Guid orderId, WorkOrder order)
        {
            var result = workOrderValidator.Validate(order);
            if (!result.IsSuccess)
            {
                return result;
            }

            var prevOrder = await workOrderRepository.ReadAsync(shopId, orderId);
            if(prevOrder == null || order.DocumentStatus != WorkOrderDocumentStatus.Saved)
            {
                return ValidationResult.Fail("notFound", $"Order with id {orderId} not found");
            }

            order.Id = orderId;
            order.DocumentStatus = WorkOrderDocumentStatus.Saved;
            CalculateStats(order);

            if(prevOrder.Number != order.Number)
            {
                if(!await workerOrderNumberRepository.TryMakeNumberUsedAsync(shopId, order.Number))
                {
                    return ValidationResult.Fail("orderNumberUsed", "order number is used");
                }

                await workOrderRepository.WriteAsync(shopId, order);
                await workerOrderNumberRepository.FreeNumberAsync(shopId, prevOrder.Number);
            }
            else
            {
                await workOrderRepository.WriteAsync(shopId, order);
            }

            return ValidationResult.Success();
        }

        public async Task<ValidationResult> UpdateStatus(Guid shopId, Guid orderId, WorkOrderStatus workOrderStatus)
        {
            var order = await workOrderRepository.ReadAsync(shopId, orderId);
            if(order == null || order.DocumentStatus != WorkOrderDocumentStatus.Saved)
            {
                return ValidationResult.Fail("notFound", $"Order with id {orderId} not found");
            }

            order.Status = workOrderStatus;

            await workOrderRepository.WriteAsync(shopId, order);

            return ValidationResult.Success();
        }

        public async Task<bool> RemoveOrderAsync(Guid shopId, Guid orderId)
        {
            var order = await workOrderRepository.ReadAsync(shopId, orderId);
            if(order == null)
            {
                return false;
            }

            order.DocumentStatus = WorkOrderDocumentStatus.Removed;
            await workOrderRepository.WriteAsync(shopId, order);
            return true;
        }

        public async Task<WorkOrder[]> ReadOrderInfosAsync(Guid shopId)
        {
            return (await workOrderRepository.ReadInfoByShopAsync(shopId)).Where(x => x.DocumentStatus == WorkOrderDocumentStatus.Saved).ToArray();
        }

        public async Task<WorkOrder> ReadOrderAsync(Guid shopId, Guid orderId)
        {
            return await workOrderRepository.ReadAsync(shopId, orderId);
        }

        private void CalculateStats(WorkOrder order)
        {
            order.TotalSum = order.ShopServices.Sum(x => decimal.Round((x.Price ?? 0) * x.Quantity, 2)) +
                             order.ShopProducts.Sum(x => decimal.Round((x.Price ?? 0) * x.Quantity, 2));
            order.FirstProductId = order.ShopServices.Select(x => (Guid?)x.ProductId).FirstOrDefault()
                                   ?? order.ShopProducts.Select(x => (Guid?)x.ProductId).FirstOrDefault();
        }

        private readonly IWorkOrderRepository workOrderRepository;
        private readonly IWorkerOrderNumberRepository workerOrderNumberRepository;
        private readonly IValidator<WorkOrder> workOrderValidator;
    }
}