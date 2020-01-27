using System;

using Cassandra.Mapping.Attributes;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.Commons.Mapping.Declarative.AttributesDefinition;

namespace Market.CustomersAndStaff.Repositories.StoredModels
{
    [CassandraTable(Keyspace = RepositoryConstants.KeyspaceName, Table = "timezone_settings")]
    public class TimeZoneSettingsStorageElement
    {
        [PartitionKey]
        public Guid ShopId { get; set; }
        [Column]
        public Guid TimeZoneId { get; set; }
    }
}