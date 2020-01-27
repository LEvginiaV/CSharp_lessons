using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Repositories.Interface;

namespace Market.CustomersAndStaff.FunctionalTests.Helpers.Customers
{
    public class CustomerHelper : ICustomerHelper
    {
        public CustomerHelper(ICustomerRepository customerRepository)
        {
            this.customerRepository = customerRepository;
        }

        public async Task<Customer> CreateAsync(string name, string phone = null, string additionalInfo = null)
        {
            var customer = new Customer{Name = name, Phone = phone, AdditionalInfo = additionalInfo};
            return await customerRepository.CreateAsync(ContextHelper.GetCurrentOrganizationId(), customer);
        }

        public async Task CheckSingleCustomer(Customer expected)
        {
            var customer = (await customerRepository.ReadByOrganizationAsync(ContextHelper.GetCurrentOrganizationId())).Single();
            customer.Should().BeEquivalentTo(expected, cfg => cfg.Excluding(x => x.Id).Excluding(x => x.OrganizationId));
        }

        private readonly ICustomerRepository customerRepository;
    }
}