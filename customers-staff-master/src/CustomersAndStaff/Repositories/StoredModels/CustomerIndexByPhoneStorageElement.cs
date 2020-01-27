using System;

using Cassandra.Mapping.Attributes;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.Commons.Mapping.Declarative.AttributesDefinition;

namespace Market.CustomersAndStaff.Repositories.StoredModels
{
    [CassandraTable(Keyspace = RepositoryConstants.KeyspaceName, Table = "customersphoneindex")]
    public class CustomerIndexByPhoneStorageElement
    {
        [PartitionKey]
        public Guid OrganizationId { get; set; }
        [ClusteringKey(0)]
        public string Phone { get; set; }
        [ClusteringKey(1)]
        public Guid CustomerId { get; set; }
    }
}