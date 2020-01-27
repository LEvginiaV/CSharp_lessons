using System;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Models.WorkOrders;

namespace Market.CustomersAndStaff.FunctionalTests.Helpers.WorkOrders
{
    public interface IWorkOrderHelper
    {
        Task CreateOrderAsync(Customer customer, Action<WorkOrderBuilder> builderAction = null);
        Task CreateOrderAsync(WorkOrder workOrder);
        Task CheckRepositoryIsEmpty();
        Task<WorkOrder> ReadSingleAsync();
    }
}