using System;

using Cassandra.Mapping.Attributes;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.Commons.Mapping.Declarative.AttributesDefinition;

namespace Market.CustomersAndStaff.Repositories.StoredModels
{
    [CassandraTable(Keyspace = RepositoryConstants.KeyspaceName, Table = "workers")]
    public class WorkerStorageElement
    {
        [PartitionKey]
        public Guid ShopId { get; set; }

        [ClusteringKey]
        public Guid Id { get; set; }

        [Column]
        public int Code { get; set; }

        [Column]
        public string FullName { get; set; }

        [Column]
        public string Phone { get; set; }

        [Column]
        public string Position { get; set; }

        [Column]
        public bool IsDeleted { get; set; }

        [Column]
        public bool IsAvailableOnline { get; set; }

        [Column]
        public string AdditionalInfo { get; set; }
    }
}