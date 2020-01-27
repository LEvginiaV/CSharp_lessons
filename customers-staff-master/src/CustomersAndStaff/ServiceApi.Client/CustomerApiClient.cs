using System;
using System.Threading.Tasks;

using Kontur.Clusterclient.Core.Model;

using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.ServiceApi.Client.Core;

namespace Market.CustomersAndStaff.ServiceApi.Client
{
    public class CustomerApiClient : ICustomerApiClient
    {
        public CustomerApiClient(ISimpleClusterClient clusterClient)
        {
            this.clusterClient = clusterClient;
        }

        public async Task<Customer> CreateAsync(Guid organizationId, Customer customer)
        {
            var result = await clusterClient.SendAsync(
                             Request.Post(Url(organizationId, c => c))
                                    .WithContentTypeApplicationJsonHeader()
                                    .WithContent(customer.SerializeDto())
                         );
            return result.DeserializeDto<Customer>();
        }

        public async Task<Customer[]> CreateManyAsync(Guid organizationId, Customer[] customers)
        {
            var result = await clusterClient.SendAsync(
                             Request.Post(Url(organizationId, c => c.AppendToPath("batch")))
                                    .WithContentTypeApplicationJsonHeader()
                                    .WithContent(customers.SerializeDto())
                         );
            return result.DeserializeDto<Customer[]>();
        }

        public async Task UpdateAsync(Guid organizationId, Guid customerId, Customer customer)
        {
            var result = await clusterClient.SendAsync(
                             Request.Post(Url(organizationId, c => c.AppendToPath(customerId)))
                                    .WithContentTypeApplicationJsonHeader()
                                    .WithContent(customer.SerializeDto())
                         );
            result.CheckResponseSuccessful();
        }

        public async Task<Customer> ReadAsync(Guid organizationId, Guid customerId)
        {
            var result = await clusterClient.SendAsync(
                             Request.Get(Url(organizationId, c => c.AppendToPath(customerId)))
                         );
            return result.DeserializeDto<Customer>();
        }

        public async Task<Customer[]> ReadByOrganizationAsync(Guid organizationId, bool includeDeleted = false)
        {
            var result = await clusterClient.SendAsync(
                             Request.Get(Url(organizationId, c => c.AppendToQuery("includeDeleted", includeDeleted)))
                         );
            return result.DeserializeDto<Customer[]>();
        }

        public async Task<int> GetVersionAsync(Guid organizationId)
        {
            var result = await clusterClient.SendAsync(
                             Request.Get(Url(organizationId, c => c.AppendToPath("version")))
                         );
            return result.DeserializeDto<int>();
        }

        private Uri Url(Guid organizationId, Func<RequestUrlBuilder, RequestUrlBuilder> conf)
        {
            return conf(
                    new RequestUrlBuilder()
                        .AppendToPath(organizationId)
                        .AppendToPath("customer")
                )
                .Build();
        }

        private readonly ISimpleClusterClient clusterClient;
    }
}