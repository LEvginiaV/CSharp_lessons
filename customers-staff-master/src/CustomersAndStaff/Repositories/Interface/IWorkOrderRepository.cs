using System;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Models.WorkOrders;

namespace Market.CustomersAndStaff.Repositories.Interface
{
    public interface IWorkOrderRepository
    {
        Task<Guid> CreateAsync(Guid shopId, WorkOrder workOrder);
        Task WriteAsync(Guid shopId, WorkOrder workOrder);
        Task<WorkOrder> ReadAsync(Guid shopId, Guid workOrderId);
        Task<WorkOrder[]> ReadByShopAsync(Guid shopId, bool includeDeleted = false);
        Task<WorkOrder[]> ReadInfoByShopAsync(Guid shopId);
        Task<WorkOrderNumber?> GetLastWorkOrderNumberAsync(Guid shopId);
        Task<WorkOrder> GetLastWorkOrder(Guid shopId);
    }
}