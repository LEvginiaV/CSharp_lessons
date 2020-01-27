using Market.Api.Models.Organizations;
using Market.Api.Models.Products;
using Market.Api.Models.Shops;
using Market.CustomersAndStaff.EvrikaPrintClient.Models.WorkOrderPrintDtos;
using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.Models.WorkOrders;

namespace Market.CustomersAndStaff.FrontApi.Converters.WorkOrders
{
    public interface IWorkOrderPrintConverter
    {
        WorkOrderPrintDto Convert(WorkOrder workOrder, Shop shop, Organization organization, Customer customer, Product[] products, Worker[] workers, bool invoice);
    }
}