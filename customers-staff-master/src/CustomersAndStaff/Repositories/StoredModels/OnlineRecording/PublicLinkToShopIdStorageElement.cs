using System;

using Cassandra.Mapping.Attributes;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.Commons.Mapping.Declarative.AttributesDefinition;

namespace Market.CustomersAndStaff.Repositories.StoredModels.OnlineRecording
{
    [CassandraTable(Keyspace = RepositoryConstants.KeyspaceName, Table = "publiclinktoshopid")]
    public class PublicLinkToShopIdStorageElement
    {
        [PartitionKey]
        public string Link { get; set; }
        [Column]
        public Guid ShopId { get; set; }
        [Column]
        public bool IsActive { get; set; }
    }
}