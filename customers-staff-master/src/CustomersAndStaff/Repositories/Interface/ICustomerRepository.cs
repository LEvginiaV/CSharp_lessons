using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Models.Customers;

namespace Market.CustomersAndStaff.Repositories.Interface
{
    public interface ICustomerRepository
    {
        Task<Customer> CreateAsync(Guid organizationId, Customer customer);
        Task<Customer[]> CreateManyAsync(Guid organizationId, Customer[] customers);
        Task UpdateAsync(Guid organizationId, Guid customerId, Action<Customer> update);
        Task<Customer> ReadAsync(Guid organizationId, Guid customerId);
        Task<Customer[]> ReadByOrganizationAsync(Guid organizationId, bool includeDeleted = false);
        IEnumerable<Customer> ReadAll();
        Task<Customer[]> FindByPhoneAsync(Guid organizationId, string phone);
        Task<int> GetVersionAsync(Guid organizationId);
        Task UpdateIndexAsync(Guid organizationId);
    }
}