using System.Linq;

using Cassandra.Data.Linq;

using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Repositories.StoredModels;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.SessionTableQueryExtending.PrimitiveStoring;

namespace Market.CustomersAndStaff.GodLikeTools.FillPhoneIndex
{
    public class FillPhoneIndex : ICommandProcessor
    {
        public FillPhoneIndex(
            CassandraStorage<CustomerStorageElement> customerStorage,
            ICustomerRepository customerRepository)
        {
            this.customerStorage = customerStorage;
            this.customerRepository = customerRepository;
        }

        public void Run(ICommandLine commandLine)
        {
            var organizationIds = customerStorage.Table.Select(x => x.OrganizationId).Execute().Distinct();

            organizationIds.AsParallel()
                           .WithDegreeOfParallelism(10)
                           .ForAll(organizationId => customerRepository.UpdateIndexAsync(organizationId).Wait());
        }

        private readonly CassandraStorage<CustomerStorageElement> customerStorage;
        private readonly ICustomerRepository customerRepository;
    }
}