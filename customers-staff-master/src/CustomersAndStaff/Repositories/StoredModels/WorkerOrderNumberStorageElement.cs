using System;

using Cassandra.Mapping.Attributes;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.Commons.Mapping.Declarative.AttributesDefinition;

namespace Market.CustomersAndStaff.Repositories.StoredModels
{
    [CassandraTable(Keyspace = RepositoryConstants.KeyspaceName, Table = "worker_order_number_segments")]
    public class WorkerOrderNumberStorageElement
    {
        [PartitionKey]
        public Guid ShopId { get; set; }

        [ClusteringKey]
        public string Point { get; set; }

        [Column(Type = typeof(string))]
        public WorkerOrderNumberStatus Status { get; set; }
    }
}