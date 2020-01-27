using System;

using Cassandra.Mapping.Attributes;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.Commons.Mapping.Declarative.AttributesDefinition;

namespace Market.CustomersAndStaff.Repositories.StoredModels
{
    [CassandraTable(Keyspace = RepositoryConstants.KeyspaceName, Table = "customers")]
    public class CustomerStorageElement
    {
        [PartitionKey]
        public Guid OrganizationId { get; set; }

        [ClusteringKey]
        public Guid Id { get; set; }

        [Column]
        public int Number { get; set; }

        [Column]
        public string Name { get; set; }

        [Column]
        public string Birthday { get; set; }

        [Column]
        public string Phone { get; set; }

        [Column]
        public string Email { get; set; }

        [Column]
        public string CustomId { get; set; }

        [Column]
        public decimal? Discount { get; set; }

        [Column]
        public string Gender { get; set; }

        [Column]
        public bool IsDeleted { get; set; }

        [Column]
        public string AdditionalInfo { get; set; }
    }
}