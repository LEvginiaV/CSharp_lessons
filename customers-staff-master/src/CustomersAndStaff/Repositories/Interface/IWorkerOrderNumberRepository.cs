using System;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Models.WorkOrders;

namespace Market.CustomersAndStaff.Repositories.Interface
{
    public interface IWorkerOrderNumberRepository
    {
        Task<WorkOrderNumber> ReserveFirstAvailableNumberAsync(Guid shopId, WorkOrderNumber number);
        Task<bool> TryMakeNumberUsedAsync(Guid shopId, WorkOrderNumber number);
        Task FreeNumberAsync(Guid shopId, WorkOrderNumber number);
    }
}