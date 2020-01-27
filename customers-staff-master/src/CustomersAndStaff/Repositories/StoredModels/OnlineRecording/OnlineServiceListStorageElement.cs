using System;

using Cassandra.Mapping.Attributes;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.Commons.Mapping.Declarative.AttributesDefinition;

namespace Market.CustomersAndStaff.Repositories.StoredModels.OnlineRecording
{
    [CassandraTable(Keyspace = RepositoryConstants.KeyspaceName, Table = "onlineservicelist")]
    public class OnlineServiceListStorageElement
    {
        [PartitionKey]
        public Guid ShopId { get; set; }
        [Column]
        public byte[] SerializedList { get; set; }
    }
}