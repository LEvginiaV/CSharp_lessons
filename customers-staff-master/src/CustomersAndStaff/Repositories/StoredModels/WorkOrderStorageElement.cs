using System;

using Cassandra;
using Cassandra.Mapping;
using Cassandra.Mapping.Attributes;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.Commons.Mapping.Declarative.AttributesDefinition;

namespace Market.CustomersAndStaff.Repositories.StoredModels
{
    [CassandraTable(Keyspace = RepositoryConstants.KeyspaceName, Table = "work_order")]
    public class WorkOrderStorageElement
    {
        [PartitionKey]
        public Guid ShopId { get; set; }

        [ClusteringKey(ClusteringSortOrder = SortOrder.Descending)]
        public Guid OrderId { get; set; }

        [Column]
        public string DocumentStatus { get; set; }
        [Column]
        public string Status { get; set; }
        [Column]
        public string OrderNumber { get; set; }
        [Column]
        public LocalDate ReceptionDate { get; set; }
        [Column]
        public decimal TotalSum { get; set; }
        [Column]
        public Guid? FirstProductId { get; set; }
        [Column]
        public Guid ClientId { get; set; }
        [Column]
        public byte[] SerializedOrder { get; set; }
    }
}