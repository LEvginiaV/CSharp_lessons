using System.Threading.Tasks;

using Market.CustomersAndStaff.Models.Customers;

namespace Market.CustomersAndStaff.FunctionalTests.Helpers.Customers
{
    public interface ICustomerHelper
    {
        Task CheckSingleCustomer(Customer expected);
        Task<Customer> CreateAsync(string name, string phone = null, string additionalInfo = null);
    }
}