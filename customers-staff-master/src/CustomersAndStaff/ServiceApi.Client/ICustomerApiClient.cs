using System;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Models.Customers;

namespace Market.CustomersAndStaff.ServiceApi.Client
{
    public interface ICustomerApiClient
    {
        Task<Customer> CreateAsync(Guid organizationId, Customer customer);
        Task<Customer[]> CreateManyAsync(Guid organizationId, Customer[] customers);
        Task UpdateAsync(Guid organizationId, Guid customerId, Customer customer);
        Task<Customer> ReadAsync(Guid organizationId, Guid customerId);
        Task<Customer[]> ReadByOrganizationAsync(Guid organizationId, bool includeDeleted = false);
        Task<int> GetVersionAsync(Guid organizationId);
    }
}