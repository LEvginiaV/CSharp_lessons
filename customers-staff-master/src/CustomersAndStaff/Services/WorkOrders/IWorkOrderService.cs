using System;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Models.Validations;
using Market.CustomersAndStaff.Models.WorkOrders;

namespace Market.CustomersAndStaff.Services.WorkOrders
{
    public interface IWorkOrderService
    {
        Task<(WorkOrderNumber OrderNumber, string AdditionalText)> GetCreateInfoAsync(Guid shopId);
        Task<(Guid, ValidationResult)> CreateNewOrderAsync(Guid shopId, WorkOrder order);
        Task<ValidationResult> SaveOrderAsync(Guid shopId, Guid orderId, WorkOrder order);
        Task<bool> RemoveOrderAsync(Guid shopId, Guid orderId);
        Task<WorkOrder[]> ReadOrderInfosAsync(Guid shopId);
        Task<WorkOrder> ReadOrderAsync(Guid shopId, Guid orderId);
        Task<ValidationResult> UpdateStatus(Guid shopId, Guid orderId, WorkOrderStatus workOrderStatus);
    }
}